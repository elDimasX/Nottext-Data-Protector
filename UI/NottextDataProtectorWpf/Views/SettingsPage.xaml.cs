using Microsoft.Win32;
using Nottext_Data_Protector.Services;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Nottext_Data_Protector.Views
{
    public partial class SettingsPage : Page
    {
        private bool _loading;

        public SettingsPage()
        {
            InitializeComponent();
            LoadState();
        }

        private void LoadState()
        {
            _loading = true;
            GlobalProtectionCheck.IsChecked = !ProtectionStore.ReadFlag(Global.protecaoHabilitada);
            TerminateProcessesCheck.IsChecked = ProtectionStore.ReadFlag(Global.terminarProcessos);
            MessageBoxCheck.IsChecked = ProtectionStore.ReadFlag(Global.messageBox);
            PasswordCheck.IsChecked = PasswordService.PasswordExists();
            RefreshThemeButtons();
            RefreshAdministrativeActions();
            _loading = false;
        }


        private void RefreshAdministrativeActions()
        {
            try
            {
                RemoveComponentsCard.Visibility = StartupController.IsDriverInstalled()
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            catch
            {
                RemoveComponentsCard.Visibility = Visibility.Collapsed;
            }
        }

        private void GlobalProtectionCheck_Click(object sender, RoutedEventArgs e)
        {
            ToggleInvertedFlag(GlobalProtectionCheck, Global.protecaoHabilitada);
        }

        private void TerminateProcessesCheck_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlag(TerminateProcessesCheck, Global.terminarProcessos);
        }

        private void MessageBoxCheck_Click(object sender, RoutedEventArgs e)
        {
            ToggleFlag(MessageBoxCheck, Global.messageBox);
        }

        private void PasswordCheck_Click(object sender, RoutedEventArgs e)
        {
            if (_loading)
                return;

            try
            {
                if (PasswordCheck.IsChecked == true)
                {
                    var win = new PasswordWindow(true);
                    if (win.ShowDialog() == true)
                        PasswordCheck.IsChecked = true;
                    else
                        PasswordCheck.IsChecked = false;
                }
                else
                {
                    PasswordService.RemovePassword();
                    PasswordCheck.IsChecked = false;
                }
            }
            catch
            {
                AppToast.Show("Não foi possível alterar a senha por um erro inesperado.", "Senha", ToastKind.Error);
                LoadState();
            }
        }



        private void DarkThemeButton_Click(object sender, RoutedEventArgs e)
        {
            SetTheme(ThemeService.Dark);
        }

        private void WhiteThemeButton_Click(object sender, RoutedEventArgs e)
        {
            SetTheme(ThemeService.White);
        }

        private void SetTheme(string theme)
        {
            if (_loading)
                return;

            try
            {
                ThemeService.SaveAndApplyTheme(theme);
                RefreshThemeButtons();
            }
            catch
            {
                AppToast.Show("Não foi possível salvar o tema do aplicativo.", "Tema", ToastKind.Error);
                RefreshThemeButtons();
            }
        }

        private void RefreshThemeButtons()
        {
            bool white = string.Equals(ThemeService.CurrentTheme, ThemeService.White, StringComparison.OrdinalIgnoreCase);

            DarkThemeButton.Style = (Style)FindResource(white ? "GhostButton" : "PrimaryButton");
            WhiteThemeButton.Style = (Style)FindResource(white ? "PrimaryButton" : "GhostButton");
        }

        private void ExportConfigButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConfigBackupService.ExportWithDialog())
                    AppToast.Show("Backup exportado com sucesso.", "Configurações", ToastKind.Success);
            }
            catch (Exception ex)
            {
                AppToast.Show(ex.Message, "Erro ao exportar", ToastKind.Error);
            }
        }

        private void ImportConfigButton_Click(object sender, RoutedEventArgs e)
        {
            if (Nottext_Data_Protector.AppMessageBox.Show("Importar um backup vai substituir as configurações atuais. Continuar?", "Importar configurações", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                return;

            try
            {
                if (ConfigBackupService.ImportWithDialog())
                {
                    AppToast.Show("Configurações importadas com sucesso.", "Configurações", ToastKind.Success);
                    LoadState();
                    RefreshMainFooter();
                }
            }
            catch (Exception ex)
            {
                AppToast.Show(ex.Message, "Erro ao importar", ToastKind.Error);
            }
        }

        private void RemoveAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (Nottext_Data_Protector.AppMessageBox.Show("Tem certeza? Toda a proteção será desabilitada.", "Remover tudo", MessageBoxButton.OKCancel, MessageBoxImage.Question) != MessageBoxResult.OK)
                return;

            try
            {
                ProtectionStore.RemoveAllComponentsAsAdmin();
                Kernel.RelerTudo();

                RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WlfS", false);
                if (key != null)
                {
                    AppToast.Show("Não foi possível remover os arquivos do Nottext Data Protector.", "Erro", ToastKind.Error);
                    return;
                }

                AppToast.Show("Sucesso! Reinicie o computador para completar a remoção.", "Remover tudo", ToastKind.Success);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                AppToast.Show(ex.Message, "Erro", ToastKind.Error);
            }
        }

        private void ToggleFlag(CheckBox checkBox, string filePath)
        {
            if (_loading)
                return;

            try
            {
                ProtectionStore.WriteFlag(filePath, checkBox.IsChecked == true);
                RefreshMainFooter();
            }
            catch
            {
                checkBox.IsChecked = !(checkBox.IsChecked == true);
                AppToast.Show("Ocorreu um erro ao verificar os dados do usuário.", "Erro", ToastKind.Error);
            }
        }

        private void ToggleInvertedFlag(CheckBox checkBox, string filePath)
        {
            if (_loading)
                return;

            try
            {
                // Mantém exatamente a lógica da versão WinForms:
                // marcado = proteção global ligada = o arquivo marcador NÃO existe.
                ProtectionStore.WriteFlag(filePath, !(checkBox.IsChecked == true));
                RefreshMainFooter();
            }
            catch
            {
                checkBox.IsChecked = !(checkBox.IsChecked == true);
                AppToast.Show("Ocorreu um erro ao verificar os dados do usuário.", "Erro", ToastKind.Error);
            }
        }
        private void RefreshMainFooter()
        {
            try
            {
                if (Window.GetWindow(this) is MainWindow mainWindow)
                    mainWindow.RefreshFooterStatus();
            }
            catch { }
        }
    }
}
