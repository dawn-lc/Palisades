using System;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Palisades.Helpers
{
    public class SettingsManager
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
        private static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Palisades",
            "settings.json"
        );

        public string Language { get; set; } = "en-US";

        private static SettingsManager? _instance;
        public static SettingsManager Instance
        {
            get
            {
                _instance ??= Load();
                return _instance;
            }
        }

        public static SettingsManager Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    var loaded = JsonSerializer.Deserialize<SettingsManager>(json) ?? new SettingsManager();
                    if (string.IsNullOrWhiteSpace(loaded.Language))
                    {
                        loaded.Language = "en-US";
                    }
                    return loaded;
                }
            }
            catch (Exception ex)
            {
                // Fallback to default settings if file is corrupted
                Console.WriteLine($"[SettingsManager] Failed to load settings: {ex.Message}\nUsing default settings.");
            }
            return new SettingsManager();
        }

        public void Save()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(SettingsPath)!;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string json = JsonSerializer.Serialize(this, jsonSerializerOptions);
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                // Handle save errors gracefully
                Console.WriteLine($"[SettingsManager] Failed to save settings: {ex.Message}");
            }
        }

        public void ApplyLanguage()
        {
            try
            {
                var lang = string.IsNullOrWhiteSpace(Language) ? "en-US" : Language;
                CultureInfo culture = new(lang);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            catch (CultureNotFoundException ex)
            {
                Console.WriteLine($"[SettingsManager] Invalid culture '{Language}': {ex.Message}");
            }
        }
    }
}