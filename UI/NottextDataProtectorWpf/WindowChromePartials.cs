using System.Windows;
using System.Windows.Input;
using Nottext_Data_Protector.Views;

namespace Nottext_Data_Protector
{
    public partial class MainWindow
    {
        private void ChromeTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowChromeSupport.Drag(this, e);
        private void ChromeMinimizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Minimize(this);
        private void ChromeMaximizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.ToggleMaximize(this);
        private void ChromeCloseButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Close(this);
    }
}

namespace Nottext_Data_Protector.Views
{
    public partial class ObjectPickerWindow
    {
        private void ChromeTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowChromeSupport.Drag(this, e);
        private void ChromeMinimizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Minimize(this);
        private void ChromeMaximizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.ToggleMaximize(this);
        private void ChromeCloseButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Close(this);
    }

    public partial class ProcessPickerWindow
    {
        private void ChromeTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowChromeSupport.Drag(this, e);
        private void ChromeMinimizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Minimize(this);
        private void ChromeMaximizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.ToggleMaximize(this);
        private void ChromeCloseButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Close(this);
    }

    public partial class PasswordWindow
    {
        private void ChromeTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowChromeSupport.Drag(this, e);
        private void ChromeMinimizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Minimize(this);
        private void ChromeMaximizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.ToggleMaximize(this);
        private void ChromeCloseButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Close(this);
    }

    public partial class ProtectionDialog
    {
        private void ChromeTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowChromeSupport.Drag(this, e);
        private void ChromeMinimizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Minimize(this);
        private void ChromeMaximizeButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.ToggleMaximize(this);
        private void ChromeCloseButton_Click(object sender, RoutedEventArgs e) => WindowChromeSupport.Close(this);
    }

    public partial class AppMessageDialog
    {
        private void ChromeTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => WindowChromeSupport.Drag(this, e);
    }
}
