using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nottext_Data_Protector.Models
{
    public class ProtectionEntry : INotifyPropertyChanged
    {
        private string _path;
        private string _protection;

        public string Path
        {
            get { return _path; }
            set { _path = value; OnPropertyChanged(); }
        }

        public string Protection
        {
            get { return _protection; }
            set { _protection = value; OnPropertyChanged(); }
        }

        public ProtectionEntry()
        {
        }

        public ProtectionEntry(string path, string protection)
        {
            Path = path;
            Protection = protection;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
