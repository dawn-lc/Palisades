using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;

namespace Palisades.Helpers
{
    public static class Loc
    {
        private static readonly ResourceManager resourceManager = new("Palisades.Resource.Strings", typeof(Loc).Assembly);

        public static event EventHandler? LanguageChanged;
        public static string CurrentLanguage => CultureInfo.CurrentUICulture.Name;

        public static (string code, string name)[] AvailableLanguages => GetAvailableLanguages();

        public static string Get(string key)
        {
            try
            {
                string? localizedString = resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
                if (string.IsNullOrEmpty(localizedString))
                {
                    Console.WriteLine($"[Loc] Resource key '{key}' not found in current culture '{CultureInfo.CurrentUICulture.Name}'. Using key as fallback.");
                    return key;
                }
                return localizedString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Loc] Failed to get resource for key '{key}': {ex.Message}");
                return key;
            }
        }

        public static void SetLanguage(string languageCode)
        {
            try
            {
                SettingsManager.Instance.Language = languageCode;
                SettingsManager.Instance.ApplyLanguage();
                SettingsManager.Instance.Save();
                LanguageChanged?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Loc] Failed to set language '{languageCode}': {ex.Message}");
            }
        }

        private static (string code, string name)[] GetAvailableLanguages()
        {
            List<(string, string)> result = [("en-US", "English")];
            
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] dirs = Directory.GetDirectories(baseDir);

            foreach (var dir in dirs)
            {
                string dirName = new DirectoryInfo(dir).Name;
                try
                {
                    CultureInfo ci = CultureInfo.GetCultureInfo(dirName);
                    result.Add((ci.Name, ci.NativeName));
                }
                catch (CultureNotFoundException)
                {
                    // 忽略非文化目录
                }
            }

            return [.. result];
        }
    }
}
