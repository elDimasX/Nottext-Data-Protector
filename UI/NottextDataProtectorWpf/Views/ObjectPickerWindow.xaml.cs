using Nottext_Data_Protector.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Nottext_Data_Protector.Views
{
    public partial class ObjectPickerWindow : Window
    {
        private readonly ObservableCollection<PickerItem> _items = new ObservableCollection<PickerItem>();
        private string _currentFolder;

        public string SelectedPath { get; private set; }

        public ObjectPickerWindow(string initialFolder)
        {
            InitializeComponent();
            WindowIconService.Apply(this);
            WindowChromeSupport.Register(this);
            ItemsList.ItemsSource = _items;

            var start = string.IsNullOrWhiteSpace(initialFolder)
                ? Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
                : initialFolder;

            if (!Directory.Exists(start))
                start = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            LoadFolder(start);
        }

        private void LoadFolder(string folder)
        {
            try
            {
                _currentFolder = folder;
                CurrentPathBox.Text = folder;
                _items.Clear();

                var directory = new DirectoryInfo(folder);

                foreach (var dir in directory.GetDirectories().OrderBy(x => x.Name))
                {
                    if (string.Equals(dir.FullName + "\\", Global.pasta, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(dir.FullName, Global.pasta.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
                        continue;

                    _items.Add(new PickerItem(dir.Name, dir.FullName, "Pasta", true));
                }

                foreach (var file in directory.GetFiles().OrderBy(x => x.Name))
                    _items.Add(new PickerItem(file.Name, file.FullName, "Arquivo", false));

                EmptyText.Visibility = _items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                SelectedPathText.Text = "Nenhum objeto selecionado.";
                SelectButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Nottext_Data_Protector.AppMessageBox.Show("Não foi possível carregar esta pasta.\r\n\r\n" + ex.Message, "Procurar arquivo ou objeto", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ItemsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ItemsList.SelectedItem is PickerItem item)
            {
                SelectedPathText.Text = item.FullPath;
                SelectButton.IsEnabled = true;
            }
            else
            {
                SelectedPathText.Text = "Nenhum objeto selecionado.";
                SelectButton.IsEnabled = false;
            }
        }

        private void ItemsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(ItemsList.SelectedItem is PickerItem item))
                return;

            if (item.IsFolder)
                LoadFolder(item.FullPath);
            else
                SelectItem(item);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Directory.GetParent(_currentFolder);
                if (parent != null)
                    LoadFolder(parent.FullName);
            }
            catch { }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            GoToTypedPath();
        }

        private void CurrentPathBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                GoToTypedPath();
        }

        private void GoToTypedPath()
        {
            var path = CurrentPathBox.Text?.Trim();
            if (Directory.Exists(path))
                LoadFolder(path);
            else if (File.Exists(path))
            {
                SelectedPath = path;
                DialogResult = true;
                Close();
            }
            else
                Nottext_Data_Protector.AppMessageBox.Show("Caminho inválido.", "Procurar arquivo ou objeto", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsList.SelectedItem is PickerItem item)
                SelectItem(item);
        }

        private void SelectItem(PickerItem item)
        {
            SelectedPath = item.FullPath;
            try { ProtectionStore.SaveLastCustomDialogFolder(item.IsFolder ? item.FullPath : Path.GetDirectoryName(item.FullPath)); } catch { }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private class PickerItem
        {
            public PickerItem(string name, string fullPath, string kind, bool isFolder)
            {
                Name = name;
                FullPath = fullPath;
                Kind = kind;
                IsFolder = isFolder;
                IconData = Geometry.Parse(isFolder
                    ? "M3,5 L9,5 L11,7 L21,7 L21,18 L3,18 Z M5,9 L5,16 L19,16 L19,9 Z"
                    : "M6,3 L15,3 L20,8 L20,21 L6,21 Z M14,4.5 L14,9 L18.5,9");
            }

            public string Name { get; }
            public string FullPath { get; }
            public string Kind { get; }
            public bool IsFolder { get; }
            public Geometry IconData { get; }
        }
    }
}
