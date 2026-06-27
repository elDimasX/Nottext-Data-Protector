using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nottext_Data_Protector
{
    internal static class WindowChromeSupport
    {
        private static readonly Geometry MaximizeGeometry = Geometry.Parse("M3,3 L21,3 L21,21 L3,21 Z");
        private static readonly Geometry RestoreGeometry = Geometry.Parse("M7,3 L21,3 L21,17 L17,17 L17,7 L7,7 Z M3,7 L17,7 L17,21 L3,21 Z");

        public static void Register(Window window)
        {
            if (window == null)
                return;

            window.StateChanged -= Window_StateChanged;
            window.StateChanged += Window_StateChanged;
            UpdateMaximizeIcon(window);
        }

        private static void Window_StateChanged(object sender, EventArgs e)
        {
            UpdateMaximizeIcon(sender as Window);
        }

        public static void Drag(Window window, MouseButtonEventArgs e)
        {
            if (window == null || e == null || e.ChangedButton != MouseButton.Left)
                return;

            try
            {
                if (e.ClickCount == 2 && window.ResizeMode == ResizeMode.CanResize)
                {
                    ToggleMaximize(window);
                    return;
                }

                window.DragMove();
            }
            catch { }
        }

        public static void Minimize(Window window)
        {
            if (window != null)
                window.WindowState = WindowState.Minimized;
        }

        public static void ToggleMaximize(Window window)
        {
            if (window == null || window.ResizeMode == ResizeMode.NoResize)
                return;

            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

            UpdateMaximizeIcon(window);
        }

        public static void Close(Window window)
        {
            if (window != null)
                window.Close();
        }

        private static void UpdateMaximizeIcon(Window window)
        {
            try
            {
                Path icon = FindChild<Path>(window, "ChromeMaximizeIcon");
                if (icon != null)
                    icon.Data = window.WindowState == WindowState.Maximized ? RestoreGeometry : MaximizeGeometry;
            }
            catch { }
        }

        private static T FindChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            if (parent == null)
                return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                T typed = child as T;
                if (typed != null && typed.Name == name)
                    return typed;

                T nested = FindChild<T>(child, name);
                if (nested != null)
                    return nested;
            }

            return null;
        }
    }
}
