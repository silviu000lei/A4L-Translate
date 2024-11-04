using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using DeepL;
using DeepL.Model;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics;

namespace A4L_Translate
{
    public partial class MainWindow : Window
    {
        private Translator translator;
        private StringBuilder sb;
        private string Filepath;
        private List<DialogDeepLObject> Dialogs = new List<DialogDeepLObject>();
        public MainWindow() => InitializeComponent();

        private void Load_File_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fișiere Text (*.ass)|*.ass";
            if (openFileDialog.ShowDialog() == true)
            {
                Filepath = openFileDialog.FileName;
                sb = new StringBuilder();
                translate.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new();
            settings.ShowDialog();
        }

        private async Task Translate_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                sb = new StringBuilder();
                Dialogs.Clear();

                translate.IsEnabled = false;
                Download.IsEnabled = false;
                string authKey = RegistrySave.GetValFromRegistry(RegistrySave.authDeeplKey);
                if (authKey == "" || authKey == null)
                {
                    Settings settings = new();
                    settings.ShowDialog();
                    return;
                }
                translator = new Translator(authKey);
                progress_translate.Maximum = 100;
                progress_translate.Minimum = 0;
                string[] lines = File.ReadAllLines(Filepath, Encoding.UTF8); // Asigură citirea în UTF-8
                int countLines = lines.Count();
                int nrLine = 0;
                foreach (string line in lines)
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }
                    await PushLineToWriterAsync(line);
                    nrLine++;
                    decimal progress = nrLine * 95 / countLines;
                    progress_translate.Value = (int)Math.Floor(progress);
                }
                List<DialogDeepLObject> DialogsOrdred = Dialogs.OrderBy(dialog => dialog.Time).ToList();
                foreach(DialogDeepLObject Dialog in DialogsOrdred)
                {
                    sb.AppendLine(Dialog.SubtitleInfo + ",," + Dialog.Subtitle);
                }
                progress_translate.Value = 100;
                Download.IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error translate the file: {ex.Message}");
            }
        }

        private async Task PushLineToWriterAsync(string line)
        {
            if (line.StartsWith("Dialogue:", StringComparison.OrdinalIgnoreCase))
            {
                string[] lineSplited = line.Split(new string[] { ",," },StringSplitOptions.None);

                string subtitle_en = CleanSubLine(lineSplited[lineSplited.Length - 1]);
                if(subtitle_en == null)
                {
                    return;
                }
                if (subtitle_en.Length == 0)
                {
                    return;
                }
                string[] lineSplitedWithoutSub = lineSplited.SkipLast(1).ToArray();
                string subtitle_info = lineSplited[0];
                string[] subtitle_info_splited = subtitle_info.Split(new char[] { ',' }, StringSplitOptions.None);
                string time_string = subtitle_info_splited[2];
                TextResult subtitle_translated = await translator.TranslateTextAsync(subtitle_en, LanguageCode.English, LanguageCode.Romanian);
                DialogDeepLObject dialog = new(time_string, string.Join(",,", lineSplitedWithoutSub), subtitle_translated.ToString());
                Dialogs.Add(dialog);
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        private static string CleanSubLine(string line)
        {
            // Înlocuiește tot ce se află între acolade cu un șir gol
            string noBraces = Regex.Replace(line, "\\{.*?\\}", "");

            // Înlocuiește "\\N" cu un spațiu
            string replaceSlashN = Regex.Replace(noBraces, "\\\\N", " ");

            return replaceSlashN;
        }
        private void Translate_Click(object sender, RoutedEventArgs e)
        {
            _ = Translate_ClickAsync(sender, e);
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Fișiere text (*.ass)|*.ass";

                if (saveFileDialog.ShowDialog() == true)
                {
                    var stringSub = sb.ToString();
                    byte[] subBytes = Encoding.Default.GetBytes(stringSub); // Scriere în ANSI
                    using (MemoryStream ms = new MemoryStream(subBytes))
                    {
                        using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, System.IO.FileAccess.Write))
                            ms.CopyTo(fileStream);
                    }
                    string folderPath = System.IO.Path.GetDirectoryName(saveFileDialog.FileName);
                    Process.Start("explorer.exe", folderPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error save the file: {ex.Message}");
            }
        }
    }
}
