using Nottext_Data_Protector.Services;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Nottext_Data_Protector.Views
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            bool installed = StartupController.IsDriverInstalled();
            bool running = StartupController.IsDriverRunning();
            bool protectionOn = installed && !ProtectionStore.ReadFlag(Global.protecaoHabilitada);

            DriverStatusText.Text = installed ? (running ? "Instalado/rodando" : "Instalado/não rodando") : "Não instalado";
            ProtectionSummaryText.Text = protectionOn ? "Ligada" : "Desligada";
            InstallButton.Visibility = installed && running ? Visibility.Collapsed : Visibility.Visible;

            if (!installed)
            {
                DashboardTitleText.Text = "Primeira instalação";
                DashboardSubtitleText.Text = "O driver WlfS ainda não está instalado. Instale os componentes para ativar a proteção e liberar as demais configurações.";
                NextStepLabelText.Text = "Próximo passo";
                NextStepValueText.Text = "Instalar componentes";
                DetailsTitleText.Text = "O que acontece agora?";
                DetailsBodyText.Text = "O aplicativo vai solicitar permissão de administrador, instalar os arquivos do driver e ajustar o serviço WlfS. Depois disso, talvez seja necessário reiniciar o Windows para o driver iniciar corretamente.";
                return;
            }

            if (!running)
            {
                DashboardTitleText.Text = "Driver instalado";
                DashboardSubtitleText.Text = "Os componentes já foram instalados, mas o serviço WlfS ainda não está em execução.";
                NextStepLabelText.Text = "Próximo passo";
                NextStepValueText.Text = "Verificar ou reiniciar";
                DetailsTitleText.Text = "Como concluir";
                DetailsBodyText.Text = "Clique em Verificar novamente para atualizar o estado. Se o modo de teste acabou de ser ativado, reinicie o Windows e abra o Nottext Data Protector novamente.";
                return;
            }

            DashboardTitleText.Text = "Proteção pronta";
            DashboardSubtitleText.Text = "O driver está instalado e em execução. Agora você pode escolher o que proteger e quais processos terão acesso autorizado.";
            NextStepLabelText.Text = "Comece por aqui";
            NextStepValueText.Text = "Adicionar objetos";
            DetailsTitleText.Text = "Como usar a proteção";
            DetailsBodyText.Text = "Em Objetos protegidos, adicione arquivos ou pastas e escolha o tipo de proteção. Em Processos autorizados, adicione aplicativos que podem acessar esses objetos. Em Configurações, ajuste a proteção global, tema, senha e backups.";
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try { Kernel.RelerTudo(); } catch { }

                RefreshStatus();

                if (Window.GetWindow(this) is MainWindow mainWindow)
                    mainWindow.RefreshFooterStatus();

                if (StartupController.IsDriverInstalled())
                {
                    AppToast.Show("Driver detectado. Recarregando o aplicativo.", "Driver", ToastKind.Success);
                    if (Window.GetWindow(this) is MainWindow main)
                        main.RefreshApplicationState();
                }
                else
                {
                    AppToast.Show("O driver ainda não foi detectado neste sistema.", "Driver", ToastKind.Warning);
                }
            }
            catch (Exception ex)
            {
                AppToast.Show(ex.Message, "Erro ao verificar", ToastKind.Error);
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool installed = StartupController.InstallAllComponentsElevated();
                if (installed)
                {
                    AppToast.Show("Componentes instalados. Verificando se o driver está rodando.", "Instalação", ToastKind.Success);
                    if (Window.GetWindow(this) is MainWindow main)
                        main.RefreshApplicationState();
                }
                else
                {
                    AppToast.Show("A instalação não foi concluída ou o driver ainda não apareceu no registro.", "Instalação", ToastKind.Warning);
                }

                RefreshStatus();
            }
            catch (Exception ex)
            {
                AppToast.Show(ex.Message, "Erro ao instalar", ToastKind.Error);
            }
        }
    }
}
