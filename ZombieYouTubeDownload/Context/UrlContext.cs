using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ZombieYouTubeDownload.Context
{
    internal class UrlContext : INotifyPropertyChanged
    {
        private string _url ="";
        private string _start = "";
        private string _end = "";
        private string _result = "";

        public string Url 
        { 
            get => _url;
            set 
            { 
                _url = value;
                OnPropertyChanged();
            }
        }
        public string Start
        {
            get => _start;
            set
            {
                _start = value;
                OnPropertyChanged();
            }
        }
        public string End
        {
            get => _end;
            set
            {
                _end = value;
                OnPropertyChanged();
            }
        }
        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Reset()
        {
            End = TimeCode;
            Start = TimeCode;
            Url = "";
            Result = "";
        }

        public const string TimeCode = "00:00:00";

    }
}
