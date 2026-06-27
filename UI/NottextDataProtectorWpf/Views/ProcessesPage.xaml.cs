using Nottext_Data_Protector.Models;
using Nottext_Data_Protector.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Nottext_Data_Protector.Views
{
    public partial class ProcessesPage : Page
    {
        public ObservableCollection<ProtectionEntry> Entries { get; private set; }
        private ObservableCollection<ProtectionEntry> _lastSavedEntries;

        public ProcessesPage()
        {
            InitializeComponent();
            ReloadFromDisk();
        }

        private void ReloadFromDisk()
        {
            _lastSavedEntries = CloneEntries(ProtectionStore.LoadProcessEntries());
            RestoreLastSavedSnapshot();
        }

        private void RestoreLastSavedSnapshot()
        {
            Entries = CloneEntries(_lastSavedEntries);
            DataContext = null;
            DataContext = this;
            EntriesList.ItemsSource = Entries;
            SetDirty(false);
            UpdateEmptyState();
        }

        private static ObservableCollection<ProtectionEntry> CloneEntries(ObservableCollection<ProtectionEntry> source)
        {
            var clone = new ObservableCollection<ProtectionEntry>();
            if (source == null)
                return clone;

            foreach (var item in source)
                clone.Add(new ProtectionEntry(item.Path, item.Protection));

            return clone;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProcessPickerWindow
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            foreach (string path in dialog.SelectedPaths)
            {
                if (!Entries.Any(x => string.Equals(x.Path, path, System.StringComparison.OrdinalIgnoreCase)))
                    Entries.Add(new ProtectionEntry(path, "Processo autorizado"));
            }

            if (dialog.SelectedPaths.Count > 0)
                SetDirty(true);

            UpdateEmptyState();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = EntriesList.SelectedItems.Cast<ProtectionEntry>().ToList();
            foreach (var item in selected)
                Entries.Remove(item);
            if (selected.Count > 0)
                SetDirty(true);
            UpdateEmptyState();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProtectionStore.SaveProcessEntries(Entries);
                _lastSavedEntries = CloneEntries(Entries);
                SetDirty(false);
                Nottext_Data_Protector.AppMessageBox.Show("Processos salvos com sucesso.", "Processos", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                Nottext_Data_Protector.AppMessageBox.Show("Ocorreu um erro ao salvar os dados do usuário.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RestoreLastSavedSnapshot();
        }

        private void Page_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            foreach (string file in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (!Entries.Any(x => string.Equals(x.Path, file, System.StringComparison.OrdinalIgnoreCase)))
                    Entries.Add(new ProtectionEntry(file, "Processo autorizado"));
            }

            SetDirty(true);
            UpdateEmptyState();
        }

        private void SetDirty(bool dirty)
        {
            SaveButton.IsEnabled = dirty;
            CancelButton.IsEnabled = dirty;
        }

        private void UpdateEmptyState()
        {
            EmptyText.Visibility = Entries == null || Entries.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
