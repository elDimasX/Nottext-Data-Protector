using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Windows;

namespace Nottext_Data_Protector.Services
{
    public static class StartupController
    {
        public static bool HandleCommandLine(string[] args)
        {
            args = args ?? new string[0];

            if (args.Contains("InstallAllComponents"))
            {
                try
                {
                    WriteInstallLog("InstallAllComponents recebido no processo elevado.");
                    EnsureTestModeForInstall();
                    WriteInstallLog("Chamando SetAcess.InstalarTudo().");
                    SetAcess.InstalarTudo();
                }
                catch (Exception ex)
                {
                    WriteInstallLog("Falha fatal no InstallAllComponents: " + ex);
                    Nottext_Data_Protector.AppMessageBox.Show(ex.ToString(), "Erro ao instalar componentes", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return true;
            }

            if (args.Contains("RemoveAllComponents"))
            {
                if (Nottext_Data_Protector.AppMessageBox.Show("Deseja desativar o modo de teste do Windows também?", "Remover componentes", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    SetTestMode(false);

                try
                {
                    Registry.LocalMachine.DeleteSubKeyTree("SYSTEM\\CurrentControlSet\\Services\\WlfS", false);
                    if (Directory.Exists(Global.pasta))
                        Directory.Delete(Global.pasta, true);
                }
                catch { }

                return true;
            }

            return false;
        }


        private static void WriteInstallLog(string message)
        {
            try
            {
                string dir = @"C:\Windows\Temp\psd";
                Directory.CreateDirectory(dir);
                File.AppendAllText(Path.Combine(dir, "install.log"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + message + Environment.NewLine);
            }
            catch { }
        }

        public static bool IsDriverInstalled()
        {
            try
            {
                RegistryView view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                using (RegistryKey key = root.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WlfS", false))
                    return key != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDriverRunning()
        {
            try
            {
                using (ServiceController service = new ServiceController("WlfS"))
                    return service.Status == ServiceControllerStatus.Running;
            }
            catch
            {
                return false;
            }
        }

        public static void EnsureReady()
        {
            Kernel.RelerTudo();

            // Primeira instalação: não fecha o aplicativo e não joga modal.
            // A MainWindow vai abrir o Dashboard para instalar os componentes.
            if (!IsDriverInstalled())
                return;

            EnsureComponents();

            try
            {
                ServiceController service = new ServiceController("WlfS");
                if (service.Status != ServiceControllerStatus.Running)
                    AppToast.Show("O serviço ainda não está em execução. Reinicie o computador se necessário.", "Serviço WlfS", ToastKind.Warning);
            }
            catch
            {
                AppToast.Show("Não foi possível verificar o serviço WlfS agora.", "Serviço WlfS", ToastKind.Warning);
            }

            Kernel.RelerTudo();
        }

        private static void EnsureComponents()
        {
            try
            {
                ServiceController service = new ServiceController("WlfS");
                if (service.Status == ServiceControllerStatus.Stopped)
                    service.Start();
            }
            catch
            {
                InstallAllComponentsElevated();
                return;
            }

            try
            {
                // Igual ao WinForms: só valida o caminho. Não força CreateDirectory aqui,
                // porque a pasta pode estar sob controle do driver.
                DirectoryInfo df = new DirectoryInfo(Global.pasta);
            }
            catch
            {
                InstallAllComponentsElevated();
            }
        }

        public static bool InstallAllComponentsElevated()
        {
            string temp = "C:\\Windows\\Temp\\psd\\";

            try
            {
                Directory.CreateDirectory(temp);

                // Mantido no mesmo espírito do WinForms original: prepara CAT/SYS/INF
                // em C:\Windows\Temp\psd e chama o MESMO executável elevado
                // com o argumento InstallAllComponents. O processo elevado executa
                // VerificarModoDeTesteEAtivar/SetAcess.InstalarTudo na sequência original.
                byte[] driver = Environment.Is64BitOperatingSystem ? Properties.Resources.WlfS_x64 : Properties.Resources.WlfS2;

                File.WriteAllBytes(Path.Combine(temp, "WlfS.cat"), Properties.Resources.wlfs);
                File.WriteAllBytes(Path.Combine(temp, "WlfS.sys"), driver);
                File.WriteAllText(Path.Combine(temp, "WlfS.inf"), Properties.Resources.WlfS1);

                string exe = Process.GetCurrentProcess().MainModule.FileName;

                Process processo = new Process();
                processo.StartInfo.FileName = exe;
                processo.StartInfo.Arguments = "InstallAllComponents";
                processo.StartInfo.Verb = "runas";
                processo.StartInfo.UseShellExecute = true;

                WriteInstallLog("Preparados: " + Path.Combine(temp, "WlfS.sys") + " (" + new FileInfo(Path.Combine(temp, "WlfS.sys")).Length + " bytes), " + Path.Combine(temp, "WlfS.cat") + ", " + Path.Combine(temp, "WlfS.inf"));

                processo.Start();
                processo.WaitForExit();
                int exitCode = processo.ExitCode;
                processo.Dispose();

                bool installed = IsDriverInstalled();
                WriteInstallLog("Processo elevado finalizou. ExitCode=" + exitCode + "; IsDriverInstalled=" + installed + "; IsDriverRunning=" + IsDriverRunning());

                if (installed)
                {
                    try { Directory.Delete(temp, true); } catch { }
                }

                return installed;
            }
            catch (Win32Exception ex)
            {
                // Mantém C:\Windows\Temp\psd para diagnóstico se algo falhar.

                if (ex.NativeErrorCode == 1223)
                    AppToast.Show("A instalação foi cancelada pelo usuário.", "Instalação cancelada", ToastKind.Warning);
                else
                    AppToast.Show(ex.Message, "Erro ao instalar componentes", ToastKind.Error);

                return false;
            }
            catch (Exception ex)
            {
                // Mantém C:\Windows\Temp\psd para diagnóstico se algo falhar.
                WriteInstallLog("Erro no InstallAllComponentsElevated: " + ex);
                AppToast.Show(ex.Message, "Erro ao instalar componentes", ToastKind.Error);
                return false;
            }
        }

        private static bool EnsureTestModeForInstall()
        {
            try
            {
                if (IsTestModeEnabled())
                    return true;

                if (Nottext_Data_Protector.AppMessageBox.Show(
                    "O modo de teste não está ativado. Para continuar, clique em OK e ativaremos o modo teste. A instalação dos componentes continuará, mas talvez seja necessário reiniciar o Windows para o driver iniciar corretamente.",
                    "Modo de teste",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    SetTestMode(true);

                    Nottext_Data_Protector.AppMessageBox.Show(
                        "Modo de teste ativado. A instalação dos componentes vai continuar. Se o driver ficar instalado, mas não rodando, reinicie o Windows para concluir.",
                        "Modo de teste",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return true;
                }

                Nottext_Data_Protector.AppMessageBox.Show(
                    "O modo de teste não foi ativado. A instalação dos componentes continuará mesmo assim, mas o driver pode ficar instalado e não rodando até o modo teste ser ativado.",
                    "Modo de teste",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return true;
            }
            catch
            {
                Nottext_Data_Protector.AppMessageBox.Show(
                    "Não foi possível verificar ou ativar o modo de teste. A instalação dos componentes continuará mesmo assim.",
                    "Modo de teste",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return true;
            }
        }

        private static bool IsTestModeEnabled()
        {
            try
            {
                RegistryView view = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                RegistryKey root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                string element = root.OpenSubKey("BCD00000000\\Objects\\{9dea862c-5cdd-4e70-acc1-f32b344d4795}\\Elements\\23000003", false)?.GetValue("Element")?.ToString();
                if (string.IsNullOrWhiteSpace(element))
                    return false;

                RegistryKey key = root.OpenSubKey("BCD00000000\\Objects\\" + element + "\\Elements\\16000049", false);
                byte[] value = key?.GetValue("Element") as byte[];
                return value != null && BitConverter.ToString(value) == "01";
            }
            catch
            {
                return false;
            }
        }

        private static void SetTestMode(bool enabled)
        {
            string value = enabled ? "on" : "off";
            SetAcess.IniciarProcesso("cmd.exe", "/C bcdedit -set testsigning " + value);
            SetAcess.IniciarProcesso("bcdedit.exe", "-set testsigning " + value);
        }
    }
}
