using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Palisades.Helpers;
using Palisades.Model;

namespace Palisades.ViewModel
{
    public partial class PalisadeViewModel : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private readonly PalisadeModel model;
        private readonly CancellationTokenSource cancelToken = new();
        private readonly Task saveTask;

        private bool shouldSave;
        private Shortcut? selectedShortcut;

        #endregion

        #region Properties

        public string Identifier
        {
            get => model.Identifier;
            set => SetAndSave(() => model.Identifier, v => model.Identifier = v, value);
        }

        public string Name
        {
            get => model.Name;
            set => SetAndSave(() => model.Name, v => model.Name = v, value);
        }

        public int FenceX
        {
            get => model.FenceX;
            set => SetAndSave(() => model.FenceX, v => model.FenceX = v, value);
        }

        public int FenceY
        {
            get => model.FenceY;
            set => SetAndSave(() => model.FenceY, v => model.FenceY = v, value);
        }

        public int Width
        {
            get => model.Width;
            set => SetAndSave(() => model.Width, v => model.Width = v, value);
        }

        public int Height
        {
            get => model.Height;
            set => SetAndSave(() => model.Height, v => model.Height = v, value);
        }

        public Color HeaderColor
        {
            get => model.HeaderColor;
            set => SetAndSave(() => model.HeaderColor, v => model.HeaderColor = v, value);
        }

        public Color BodyColor
        {
            get => model.BodyColor;
            set => SetAndSave(() => model.BodyColor, v => model.BodyColor = v, value);
        }

        public SolidColorBrush TitleColor
        {
            get => new(model.TitleColor);
            set => SetAndSave(() => model.TitleColor, v => model.TitleColor = value.Color, value.Color);
        }

        public SolidColorBrush LabelsColor
        {
            get => new(model.LabelsColor);
            set => SetAndSave(() => model.LabelsColor, v => model.LabelsColor = value.Color, value.Color);
        }

        public ObservableCollection<Shortcut> Shortcuts
        {
            get => model.Shortcuts;
            set => SetAndSave(() => model.Shortcuts, v => model.Shortcuts = v, value);
        }

        public Shortcut? SelectedShortcut
        {
            get => selectedShortcut;
            set => SetAndSave(() => selectedShortcut, v => selectedShortcut = v, value);
        }

        public string SettingsText => Loc.Get("TraySettings");
        public string AboutText => Loc.Get("AboutTitle");
        public string EditFence => Loc.Get("EditFence");
        public string DeleteFence => Loc.Get("DeleteFence");
        public string NewFence => Loc.Get("NewFence");

        #endregion

        #region Init

        public PalisadeViewModel() : this(new PalisadeModel()) { }

        public PalisadeViewModel(PalisadeModel model)
        {
            this.model = model;

            Shortcuts.CollectionChanged += (_, _) => Save();

            saveTask = Task.Run(SaveLoop, cancelToken.Token);

            Loc.LanguageChanged += (_, _) => RefreshLocalizedProperties();
        }

        #endregion

        #region Save

        private async Task SaveLoop()
        {
            while (!cancelToken.Token.IsCancellationRequested)
            {
                if (shouldSave)
                {
                    SaveNow();
                }
                await Task.Delay(1000, cancelToken.Token);
            }
        }

        private void SaveNow()
        {
            try
            {
                string saveDirectory = PDirectory.GetPalisadeDirectory(Identifier);
                PDirectory.EnsureExists(saveDirectory);

                using StreamWriter writer = new(Path.Combine(saveDirectory, "state.xml"));
                XmlSerializer serializer = new(typeof(PalisadeModel), [typeof(Shortcut), typeof(LnkShortcut), typeof(UrlShortcut)]);
                serializer.Serialize(writer, this.model);

                shouldSave = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveNow] Error: {ex}");
            }
        }

        public void Save() => shouldSave = true;

        public void Delete()
        {
            string saveDirectory = PDirectory.GetPalisadeDirectory(Identifier);
            Directory.Delete(saveDirectory, true);
        }

        #endregion

        #region Commands

        public ICommand NewPalisadeCommand { get; } = new RelayCommand(PalisadesManager.CreatePalisade);

        public ICommand DeletePalisadeCommand { get; } = new RelayCommand<string>(PalisadesManager.DeletePalisade);

        public ICommand EditPalisadeCommand { get; } = new RelayCommand<PalisadeViewModel>(vm =>
        {
            var edit = new View.EditPalisade
            {
                DataContext = vm,
                Owner = PalisadesManager.GetPalisade(vm.Identifier)
            };
            edit.ShowDialog();
        });

        public ICommand OpenAboutCommand { get; } = new RelayCommand<PalisadeViewModel>(vm =>
        {
            var about = new View.About
            {
                DataContext = new AboutViewModel(),
                Owner = PalisadesManager.GetPalisade(vm.Identifier)
            };
            about.ShowDialog();
        });

        public ICommand OpenSettingsCommand { get; } = new RelayCommand(() =>
        {
            var settingsWindow = new View.Settings
            {
                Owner = Application.Current.MainWindow
            };
            settingsWindow.ShowDialog();
        });

        public ICommand DropShortcut => new RelayCommand<DragEventArgs>(DropShortcutsHandler);

        public ICommand ClickShortcut => new RelayCommand<Shortcut>(SelectShortcut);

        public ICommand DelKeyPressed => new RelayCommand(DeleteShortcut);

        #endregion

        #region Shortcuts

        public void DropShortcutsHandler(DragEventArgs e)
        {
            e.Handled = true;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Handled = false;
                return;
            }

            string[] dropped = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (var path in dropped)
            {
                string? ext = Path.GetExtension(path)?.ToLowerInvariant();

                Shortcut? shortcut = ext switch
                {
                    ".lnk" => LnkShortcut.BuildFrom(path, Identifier),
                    ".url" => UrlShortcut.BuildFrom(path, Identifier),
                    _ => null
                };

                if (shortcut != null)
                    Shortcuts.Add(shortcut);
            }
        }

        public void SelectShortcut(Shortcut shortcut)
            => SelectedShortcut = SelectedShortcut == shortcut ? null : shortcut;

        public void DeleteShortcut()
        {
            if (SelectedShortcut != null)
            {
                Shortcuts.Remove(SelectedShortcut);
                SelectedShortcut = null;
            }
        }

        #endregion

        #region Helpers

        private void RefreshLocalizedProperties()
        {
            OnPropertyChanged(nameof(SettingsText));
            OnPropertyChanged(nameof(AboutText));
            OnPropertyChanged(nameof(EditFence));
            OnPropertyChanged(nameof(DeleteFence));
            OnPropertyChanged(nameof(NewFence));
        }

        private void SetAndSave<T>(Func<T> getter, Action<T> setter, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(getter(), value))
            {
                setter(value);
                OnPropertyChanged(propertyName);
                Save();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Dispose()
        {
            cancelToken.Cancel();
            saveTask.Wait();
            if (shouldSave) SaveNow();
            cancelToken.Dispose();
        }

        #endregion
    }
}
