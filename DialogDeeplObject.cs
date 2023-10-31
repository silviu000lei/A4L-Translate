using DeepL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A4L_Translate
{
    internal class DialogDeepLObject
    {
        public DialogDeepLObject(string time_string, string subtitle_info, string subtitle_translated)
        {
            Time = TimeSpan.Parse(time_string);
            SubtitleInfo = subtitle_info;
            Subtitle = subtitle_translated;
        }

        public TimeSpan Time { get; set; }
        public string SubtitleInfo {  get; set; }
        public string Subtitle {  get; set; }
    }
}
