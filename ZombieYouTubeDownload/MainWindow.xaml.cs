using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZombieYouTubeDownload.Context;

namespace ZombieYouTubeDownload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UrlContext _urlContext = new UrlContext();

        public MainWindow()
        {
            _urlContext.Reset();
            this.DataContext = _urlContext;
            InitializeComponent();
        }

        #region Events
        private void CalculateResult(object sender, RoutedEventArgs e)
        {
            _urlContext.Result = "";

            //Validation
            if (!IsValidTimeCode(_urlContext.Start, out TimeSpan start))
            {
                _urlContext.Start = UrlContext.TimeCode;
                return;
            }

            if (!IsValidTimeCode(_urlContext.End, out TimeSpan end))
            {
                _urlContext.End = UrlContext.TimeCode;
                return;
            }

            if (start > end || start == new TimeSpan())
            {
                _urlContext.Start = UrlContext.TimeCode;
                _urlContext.End = UrlContext.TimeCode;
                return;
            }

            if (!Uri.TryCreate(_urlContext.Url, UriKind.Absolute, out _))
            {
                _urlContext.Url = "";
                return;
            }

            _urlContext.Result = GetFormattedString(url: _urlContext.Url,
                                                    videoInformation: _urlContext.VideoInformation,
                                                    start: _urlContext.Start,
                                                    end: _urlContext.End);
        }

        public void SendCommand(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(_urlContext.Url))
            {
                return;
            }

            var startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                UseShellExecute = false,
                Arguments = @"/K cd C:\ &" + _urlContext.Result
            };
            var process = new Process { StartInfo = startInfo };

            process.Start();
            process.WaitForExit();
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            _urlContext.Reset();
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
        }
        #endregion

        #region Private functions
        private bool IsValidTimeCode(string timeCode, out TimeSpan time)
        {
            time = new TimeSpan();
            if (timeCode.Length > 8) return false;
            if (timeCode.Split(':').Length != 3) return false;
            var chunks = timeCode.Split(':');
            foreach (var chunk in chunks)
            {
                if (chunk.Length != 2) return false;
                if (!chunk.All(a => Char.IsNumber(a))) return false;
                if (int.TryParse(chunk, out int chunkResult) && chunkResult >= 60) return false;
            }
            
            time = new TimeSpan(int.Parse(chunks[0]), int.Parse(chunks[1]), int.Parse(chunks[2]));

            return true;
        }

        private string GetFormattedString(string url, bool videoInformation, string start, string end)
        {
            //Get video information:
            //yt-dlp --list-formats https://www.youtube.com/watch?v=LNHbm7GBHwg&t=1767s

            if(videoInformation)
            {
                return $"""
                    yt-dlp --list-formats {url}
                    """.Trim();
            }
            else
            {
                return $"""
                    yt-dlp --output "D:\Video Sources\%(title)s-%(id)s.%(ext)s" -S res,ext:mp4:m4a --recode mp4 --external-downloader ffmpeg --external-downloader-args "ffmpeg_i:-ss {start} -to {end}" "{url}"
                    """.Trim();
            }


        }
        #endregion

    }
}
