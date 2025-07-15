using Palisades.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Palisades.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public class LanguageItem
        {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public IEnumerable<LanguageItem> AvailableLanguages => Loc.AvailableLanguages.Select(l => new LanguageItem { Code = l.code, Name = l.name });

        public string SettingsPage => Loc.Get("SettingsPage");
        public string Language => Loc.Get("Language");
        public string CurrentLanguage => Loc.CurrentLanguage;

        private string selectedLanguage = Loc.CurrentLanguage;
        public string SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && selectedLanguage != value)
                {
                    selectedLanguage = value;
                    Loc.SetLanguage(value);
                    OnPropertyChanged(nameof(SelectedLanguage));
                    OnPropertyChanged(nameof(CurrentLanguage));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SettingsViewModel()
        {
            Loc.LanguageChanged += RefreshLocalizedProperties;
        }

        private void RefreshLocalizedProperties(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SettingsPage));
            OnPropertyChanged(nameof(Language));
            OnPropertyChanged(nameof(AvailableLanguages));
            OnPropertyChanged(nameof(CurrentLanguage));
            OnPropertyChanged(nameof(SelectedLanguage));
        }
    }
}
