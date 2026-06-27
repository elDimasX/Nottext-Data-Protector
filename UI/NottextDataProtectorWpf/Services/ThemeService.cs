using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Nottext_Data_Protector.Services
{
    public static class ThemeService
    {
        private static readonly Encoding Encode = Encoding.GetEncoding("iso-8859-1");
        public const string Dark = "Dark";
        public const string White = "White";

        public static string CurrentTheme { get; private set; } = Dark;

        public static string LoadSavedTheme()
        {
            try
            {
                string value = null;
                ProtectionStore.WithDriverAccess(delegate
                {
                    if (File.Exists(Global.temaArquivo))
                        value = File.ReadAllText(Global.temaArquivo, Encode).Trim();
                }, false);

                if (string.Equals(value, White, StringComparison.OrdinalIgnoreCase))
                    return White;
            }
            catch { }

            return Dark;
        }

        public static void ApplySavedTheme()
        {
            ApplyTheme(LoadSavedTheme(), false);
        }

        public static void SaveAndApplyTheme(string theme)
        {
            ApplyTheme(theme, true);
        }

        public static void ApplyTheme(string theme, bool save)
        {
            if (!string.Equals(theme, White, StringComparison.OrdinalIgnoreCase))
                theme = Dark;

            CurrentTheme = theme;

            if (save)
            {
                ProtectionStore.WithDriverAccess(delegate
                {
                    File.WriteAllText(Global.temaArquivo, theme, Encode);
                }, true);
            }

            if (Application.Current == null)
                return;

            if (theme == White)
                ApplyWhite();
            else
                ApplyDark();

            RefreshOpenWindows();
        }

        private static void ApplyDark()
        {
            SetBrush("BgBrush", "#0F1115");
            SetBrush("SidebarBrush", "#121722");
            SetBrush("SurfaceBrush", "#171B24");
            SetBrush("SurfaceElevatedBrush", "#202633");
            SetBrush("BorderBrushSoft", "#2C3342");
            SetBrush("TextPrimaryBrush", "#F3F6FC");
            SetBrush("TextSecondaryBrush", "#AAB3C4");
            SetBrush("AccentBrush", "#00D4FF");
            SetBrush("AccentSoftBrush", "#1A00D4FF");
            SetBrush("DangerBrush", "#FF5B6E");
            SetBrush("SuccessBrush", "#40D98F");
            SetBrush("FieldBrush", "#111722");
            SetBrush("PanelBrush", "#0F141D");
            SetBrush("CardItemBrush", "#171D29");
            SetBrush("FooterBrush", "#121821");
            SetBrush("StatusBrush", "#0E141F");
            SetBrush("GhostBrush", "#202633");
            SetBrush("PrimaryButtonTextBrush", "#061017");
            SetBrush("WindowOutlineBrush", "#3300D4FF");
            SetBrush("DangerSoftBrush", "#24FF5B6E");
        }

        private static void ApplyWhite()
        {
            SetBrush("BgBrush", "#F4F7FB");
            SetBrush("SidebarBrush", "#FFFFFF");
            SetBrush("SurfaceBrush", "#FFFFFF");
            SetBrush("SurfaceElevatedBrush", "#F0F4FA");
            SetBrush("BorderBrushSoft", "#D8E0EC");
            SetBrush("TextPrimaryBrush", "#111827");
            SetBrush("TextSecondaryBrush", "#5D6678");
            SetBrush("AccentBrush", "#0078D4");
            SetBrush("AccentSoftBrush", "#1A0078D4");
            SetBrush("DangerBrush", "#D92D44");
            SetBrush("SuccessBrush", "#168A52");
            SetBrush("FieldBrush", "#FFFFFF");
            SetBrush("PanelBrush", "#F8FAFD");
            SetBrush("CardItemBrush", "#FFFFFF");
            SetBrush("FooterBrush", "#FFFFFF");
            SetBrush("StatusBrush", "#EDF3FA");
            SetBrush("GhostBrush", "#E9EEF6");
            SetBrush("PrimaryButtonTextBrush", "#FFFFFF");
            SetBrush("WindowOutlineBrush", "#550078D4");
            SetBrush("DangerSoftBrush", "#18D92D44");
        }

        private static void SetBrush(string key, string hex)
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(hex);
                var brush = Application.Current.Resources[key] as SolidColorBrush;

                if (brush != null && !brush.IsFrozen)
                {
                    brush.Color = color;
                    return;
                }

                Application.Current.Resources[key] = new SolidColorBrush(color);
            }
            catch { }
        }

        private static void RefreshOpenWindows()
        {
            try
            {
                foreach (Window window in Application.Current.Windows)
                {
                    window.Background = (Brush)Application.Current.Resources["BgBrush"];
                    window.Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"];
                    window.InvalidateVisual();
                }
            }
            catch { }
        }
    }
}
