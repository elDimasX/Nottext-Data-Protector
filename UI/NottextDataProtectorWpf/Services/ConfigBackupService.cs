using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Nottext_Data_Protector.Services
{
    public static class ConfigBackupService
    {
        private static readonly Encoding Encode = Encoding.GetEncoding("iso-8859-1");

        private static readonly string[] ConfigFiles =
        {
            "pp.cat", "pp.sys", "pp.hnd", "pp.enp", "pp.prc", "pp.inf", "pp.end",
            "pp.term", "pp.msg", "pp.psh", "pp.koo", "pp.zpp", "pp.fpp", "pp.theme"
        };

        public static bool ExportWithDialog()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Exportar configurações",
                Filter = "Backup Nottext Data Protector (*.ndpbackup)|*.ndpbackup|Arquivo ZIP (*.zip)|*.zip",
                FileName = "nottext-data-protector-backup.ndpbackup",
                AddExtension = true,
                OverwritePrompt = true
            };

            if (dialog.ShowDialog() != true)
                return false;

            Export(dialog.FileName);
            return true;
        }

        public static bool ImportWithDialog()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Importar configurações",
                Filter = "Backup Nottext Data Protector (*.ndpbackup;*.zip)|*.ndpbackup;*.zip|Todos os arquivos (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
                return false;

            Import(dialog.FileName);
            return true;
        }

        public static void Export(string destinationFile)
        {
            if (string.IsNullOrWhiteSpace(destinationFile))
                throw new ArgumentException("Caminho de exportação inválido.");

            string temp = Path.Combine(Path.GetTempPath(), "ndp_export_" + Guid.NewGuid().ToString("N"));

            try
            {
                Directory.CreateDirectory(temp);

                ProtectionStore.WithDriverAccess(delegate
                {
                    foreach (string fileName in ConfigFiles)
                    {
                        try
                        {
                            string source = Path.Combine(Global.pasta, fileName);
                            if (File.Exists(source))
                                File.Copy(source, Path.Combine(temp, fileName), true);
                        }
                        catch { }
                    }

                    File.WriteAllText(Path.Combine(temp, "backup.info"), "Nottext Data Protector Backup\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Encode);
                }, false);

                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);

                ZipFile.CreateFromDirectory(temp, destinationFile, CompressionLevel.Optimal, false);
            }
            finally
            {
                try { if (Directory.Exists(temp)) Directory.Delete(temp, true); } catch { }
            }
        }

        public static void Import(string sourceFile)
        {
            if (string.IsNullOrWhiteSpace(sourceFile) || !File.Exists(sourceFile))
                throw new FileNotFoundException("Backup não encontrado.", sourceFile);

            string temp = Path.Combine(Path.GetTempPath(), "ndp_import_" + Guid.NewGuid().ToString("N"));

            try
            {
                Directory.CreateDirectory(temp);
                ZipFile.ExtractToDirectory(sourceFile, temp);

                ProtectionStore.WithDriverAccess(delegate
                {
                    Directory.CreateDirectory(Global.pasta);

                    foreach (string fileName in ConfigFiles)
                    {
                        try
                        {
                            string candidate = Directory.GetFiles(temp, fileName, SearchOption.AllDirectories).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(candidate) && File.Exists(candidate))
                                File.Copy(candidate, Path.Combine(Global.pasta, fileName), true);
                        }
                        catch { }
                    }
                }, true);

                Kernel.RelerTudo();
                ThemeService.ApplySavedTheme();
            }
            finally
            {
                try { if (Directory.Exists(temp)) Directory.Delete(temp, true); } catch { }
            }
        }
    }
}
