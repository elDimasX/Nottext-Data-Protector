using System;
using System.Windows;
using Nottext_Data_Protector.Views;

namespace Nottext_Data_Protector
{
    public enum ToastKind
    {
        Info,
        Success,
        Warning,
        Error
    }

    internal static class AppToast
    {
        public static void Show(string message)
        {
            Show(message, "Nottext Data Protector", ToastKind.Info);
        }

        public static void Show(string message, string title)
        {
            Show(message, title, ToastKind.Info);
        }

        public static void Show(string message, string title, ToastKind kind)
        {
            try
            {
                var main = Application.Current != null ? Application.Current.MainWindow as MainWindow : null;
                if (main != null && main.IsVisible)
                {
                    main.ShowToast(title, message, kind);
                    return;
                }
            }
            catch { }

            // Fallback apenas para momentos de startup, quando o MainWindow ainda não existe.
            try
            {
                var dialog = new AppMessageDialog(message, title, MessageBoxButton.OK, ToImage(kind));
                dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dialog.ShowDialog();
            }
            catch { }
        }

        private static MessageBoxImage ToImage(ToastKind kind)
        {
            switch (kind)
            {
                case ToastKind.Success: return MessageBoxImage.Information;
                case ToastKind.Warning: return MessageBoxImage.Warning;
                case ToastKind.Error: return MessageBoxImage.Error;
                default: return MessageBoxImage.Information;
            }
        }
    }
}
