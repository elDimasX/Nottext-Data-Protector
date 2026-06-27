using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Nottext_Data_Protector.Models;

namespace Nottext_Data_Protector.Services
{
    public static class ProtectionStore
    {
        private static readonly Encoding Encode = Encoding.GetEncoding("iso-8859-1");

        public static readonly string[] FileProtectionOptions =
        {
            "Somente Leitura",
            "Bloquear",
            "Ocultar",
            "Não Executar",
            "Proteger Processo"
        };

        public static ObservableCollection<ProtectionEntry> LoadFileEntries()
        {
            var entries = new ObservableCollection<ProtectionEntry>();
            WithDriverAccess(delegate
            {
                AddLines(entries, Global.arquivoSomenteLeitura, "Somente Leitura");
                AddLines(entries, Global.arquivoBloquear, "Bloquear");
                AddLines(entries, Global.arquivoOcultar, "Ocultar");
                AddLines(entries, Global.arquivoNaoExecutar, "Não Executar");
                AddLines(entries, Global.arquivoProtegerProcesso, "Proteger Processo");
            }, false);
            return entries;
        }

        public static ObservableCollection<ProtectionEntry> LoadProcessEntries()
        {
            var entries = new ObservableCollection<ProtectionEntry>();
            WithDriverAccess(delegate
            {
                AddLines(entries, Global.arquivoProcessos, "Processo autorizado");
            }, false);
            return entries;
        }

        public static void SaveFileEntries(IEnumerable<ProtectionEntry> entries)
        {
            WithDriverAccess(delegate
            {
                // Mantém o fluxo do WinForms: não fica recriando/verificando a pasta protegida aqui.
                // O app pede acesso ao driver primeiro e só então toca nos arquivos de configuração.
                File.WriteAllText(Global.arquivoSomenteLeitura, string.Empty);
                File.WriteAllText(Global.arquivoBloquear, string.Empty);
                File.WriteAllText(Global.arquivoOcultar, string.Empty);
                File.WriteAllText(Global.arquivoNaoExecutar, string.Empty);
                File.WriteAllText(Global.arquivoProtegerProcesso, string.Empty);

                foreach (var entry in entries.Where(x => !string.IsNullOrWhiteSpace(x.Path)))
                {
                    var target = GetFileForProtection(entry.Protection);
                    if (!string.IsNullOrWhiteSpace(target))
                        File.AppendAllText(target, entry.Path + "\r\n", Encode);
                }
            }, true);
        }

        public static void SaveProcessEntries(IEnumerable<ProtectionEntry> entries)
        {
            WithDriverAccess(delegate
            {
                // Igual ao WinForms de processos: cria a pasta se for necessário, depois salva.
                Directory.CreateDirectory(Global.pasta);
                File.WriteAllText(Global.arquivoProcessos, string.Empty);

                foreach (var entry in entries.Where(x => !string.IsNullOrWhiteSpace(x.Path)))
                    File.AppendAllText(Global.arquivoProcessos, entry.Path + "\r\n", Encode);
            }, true);
        }

        public static bool ReadFlag(string filePath)
        {
            try
            {
                bool exists = false;
                WithDriverAccess(delegate { exists = File.Exists(filePath); }, false);
                return exists;
            }
            catch { return false; }
        }

        public static void WriteFlag(string filePath, bool enabled)
        {
            WithDriverAccess(delegate
            {
                // Não chama Directory.Exists/CreateDirectory aqui.
                // Se a pasta estiver protegida pelo driver, essa checagem extra pode ser justamente o que dispara o bloqueio.
                if (enabled)
                {
                    File.WriteAllText(filePath, string.Empty);
                }
                else
                {
                    File.Delete(filePath);
                }
            }, true);
        }

        public static string GetLastCustomDialogFolder()
        {
            try
            {
                string value = null;
                WithDriverAccess(delegate
                {
                    if (File.Exists(Global.ultimoLocalCustomFileDialog))
                        value = File.ReadAllText(Global.ultimoLocalCustomFileDialog, Encode);
                }, false);

                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
            catch { }

            return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        }

        public static void SaveLastCustomDialogFolder(string path)
        {
            try
            {
                WithDriverAccess(delegate
                {
                    File.WriteAllText(Global.ultimoLocalCustomFileDialog, path ?? string.Empty, Encode);
                }, false);
            }
            catch { }
        }

        public static string GetLastProtectionOption()
        {
            try
            {
                string value = null;
                WithDriverAccess(delegate
                {
                    if (File.Exists(Global.ultimaOpcaoProtecao))
                        value = File.ReadAllText(Global.ultimaOpcaoProtecao, Encode);
                }, false);

                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
            catch { }

            return "Somente Leitura";
        }

        public static void SaveLastProtectionOption(string protection)
        {
            try
            {
                WithDriverAccess(delegate
                {
                    File.WriteAllText(Global.ultimaOpcaoProtecao, protection ?? string.Empty, Encode);
                }, false);
            }
            catch { }
        }

        public static void RemoveAllComponentsAsAdmin()
        {
            var exe = Process.GetCurrentProcess().MainModule.FileName;
            var info = new ProcessStartInfo(exe)
            {
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Normal,
                Arguments = "RemoveAllComponents",
                CreateNoWindow = false
            };

            Process.Start(info)?.WaitForExit();
        }

        private static void AddLines(Collection<ProtectionEntry> entries, string filePath, string protection)
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                foreach (var line in File.ReadAllLines(filePath, Encode))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        entries.Add(new ProtectionEntry(line, protection));
                }
            }
            catch { }
        }

        private static string GetFileForProtection(string protection)
        {
            switch (protection)
            {
                case "Somente Leitura": return Global.arquivoSomenteLeitura;
                case "Bloquear": return Global.arquivoBloquear;
                case "Ocultar": return Global.arquivoOcultar;
                case "Não Executar": return Global.arquivoNaoExecutar;
                case "Proteger Processo": return Global.arquivoProtegerProcesso;
                default: return null;
            }
        }

        public static void WithDriverAccess(Action action, bool refreshAfter)
        {
            try
            {
                Kernel.RelerTudo();
                action();
            }
            catch
            {
                // Se o driver ainda não marcou/liberou o processo no primeiro IRP, tenta de novo.
                Kernel.RelerTudo();
                Thread.Sleep(75);
                action();
            }
            finally
            {
                Kernel.RelerTudo();
                if (refreshAfter)
                    Global.AtualizarExplorer();
            }
        }
    }
}
