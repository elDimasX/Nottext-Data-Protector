using System;
using System.Windows;
using Nottext_Data_Protector.Services;
using Nottext_Data_Protector.Views;

namespace Nottext_Data_Protector
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Enquanto a senha estiver aberta, ela é a única janela do processo.
                // O modo padrão do WPF encerra o app quando a última janela fecha;
                // então, após a senha correta, o app morria antes de abrir a MainWindow.
                ShutdownMode = ShutdownMode.OnExplicitShutdown;

                // IMPORTANTE: argumentos administrativos precisam ser tratados antes de qualquer
                // verificação/leitura que dependa do driver já instalado.
                // Se o app elevado abrir com InstallAllComponents, o driver ainda pode não existir.
                if (StartupController.HandleCommandLine(e.Args))
                {
                    Shutdown();
                    return;
                }

                // Reforço: a primeira chamada real agora acontece no Program.Main(), antes do WPF iniciar.
                // Esta segunda chamada só roda no fluxo normal do app, não no instalador elevado.
                Kernel.RelerTudo();

                StartupController.EnsureReady();

                ThemeService.ApplySavedTheme();

                // Reforça a liberação após garantir serviço/componentes.
                Kernel.RelerTudo();

                if (PasswordService.PasswordExists())
                {
                    var password = new PasswordWindow(false);
                    if (password.ShowDialog() != true)
                    {
                        Shutdown();
                        return;
                    }
                }

                // E mais uma vez antes da UI começar a carregar listas/configurações.
                Kernel.RelerTudo();

                var main = new MainWindow();
                MainWindow = main;
                main.Show();

                // A partir daqui, a janela principal volta a controlar o ciclo de vida normal.
                ShutdownMode = ShutdownMode.OnMainWindowClose;

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                Nottext_Data_Protector.AppMessageBox.Show(ex.ToString(), "Erro ao iniciar", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
