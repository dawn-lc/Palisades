using Palisades.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

        #region Constants
        private const string NOT_FOUND = "not found";
        #endregion

        #region Attributs
        private readonly string version;
        private readonly string releaseDate;
        #endregion

        #region Accessors
        public string Version => version;
        public string ReleaseDate => releaseDate;


        public List<CreditItem> Credits { get; } =
        [
            new() { Name = "GongSolutions.WPF.DragDrop", Url = "https://github.com/punker76/gong-wpf-dragdrop", Description = "Drag & Drop functionality" },
            new() { Name = "Hardcodet.NotifyIcon.Wpf", Url = "https://github.com/hardcodet/wpf-notifyicon", Description = "Windows tray icon support" },
            new() { Name = "PixiEditor.ColorPicker", Url = "https://github.com/PixiEditor/ColorPicker", Description = "Color picker control" }
        ];
        #endregion

        public AboutViewModel()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? maybeVersion = assembly.GetName().Version;
            version = maybeVersion != null ? "v" + maybeVersion.Major + "." + maybeVersion.Minor + "." + maybeVersion.Build : NOT_FOUND;

            ReleaseDateAttribute? maybeDateAttribute = assembly.GetCustomAttribute<ReleaseDateAttribute>();
            releaseDate = string.Format(Loc.Get("AboutReleaseDate"), 
                maybeDateAttribute != null ? maybeDateAttribute.DateTime.ToShortDateString() : NOT_FOUND);

            Loc.LanguageChanged += (s, e) => RefreshLocalizedProperties();
        }

        private void RefreshLocalizedProperties()
        {
            OnPropertyChanged(nameof(AboutTitle));
            OnPropertyChanged(nameof(AboutMaintainedBy));
            OnPropertyChanged(nameof(AboutLicenseInfo));
            OnPropertyChanged(nameof(AboutGithubRepo));
            OnPropertyChanged(nameof(AboutCredits));
            OnPropertyChanged(nameof(AboutLibrariesIntro));
            OnPropertyChanged(nameof(ReleaseDate));
        }

        #region Commands
        public ICommand NavigateCommand { get; private set; } = new RelayCommand<string>((url) =>
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        });
        #endregion

        public string AboutTitle => Loc.Get("AboutTitle");
        public string AboutMaintainedBy => Loc.Get("AboutMaintainedBy");
        public string AboutLicenseInfo => Loc.Get("AboutLicenseInfo");
        public string AboutGithubRepo => Loc.Get("AboutGithubRepo");
        public string AboutCredits => Loc.Get("AboutCredits");
        public string AboutLibrariesIntro => Loc.Get("AboutLibrariesIntro");

    }
}
