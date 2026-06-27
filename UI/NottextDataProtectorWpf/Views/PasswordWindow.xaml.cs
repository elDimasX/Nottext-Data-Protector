using Nottext_Data_Protector.Services;
using System.Windows;
using System.Windows.Input;

namespace Nottext_Data_Protector.Views
{
    public partial class PasswordWindow : Window
    {
        private readonly bool _createMode;
        private int _tries = 4;

        public PasswordWindow(bool createMode)
        {
            InitializeComponent();
            WindowIconService.Apply(this);
            _createMode = createMode || !PasswordService.PasswordExists();

            if (_createMode)
            {
                TitleText.Text = "Definir senha";
                SubtitleText.Text = "Crie uma senha para proteger a abertura do aplicativo.";
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Confirm();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e) => Confirm();

        private void Confirm()
        {
            if (string.IsNullOrEmpty(PasswordBox.Password))
            {
                ShowError("Por favor, digite uma senha válida.");
                return;
            }

            try
            {
                if (_createMode)
                {
                    PasswordService.SavePassword(PasswordBox.Password);
                    DialogResult = true;
                    Close();
                    return;
                }

                if (PasswordService.ValidatePassword(PasswordBox.Password))
                {
                    DialogResult = true;
                    Close();
                    return;
                }

                _tries--;
                if (_tries <= 0)
                {
                    Application.Current.Shutdown();
                    return;
                }

                ShowError("Senha incorreta! Mais " + _tries + " tentativas.");
            }
            catch
            {
                Nottext_Data_Protector.AppMessageBox.Show("Ocorreu um erro ao gravar os dados do usuário.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
