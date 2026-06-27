using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Nottext_Data_Protector
{
    internal static class WindowIconService
    {
        private static BitmapFrame _icon;

        public static void Apply(Window window)
        {
            if (window == null)
                return;

            try
            {
                if (_icon == null)
                    _icon = BitmapFrame.Create(new Uri("pack://application:,,,/NottextDataProtector.ico", UriKind.Absolute));

                window.Icon = _icon;
            }
            catch
            {
                // Ícone é apenas visual. Nunca deixe isso impedir o app de abrir.
            }
        }
    }
}
