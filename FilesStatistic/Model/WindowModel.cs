using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.AxHost;


namespace FilesStatistic.Model
{
    internal class WindowModel :INotifyPropertyChanged
    {
        private string? dPath;
        private Thread? getTxtFiles ;
        private bool checkBoxEnabled;

        private bool isScaned,isAnalize;

        private bool IsScaned
        {
            get => isScaned;
            set
            {
                isScaned = value;
                OnPropertyChanged("ButtonName");
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private bool IsAnalize
        {
            get => isAnalize;
            set
            {
                isAnalize = value;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void getFiles(bool subfolders)
        {
            if (IsScaned)
            {
                getTxtFiles?.Interrupt();
                return;
            }
            
            if (!Directory.Exists(DirPath))
            {
                DirPath = null;
                return;
            }

            FilesStats.Clear();

            IEnumerable<string> files = Directory.EnumerateFiles(DirPath, "*.txt",
                                         new EnumerationOptions() { IgnoreInaccessible = true, RecurseSubdirectories = subfolders, MaxRecursionDepth = SubfoldersDepth });
            if (files.Any())
            {
                IsScaned = true;
                getTxtFiles = new(() =>
                {
                    foreach (string file in files)
                    {
                        try
                        {
                            System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                FilesStats.Add(new()
                                {
                                    Path = file
                                });
                            }));
                        }
                        catch  {break;}
                    }
                    System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        IsScaned = false;
                    }));

                });
                getTxtFiles.Start();
            }
        }

        private void analizeFile(object o)
        {
            FileStatistic? fStat = o as FileStatistic;
            string? file =null;
            try { file = File.ReadAllText(Path.Combine(fStat.Path)); } catch { return; }
            int punctuations = file?.Where(x => char.IsPunctuation(x)).Count() ?? 0;
            int lines = getLinesCount(file);
            int words = getWordsCount(file);
            System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                fStat.Lines = lines;
                fStat.Punctuation = punctuations;
                fStat.IsAnalized = true;
                fStat.Words = words;
            }));
            
            System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                Monitor.Enter(TotalStat);
                try
                {
                    TotalStat.Lines += lines;
                    TotalStat.Punctuation += punctuations;
                    TotalStat.Words += words;
                }
                finally { Monitor.Exit(TotalStat); }

            }));
        }

        private int getLinesCount(string? text)
        {
            int linesCount = 0;
            if (text != null && text != "")
            {
                linesCount = text.Where(x => x == '\n').Count() ;
                if (text.Last() != '\n') linesCount++;
            }
            return linesCount;
        }

        private int getWordsCount(string? text)
        {
            int wordsCount = 0;
            if (text != null && text != "")
            {
                bool wordStart = false;
                foreach (var chr in text)
                {
                    if (Char.IsLetter(chr) || Char.IsDigit(chr)) wordStart = true;
                    else if (wordStart)
                    {
                        wordStart = false;
                        wordsCount++;
                    }
                }
                if (wordStart) wordsCount++;
            }
            return wordsCount;
        }

        private void analize()
        {
            TotalStat.Clear();
            isAnalize = true;
            foreach (var item in FilesStats)
                ThreadPool.QueueUserWorkItem(analizeFile, item);
            Thread checker = new(() =>
            {
                while (isAnalize)
                {
                    Thread.Sleep(300);
                    if (ThreadPool.PendingWorkItemCount == 0)
                        System.Windows.Application.Current?.Dispatcher.Invoke(new Action(() =>
                        {
                            IsAnalize = false;
                        }));
                }
            });
            checker.Start();
        }

        private void openFolder()
        {
            FolderBrowserDialog fbdialog = new();
            if (fbdialog.ShowDialog() == DialogResult.OK)
            {
                DirPath = fbdialog.SelectedPath;
            }
        }

        public bool Subfolders { get; set; }

        public int SubfoldersDepth { get; set; } = 2;

        public bool CheckBoxEnabled
        {
            get => checkBoxEnabled;
            set
            {
                checkBoxEnabled = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FileStatistic> FilesStats { get; set; }

        public TotalStatistic TotalStat {get;set;}

        public string ButtonName => IsScaned ? "Cancel" : "Scan";

        public string? DirPath
        { 
            get => dPath;
            set
            {
                dPath = value;
                CheckBoxEnabled = !string.IsNullOrEmpty(dPath);
                OnPropertyChanged();
            }
        }

        public WindowModel()
        {
            FilesStats = new();
            TotalStat = new();
        }



        public RelayCommand OpenFolder => new((o)=>openFolder());

        public RelayCommand Scan => new((o) => getFiles(Subfolders),(o)=>!string.IsNullOrEmpty(DirPath) && !IsAnalize);

        public RelayCommand Analize => new((o) => analize() ,(o)=> FilesStats.Count > 0 && !IsScaned && !IsAnalize);

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
