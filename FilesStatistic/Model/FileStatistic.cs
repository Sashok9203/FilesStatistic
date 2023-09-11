using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FilesStatistic.Model
{
    internal class FileStatistic : INotifyPropertyChanged
    {
        private bool isAnalized;
        public int? words;
        public int? lines;
        public int? punctuation;


        public int? Words
        {
            get => words;
            set
            {
                words = value;
                OnPropertyChanged();
            }
        }

        public int? Lines
        {
            get => lines;
            set
            {
                lines = value;
                OnPropertyChanged();
            }
        }

        public int? Punctuation
        {
            get => punctuation;
            set
            {
                punctuation = value;
                OnPropertyChanged();
            }
        }

        public string? Path { get; set; }

        public bool IsAnalized
        {
            get => isAnalized;
            set 
            {
                isAnalized = value;
                OnPropertyChanged("IsAnalizedIcon");
            }
        }

        public string? IsAnalizedIcon => isAnalized ? @"Images\icons8-галочка-60.png" : null;

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
