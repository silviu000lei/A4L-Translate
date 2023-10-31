using DeepL;
using DeepL.Model;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;


namespace A4L_Translate
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private string deeplAuthKey ;
        private Translator translator;
        public static readonly string NameApp = "A4L Translate";
        public static readonly string authDeeplKey = "auth_deepl";
        public Settings()
        {
            InitializeComponent();
            deepl_auth_key.Text = RegistrySave.GetValFromRegistry(RegistrySave.authDeeplKey);
        }

        private void Cancel_button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async Task Save_button_ClickAsync(object sender, RoutedEventArgs e)
        {
            deeplAuthKey = deepl_auth_key.Text;
            var isValid = await ValidateAuthKeyAsync(deeplAuthKey);
            if (isValid)
            {
                RegistrySave.SaveValOnRegistry(authDeeplKey, deeplAuthKey);
                this.Close();
            }
        }

        private async Task<bool> ValidateAuthKeyAsync(string authKey)
        {
            try
            {
                translator = new Translator(authKey);
                TextResult text_translated = await translator.TranslateTextAsync(" The key is good!", LanguageCode.English, LanguageCode.Romanian);
                MessageBox.Show(text_translated.ToString());
            } 
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;
        }

        private void Save_button_Click(object sender, RoutedEventArgs e)
        {
            _ = Save_button_ClickAsync(sender, e);
        }
    }
}
