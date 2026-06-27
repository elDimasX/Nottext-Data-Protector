using Nottext_Data_Protector.Models;
using Nottext_Data_Protector.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Nottext_Data_Protector.Views
{
    public partial class ObjectsPage : Page
    {
        public ObservableCollection<ProtectionEntry> Entries { get; private set; }
        private ObservableCollection<ProtectionEntry> _lastSavedEntries;

        public ObjectsPage()
        {
            InitializeComponent();
            ReloadFromDisk();
        }

        private void ReloadFromDisk()
        {
            _lastSavedEntries = CloneEntries(ProtectionStore.LoadFileEntries());
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
            var dialog = new ObjectPickerWindow(ProtectionStore.GetLastCustomDialogFolder())
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                AddOrEdit(dialog.SelectedPath, null);
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

        private void EntriesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EntriesList.SelectedItem is ProtectionEntry entry)
                AddOrEdit(entry.Path, entry);
        }

        private void EntriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEmptyState();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProtectionStore.SaveFileEntries(Entries);
                _lastSavedEntries = CloneEntries(Entries);
                SetDirty(false);
                Nottext_Data_Protector.AppMessageBox.Show("Objetos salvos com sucesso.", "Objetos protegidos", MessageBoxButton.OK, MessageBoxImage.Information);
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

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
                AddOrEdit(file, null);
        }

        private void AddOrEdit(string path, ProtectionEntry existing)
        {
            var dialog = new ProtectionDialog(path, existing?.Protection ?? ProtectionStore.GetLastProtectionOption())
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            if (existing == null)
                Entries.Add(new ProtectionEntry(dialog.SelectedPath, dialog.SelectedProtection));
            else
            {
                existing.Path = dialog.SelectedPath;
                existing.Protection = dialog.SelectedProtection;
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
