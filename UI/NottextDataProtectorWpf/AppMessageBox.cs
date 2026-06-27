using System.Windows;
using Nottext_Data_Protector.Views;

namespace Nottext_Data_Protector
{
    internal static class AppMessageBox
    {
        public static MessageBoxResult Show(string message)
        {
            return Show(message, "Nottext Data Protector", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult Show(string message, string caption)
        {
            return Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button)
        {
            return Show(message, caption, button, MessageBoxImage.Information);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            if (button == MessageBoxButton.OK)
            {
                AppToast.Show(message, caption, ToToastKind(image));
                return MessageBoxResult.OK;
            }

            Window owner = FindOwner();
            var dialog = new AppMessageDialog(message, caption, button, image);

            if (owner != null && owner.IsVisible && !ReferenceEquals(owner, dialog))
                dialog.Owner = owner;
            else
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            dialog.ShowDialog();
            return dialog.Result;
        }

        private static ToastKind ToToastKind(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Error: return ToastKind.Error;
                case MessageBoxImage.Warning: return ToastKind.Warning;
                case MessageBoxImage.Question: return ToastKind.Warning;
                default: return ToastKind.Success;
            }
        }

        private static Window FindOwner()
        {
            if (Application.Current == null)
                return null;

            Window active = null;
            foreach (Window window in Application.Current.Windows)
            {
                if (window != null && window.IsActive)
                    active = window;
            }

            if (active != null)
                return active;

            return Application.Current.MainWindow;
        }
    }
}
