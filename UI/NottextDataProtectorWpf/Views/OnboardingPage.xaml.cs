using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nottext_Data_Protector.Views
{
    public partial class OnboardingPage : Page
    {
        private int _step;

        public OnboardingPage()
        {
            InitializeComponent();
            ApplyStep();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_step > 0)
                _step--;

            ApplyStep();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_step < 2)
            {
                _step++;
                ApplyStep();
                return;
            }

            (Application.Current.MainWindow as MainWindow)?.CompleteOnboarding();
        }

        private void ApplyStep()
        {
            WelcomeStep.Visibility = _step == 0 ? Visibility.Visible : Visibility.Collapsed;
            ProtectionsStep.Visibility = _step == 1 ? Visibility.Visible : Visibility.Collapsed;
            StartStep.Visibility = _step == 2 ? Visibility.Visible : Visibility.Collapsed;

            BackButton.Visibility = _step == 0 ? Visibility.Collapsed : Visibility.Visible;
            NextButton.Content = _step == 2 ? "Iniciar programa" : "Próximo";

            if (_step == 0)
            {
                StepTitle.Text = "Bem-vindo ao Nottext Data Protector";
                StepSubtitle.Text = "Proteja arquivos, pastas e processos importantes com uma experiência simples e direta.";
            }
            else if (_step == 1)
            {
                StepTitle.Text = "Próximos passos";
                StepSubtitle.Text = "Entenda rapidamente as proteções disponíveis antes de começar.";
            }
            else
            {
                StepTitle.Text = "Iniciar programa";
                StepSubtitle.Text = "Agora você pode abrir o painel principal e concluir a configuração do aplicativo.";
            }

            Brush active = (Brush)FindResource("AccentBrush");
            Brush inactive = (Brush)FindResource("BorderBrushSoft");
            Dot1.Fill = _step == 0 ? active : inactive;
            Dot2.Fill = _step == 1 ? active : inactive;
            Dot3.Fill = _step == 2 ? active : inactive;
        }
    }
}
