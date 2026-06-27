using Nottext_Data_Protector.Services;
using Nottext_Data_Protector.Views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Nottext_Data_Protector
{
    public partial class MainWindow : Window
    {
        private bool _sidebarCompact;

        public MainWindow()
        {
            InitializeComponent();
            WindowIconService.Apply(this);
            WindowChromeSupport.Register(this);
            UpdateFooterStatus();

            if (!OnboardingService.HasCompletedOnboarding())
                ShowOnboarding();
            else if (!StartupController.IsDriverInstalled())
                ShowDashboard();
            else
                ShowObjects();
        }

        public void ReloadAfterDriverInstall()
        {
            RefreshApplicationState();
        }

        public void RefreshApplicationState()
        {
            try
            {
                Kernel.RelerTudo();
            }
            catch { }

            UpdateFooterStatus();

            if (StartupController.IsDriverInstalled())
                ShowObjects();
            else
                ShowDashboard();
        }

        public void ShowToast(string title, string message, ToastKind kind)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ShowToast(title, message, kind));
                return;
            }

            var root = new Border
            {
                Background = (Brush)FindResource("SurfaceBrush"),
                BorderBrush = GetToastBorder(kind),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(16),
                Padding = new Thickness(14),
                Margin = new Thickness(0, 0, 0, 10),
                Opacity = 0,
                RenderTransform = new TranslateTransform(24, 0)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var icon = new Border
            {
                Width = 34,
                Height = 34,
                CornerRadius = new CornerRadius(12),
                Background = GetToastSoftBrush(kind),
                Margin = new Thickness(0, 0, 12, 0),
                Child = new TextBlock
                {
                    Text = GetToastGlyph(kind),
                    Foreground = GetToastBorder(kind),
                    FontWeight = FontWeights.Bold,
                    FontSize = 18,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            var texts = new StackPanel();
            texts.Children.Add(new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(title) ? "Nottext Data Protector" : title,
                Foreground = (Brush)FindResource("TextPrimaryBrush"),
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                TextTrimming = TextTrimming.CharacterEllipsis
            });
            texts.Children.Add(new TextBlock
            {
                Text = message ?? string.Empty,
                Foreground = (Brush)FindResource("TextSecondaryBrush"),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 3, 0, 0)
            });

            Grid.SetColumn(icon, 0);
            Grid.SetColumn(texts, 1);
            grid.Children.Add(icon);
            grid.Children.Add(texts);
            root.Child = grid;
            ToastHost.Children.Insert(0, root);

            var slide = root.RenderTransform as TranslateTransform;
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            root.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(180)) { EasingFunction = ease });
            slide.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(24, 0, TimeSpan.FromMilliseconds(220)) { EasingFunction = ease });

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3.4) };
            timer.Tick += delegate
            {
                timer.Stop();
                var fade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(180));
                fade.Completed += delegate { ToastHost.Children.Remove(root); };
                root.BeginAnimation(OpacityProperty, fade);
            };
            timer.Start();
        }

        private Brush GetToastBorder(ToastKind kind)
        {
            switch (kind)
            {
                case ToastKind.Success: return (Brush)FindResource("SuccessBrush");
                case ToastKind.Warning: return (Brush)FindResource("AccentBrush");
                case ToastKind.Error: return (Brush)FindResource("DangerBrush");
                default: return (Brush)FindResource("AccentBrush");
            }
        }

        private Brush GetToastSoftBrush(ToastKind kind)
        {
            switch (kind)
            {
                case ToastKind.Error: return (Brush)FindResource("DangerSoftBrush");
                default: return (Brush)FindResource("AccentSoftBrush");
            }
        }

        private string GetToastGlyph(ToastKind kind)
        {
            switch (kind)
            {
                case ToastKind.Success: return "✓";
                case ToastKind.Warning: return "!";
                case ToastKind.Error: return "×";
                default: return "i";
            }
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e) => ShowDashboard();
        private void ObjectsButton_Click(object sender, RoutedEventArgs e) => ShowObjects();
        private void ProcessesButton_Click(object sender, RoutedEventArgs e) => ShowProcesses();
        private void SettingsButton_Click(object sender, RoutedEventArgs e) => ShowSettings();
        private void AboutButton_Click(object sender, RoutedEventArgs e) => ShowAbout();

        private void ShowOnboarding()
        {
            SetOnboardingMode(true);
            OnboardingFrame.Content = new OnboardingPage();
        }

        private void SetOnboardingMode(bool enabled)
        {
            if (AppShell != null)
                AppShell.Visibility = enabled ? Visibility.Collapsed : Visibility.Visible;

            if (OnboardingShell != null)
                OnboardingShell.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
        }

        public void CompleteOnboarding()
        {
            OnboardingService.MarkCompleted();

            if (!StartupController.IsDriverInstalled())
                ShowDashboard();
            else
                ShowObjects();

            AppToast.Show("Pronto para começar.", "Bem-vindo", ToastKind.Success);
        }

        private void ShowDashboard()
        {
            SetOnboardingMode(false);
            PageTitle.Text = "Início";
            PageSubtitle.Text = StartupController.IsDriverInstalled()
                ? "Resumo rápido do estado do aplicativo e da proteção."
                : "Primeira instalação: instale o driver para liberar a proteção.";
            ContentFrame.Content = new DashboardPage();
            MarkActive(DashboardButton);
            UpdateFooterStatus();
        }

        private void ShowObjects()
        {
            SetOnboardingMode(false);
            if (!StartupController.IsDriverInstalled())
            {
                ShowDashboard();
                AppToast.Show("Instale o driver antes de gerenciar objetos protegidos.", "Primeira instalação", ToastKind.Warning);
                return;
            }

            PageTitle.Text = "Objetos protegidos";
            PageSubtitle.Text = "Arraste arquivos para cá ou adicione manualmente o que deve ser protegido.";
            ContentFrame.Content = new ObjectsPage();
            MarkActive(ObjectsButton);
            UpdateFooterStatus();
        }

        private void ShowProcesses()
        {
            SetOnboardingMode(false);
            if (!StartupController.IsDriverInstalled())
            {
                ShowDashboard();
                AppToast.Show("Instale o driver antes de gerenciar processos autorizados.", "Primeira instalação", ToastKind.Warning);
                return;
            }

            PageTitle.Text = "Processos autorizados";
            PageSubtitle.Text = "Controle quais processos terão permissão dentro das regras do driver.";
            ContentFrame.Content = new ProcessesPage();
            MarkActive(ProcessesButton);
            UpdateFooterStatus();
        }

        private void ShowSettings()
        {
            SetOnboardingMode(false);
            PageTitle.Text = "Configurações";
            PageSubtitle.Text = "Ative proteção global, senha, mensagens, tema e backup das configurações.";
            ContentFrame.Content = new SettingsPage();
            MarkActive(SettingsButton);
            UpdateFooterStatus();
        }

        private void ShowAbout()
        {
            SetOnboardingMode(false);
            PageTitle.Text = "Sobre";
            PageSubtitle.Text = "Informações do aplicativo e resumo das proteções disponíveis.";
            ContentFrame.Content = new AboutPage();
            MarkActive(AboutButton);
            UpdateFooterStatus();
        }

        private void MarkActive(Button active)
        {
            Button[] buttons = { DashboardButton, ObjectsButton, ProcessesButton, SettingsButton, AboutButton };
            foreach (var button in buttons)
            {
                button.Background = Brushes.Transparent;
                button.Foreground = (Brush)FindResource("TextSecondaryBrush");
            }

            active.Background = (Brush)FindResource("AccentSoftBrush");
            active.Foreground = (Brush)FindResource("TextPrimaryBrush");
        }

        public void RefreshFooterStatus()
        {
            UpdateFooterStatus();
        }

        private void UpdateFooterStatus()
        {
            bool installed = StartupController.IsDriverInstalled();
            bool running = StartupController.IsDriverRunning();
            StatusText.Text = installed ? (running ? "Driver instalado/rodando" : "Driver instalado/não rodando") : "Driver não instalado";
            DriverDot.Fill = installed ? (Brush)FindResource(running ? "SuccessBrush" : "AccentBrush") : (Brush)FindResource("DangerBrush");

            bool protectionOn = false;
            if (installed)
                protectionOn = !ProtectionStore.ReadFlag(Global.protecaoHabilitada);

            ProtectionStatusText.Text = protectionOn ? "Proteção ligada" : "Proteção desligada";
            ProtectionDot.Fill = (Brush)FindResource(protectionOn ? "SuccessBrush" : "DangerBrush");

            ApplySidebarCompactState();
        }

        private void SidebarCompactButton_Click(object sender, RoutedEventArgs e)
        {
            _sidebarCompact = !_sidebarCompact;
            ApplySidebarCompactState();
        }

        private void ApplySidebarCompactState()
        {
            if (SidebarColumn == null)
                return;

            SidebarColumn.Width = new GridLength(_sidebarCompact ? 150 : 280);
            SidebarShell.Padding = _sidebarCompact ? new Thickness(12) : new Thickness(18);
            SidebarCompactButton.ToolTip = _sidebarCompact ? "Expandir sidebar" : "Recolher sidebar";
            BrandTextPanel.Visibility = _sidebarCompact ? Visibility.Collapsed : Visibility.Visible;
            BrandCard.Width = _sidebarCompact ? 54 : double.NaN;
            BrandCard.Height = _sidebarCompact ? 54 : 76;
            BrandCard.Padding = _sidebarCompact ? new Thickness(5) : new Thickness(16);
            if (BrandIconBox != null)
            {
                BrandIconBox.Width = _sidebarCompact ? 40 : 44;
                BrandIconBox.Height = _sidebarCompact ? 40 : 44;
                BrandIconBox.CornerRadius = new CornerRadius(_sidebarCompact ? 13 : 14);
            }
            if (BrandIconText != null)
                BrandIconText.FontSize = _sidebarCompact ? 18 : 20;
            BrandCard.HorizontalAlignment = _sidebarCompact ? HorizontalAlignment.Left : HorizontalAlignment.Stretch;

            DashboardLabel.Visibility = _sidebarCompact ? Visibility.Collapsed : Visibility.Visible;
            ObjectsLabel.Visibility = _sidebarCompact ? Visibility.Collapsed : Visibility.Visible;
            ProcessesLabel.Visibility = _sidebarCompact ? Visibility.Collapsed : Visibility.Visible;
            SettingsLabel.Visibility = _sidebarCompact ? Visibility.Collapsed : Visibility.Visible;
            AboutLabel.Visibility = _sidebarCompact ? Visibility.Collapsed : Visibility.Visible;

            // O footer continua mostrando texto mesmo em modo compacto.
            // Antes ele escondia os TextBlocks e sobravam apenas bolinhas, parecendo painel vazio.
            StatusText.Visibility = Visibility.Visible;
            ProtectionStatusText.Visibility = Visibility.Visible;

            FooterStatusCard.Padding = _sidebarCompact ? new Thickness(9, 8, 9, 8) : new Thickness(10, 8, 10, 8);
            StatusText.FontSize = _sidebarCompact ? 11 : 13;
            ProtectionStatusText.FontSize = _sidebarCompact ? 11 : 13;
        }
    }
}
