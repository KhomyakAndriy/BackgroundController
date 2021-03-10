using System;
using System.Collections.Generic;
using System.Configuration;

namespace BackgroundController
{
    public static class SettingsManager
    {
        public static T ReadSetting<T>(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings[key];
            if (string.IsNullOrWhiteSpace(result))
            {
                throw new KeyNotFoundException($"Key \"{key}\" not found in settings.");
            }

            return (T) Convert.ChangeType(result, typeof(T));
        }

        public static void AddUpdateAppSettings(string key, object value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value.ToString());
                }
                else
                {
                    settings[key].Value = value.ToString();
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }
}
