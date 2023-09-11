using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FilesStatistic.Model
{
    internal class TotalStatistic:INotifyPropertyChanged
    {
        private int words;
        private int lines;
        private int punctuation;
        

        public int Words
        {
            get => words;
            set
            {
                words = value;
                OnPropertyChanged();
            }
        }
        public int Lines
        {
            get => lines;
            set
            {
                lines = value;
                OnPropertyChanged();
            }
        }
        public int Punctuation
        {
            get => punctuation;
            set
            {
                punctuation = value;
                OnPropertyChanged();
            }
        }
       
        public void Clear()
        {
            Punctuation = 0;
            Lines = 0;
            Words = 0;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
