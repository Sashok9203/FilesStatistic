using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FilesStatistic.Model
{
    internal class FileStatistic : INotifyPropertyChanged
    {
        private bool isAnalized;
                
        public int? Words { get; set; }
       
        public int? Lines { get; set; }

        public int? Punctuation { get; set; }

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
