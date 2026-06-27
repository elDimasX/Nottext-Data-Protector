using System.Windows;
using System.Windows.Controls;
using Nottext_Data_Protector.Services;

namespace Nottext_Data_Protector.Views
{
    public partial class ProtectionDialog : Window
    {
        public string SelectedPath => PathTextBox.Text;
        public string SelectedProtection { get; private set; }

        public ProtectionDialog(string path, string protection)
        {
            InitializeComponent();
            WindowIconService.Apply(this);
            PathTextBox.Text = path ?? string.Empty;
            SelectProtection(string.IsNullOrWhiteSpace(protection) ? ProtectionStore.GetLastProtectionOption() : protection);
            UpdateDescription();
        }

        private void ProtectionOption_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio)
            {
                SelectedProtection = (radio.Tag ?? radio.Content)?.ToString();
                UpdateDescription();
            }
        }

        private void SelectProtection(string protection)
        {
            switch (protection)
            {
                case "Bloquear":
                    BlockRadio.IsChecked = true;
                    break;
                case "Ocultar":
                    HideRadio.IsChecked = true;
                    break;
                case "Não Executar":
                    NoExecuteRadio.IsChecked = true;
                    break;
                case "Proteger Processo":
                    ProtectProcessRadio.IsChecked = true;
                    break;
                default:
                    ReadOnlyRadio.IsChecked = true;
                    break;
            }
        }

        private void UpdateDescription()
        {
            switch (SelectedProtection)
            {
                case "Somente Leitura":
                    DescriptionText.Text = "Permite leitura, mas sinaliza restrição para alterações.";
                    break;
                case "Bloquear":
                    DescriptionText.Text = "Bloqueia o acesso ao objeto conforme a regra consumida pelo driver.";
                    break;
                case "Ocultar":
                    DescriptionText.Text = "Marca o objeto para ocultação e atualiza o Explorer após salvar.";
                    break;
                case "Não Executar":
                    DescriptionText.Text = "Impede execução do arquivo configurado, mantendo o comportamento original.";
                    break;
                case "Proteger Processo":
                    DescriptionText.Text = "Protege processo contra modificação/finalização quando suportado pelo driver.";
                    break;
                default:
                    DescriptionText.Text = "Selecione uma proteção.";
                    break;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PathTextBox.Text))
            {
                Nottext_Data_Protector.AppMessageBox.Show("Informe um caminho válido.", "Proteção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ProtectionStore.SaveLastProtectionOption(SelectedProtection);
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
