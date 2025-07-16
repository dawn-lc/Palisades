using Palisades.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Palisades.View;

namespace Palisades.ViewModel
{
    public partial class AboutViewModel : INotifyPropertyChanged
    {
        public class CreditItem
        {
            public string Name { get; set; } = "";
            public string Url { get; set; } = "";
            public string Description { get; set; } = "";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private const string NOT_FOUND = "Not found";

        private readonly string version;
        public string Version => version;

        private readonly string releaseDate;
        public string ReleaseDate => releaseDate;

        public List<CreditItem> Credits { get; } =
        [
            new() { Name = "GongSolutions.WPF.DragDrop", Url = "https://github.com/punker76/gong-wpf-dragdrop", Description = "Drag & Drop functionality" },
            new() { Name = "Hardcodet.NotifyIcon.Wpf", Url = "https://github.com/hardcodet/wpf-notifyicon", Description = "Windows tray icon support" },
            new() { Name = "PixiEditor.ColorPicker", Url = "https://github.com/PixiEditor/ColorPicker", Description = "Color picker control" },
            new() { Name = "PixiEditor.ColorPicker.Models", Url = "https://github.com/PixiEditor/ColorPicker", Description = "Color picker models" },
            new() { Name = "Microsoft.Xaml.Behaviors.Wpf", Url = "https://github.com/microsoft/XamlBehaviorsWpf", Description = "WPF behaviors and triggers" },
            new() { Name = "Sentry", Url = "https://github.com/getsentry/sentry-dotnet", Description = "Error monitoring and reporting" }
        ];

        public AboutViewModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? maybeVersion = assembly.GetName().Version;
            version = maybeVersion != null ? "v" + maybeVersion.Major + "." + maybeVersion.Minor + "." + maybeVersion.Build : NOT_FOUND;

            ReleaseDateAttribute? maybeDateAttribute = assembly.GetCustomAttribute<ReleaseDateAttribute>();
            releaseDate = string.Format(Loc.Get("Palisade.About.ReleaseDate"),
                maybeDateAttribute != null ? maybeDateAttribute.DateTime.ToShortDateString() : NOT_FOUND);

            Authors = ParseAuthorsFromString(Loc.Get("Palisade.About.Authors"));
            Loc.LanguageChanged += (s, e) => RefreshLocalizedProperties();
        }

        public string About_Title => Loc.Get("Palisade.About.Title");
        public string About_LicenseInfo => Loc.Get("Palisade.About.LicenseInfo");
        public string About_Credits => Loc.Get("Palisade.About.Credits");
        public class AuthorItem
        {
            public string Name { get; set; } = "";
            public string Url { get; set; } = "";
        }

        private List<AuthorItem> ParseAuthorsFromString(string authorsStr)
        {
            var authors = new List<AuthorItem>();
            var entries = authorsStr.Split([','], StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                var parts = entry.Split('|');
                if (parts.Length == 2)
                {
                    authors.Add(new AuthorItem
                    {
                        Name = parts[0].Trim(),
                        Url = parts[1].Trim()
                    });
                }
            }
            return authors;
        }

        public List<AuthorItem> Authors { get; private set; }

        public string About_HomePage => Loc.Get("Palisade.About.HomePage");
        public string About_HomePageURL => Loc.Get("Palisade.About.URL");
        public string About_LibrariesIntro => Loc.Get("Palisade.About.LibrariesIntro");
        private void RefreshLocalizedProperties()
        {
            OnPropertyChanged(nameof(About_Title));
            OnPropertyChanged(nameof(About_LicenseInfo));
            OnPropertyChanged(nameof(About_Credits));
            OnPropertyChanged(nameof(About_HomePage));
            OnPropertyChanged(nameof(About_LibrariesIntro));
            OnPropertyChanged(nameof(ReleaseDate));
            OnPropertyChanged(nameof(Authors));
        }


        public ICommand NavigateCommand { get; private set; } = new RelayCommand<string>((url) =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        });
    }
}
