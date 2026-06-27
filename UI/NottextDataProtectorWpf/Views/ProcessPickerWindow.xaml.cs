using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Nottext_Data_Protector.Views
{
    public partial class ProcessPickerWindow : Window
    {
        private readonly ObservableCollection<ProcessChoice> _allProcesses = new ObservableCollection<ProcessChoice>();
        public List<string> SelectedPaths { get; private set; } = new List<string>();

        public ProcessPickerWindow()
        {
            InitializeComponent();
            WindowIconService.Apply(this);
            WindowChromeSupport.Register(this);
            LoadProcessesSafe();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadProcessesSafe();
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void LoadProcessesSafe()
        {
            _allProcesses.Clear();

            int skipped = 0;
            Process[] processes = new Process[0];

            try
            {
                processes = Process.GetProcesses();
            }
            catch
            {
                processes = new Process[0];
            }

            foreach (Process process in processes)
            {
                try
                {
                    string name = SafeString(delegate { return process.ProcessName; });
                    int id = SafeInt(delegate { return process.Id; });
                    string path = SafeString(delegate { return process.MainModule.FileName; });

                    if (string.IsNullOrWhiteSpace(path) || !path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        skipped++;
                        continue;
                    }

                    _allProcesses.Add(new ProcessChoice
                    {
                        Name = name,
                        Id = id,
                        Path = path
                    });
                }
                catch
                {
                    skipped++;
                }
                finally
                {
                    try { process.Dispose(); } catch { }
                }
            }

            var ordered = _allProcesses.OrderBy(x => x.Name).ThenBy(x => x.Id).ToList();
            _allProcesses.Clear();
            foreach (var item in ordered)
                _allProcesses.Add(item);

            StatusText.Text = _allProcesses.Count + " processos disponíveis" + (skipped > 0 ? " • " + skipped + " ignorados sem permissão/caminho" : "");
            ApplyFilter();
        }

        private static string SafeString(Func<string> getter)
        {
            try { return getter() ?? string.Empty; }
            catch { return string.Empty; }
        }

        private static int SafeInt(Func<int> getter)
        {
            try { return getter(); }
            catch { return 0; }
        }

        private void ApplyFilter()
        {
            string query = (SearchBox.Text ?? string.Empty).Trim().ToLowerInvariant();
            IEnumerable<ProcessChoice> items = _allProcesses;

            if (!string.IsNullOrWhiteSpace(query))
            {
                items = items.Where(x =>
                    (x.Name ?? string.Empty).ToLowerInvariant().Contains(query) ||
                    (x.Path ?? string.Empty).ToLowerInvariant().Contains(query) ||
                    x.Id.ToString().Contains(query));
            }

            var filtered = items.ToList();
            ProcessesList.ItemsSource = filtered;
            EmptyText.Visibility = filtered.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AddSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPaths = ProcessesList.SelectedItems
                .OfType<ProcessChoice>()
                .Where(x => !string.IsNullOrWhiteSpace(x.Path))
                .Select(x => x.Path)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (SelectedPaths.Count == 0)
            {
                Nottext_Data_Protector.AppMessageBox.Show("Selecione pelo menos um processo com caminho válido.", "Processos", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public class ProcessChoice
        {
            public string Name { get; set; }
            public int Id { get; set; }
            public string Path { get; set; }
            public string DisplayName { get { return string.IsNullOrWhiteSpace(Name) ? Path : Name; } }
            public string PidText { get { return "PID " + Id; } }
        }
    }
}
