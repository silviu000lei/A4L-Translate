using Microsoft.Win32;
using System;
using System.Windows;

namespace A4L_Translate
{
    public class RegistrySave
    {

        public static readonly string NameApp = "A4L Translate";
        public static readonly string authDeeplKey = "auth_deepl";

        public static void SaveValOnRegistry(string key, string value)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(@"Software\" + NameApp);
            if (regKey != null)
            {
                regKey.SetValue(key, value);
                regKey.Close();
            }
            else
            {
                MessageBox.Show("Nu s-a putut crea sau deschide cheia.");
            }
        }
        public static string? GetValFromRegistry(string key)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\" + NameApp);

            if (regKey != null)
            {
                // Preia valoarea din cheia specificată
                object value = regKey.GetValue(key);

                if (value != null)
                {
                    return value.ToString();
                }
                else
                {
                    MessageBox.Show("Cheia nu conține o valoare 'MySetting'.");
                }

                regKey.Close();
            }
            else
            {
                MessageBox.Show("Nu s-a putut deschide cheia.");
            }

            return null;
        }
    }
}
