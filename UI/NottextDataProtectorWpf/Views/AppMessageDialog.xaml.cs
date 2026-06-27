using System.Windows;
using System.Windows.Media;

namespace Nottext_Data_Protector.Views
{
    public partial class AppMessageDialog : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.Cancel;

        public AppMessageDialog(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            InitializeComponent();
            WindowIconService.Apply(this);
            WindowChromeSupport.Register(this);

            CaptionText.Text = string.IsNullOrWhiteSpace(caption) ? "Nottext Data Protector" : caption;
            TitleText.Text = CaptionText.Text;
            MessageText.Text = message ?? string.Empty;

            ConfigureButtons(button);
            ConfigureIcon(image);
        }

        private void ConfigureButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    CancelButton.Visibility = Visibility.Visible;
                    OkButton.Content = "OK";
                    CancelButton.Content = "Cancelar";
                    break;
                case MessageBoxButton.YesNo:
                    CancelButton.Visibility = Visibility.Visible;
                    OkButton.Content = "Sim";
                    CancelButton.Content = "Não";
                    break;
                default:
                    CancelButton.Visibility = Visibility.Collapsed;
                    OkButton.Content = "OK";
                    break;
            }
        }

        private void ConfigureIcon(MessageBoxImage image)
        {
            string iconData;
            Brush iconBrush;

            switch (image)
            {
                case MessageBoxImage.Error:
                    iconData = "M12,2 C6.48,2 2,6.48 2,12 C2,17.52 6.48,22 12,22 C17.52,22 22,17.52 22,12 C22,6.48 17.52,2 12,2 Z M8,8 L16,16 M16,8 L8,16";
                    iconBrush = (Brush)FindResource("DangerBrush");
                    break;
                case MessageBoxImage.Warning:
                    iconData = "M1,21 L12,2 L23,21 Z M11,9 L13,9 L13,14 L11,14 Z M11,16 L13,16 L13,18 L11,18 Z";
                    iconBrush = (Brush)FindResource("AccentBrush");
                    break;
                case MessageBoxImage.Question:
                    iconData = "M12,2 C6.48,2 2,6.48 2,12 C2,17.52 6.48,22 12,22 C17.52,22 22,17.52 22,12 C22,6.48 17.52,2 12,2 Z M11,17 L13,17 L13,15 L11,15 Z M12,6 C9.8,6 8.5,7.2 8.4,9.1 L10.5,9.1 C10.6,8.2 11.1,7.8 12,7.8 C12.9,7.8 13.5,8.3 13.5,9.2 C13.5,10.1 12.9,10.5 12.1,11 C10.9,11.7 10.6,12.5 10.7,14 L12.7,14 C12.7,13 13,12.6 13.9,12 C15,11.3 15.7,10.5 15.7,9.1 C15.7,7.2 14.2,6 12,6 Z";
                    iconBrush = (Brush)FindResource("AccentBrush");
                    break;
                default:
                    iconData = "M11,17 L13,17 L13,11 L11,11 Z M11,9 L13,9 L13,7 L11,7 Z M12,2 C6.48,2 2,6.48 2,12 C2,17.52 6.48,22 12,22 C17.52,22 22,17.52 22,12 C22,6.48 17.52,2 12,2 Z";
                    iconBrush = (Brush)FindResource("AccentBrush");
                    break;
            }

            IconPath.Data = Geometry.Parse(iconData);
            IconPath.Fill = iconBrush;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Result = OkButton.Content != null && OkButton.Content.ToString() == "Sim" ? MessageBoxResult.Yes : MessageBoxResult.OK;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = CancelButton.Content != null && CancelButton.Content.ToString() == "Não" ? MessageBoxResult.No : MessageBoxResult.Cancel;
            DialogResult = false;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            DialogResult = false;
            Close();
        }
    }
}
