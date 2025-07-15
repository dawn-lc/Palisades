using Palisades.Helpers;
using Palisades.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Palisades.ViewModel
{
    public partial class PalisadeViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool disposedValue;
        private readonly PalisadeModel model;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Timer? debounceTimer;
        public void Save(string? propertyName = null)
        {
            debounceTimer?.Stop();
            debounceTimer?.Dispose();
            debounceTimer = new Timer(200)
            {
                AutoReset = false
            };
            debounceTimer.Elapsed += (s, e) =>
            {
                model.Save(propertyName);
                OnPropertyChanged(propertyName);
            };
            debounceTimer.Start();
        }
        public void Delete()
        {
            string saveDirectory = PDirectory.GetPalisadeDirectory(Identifier);
            Directory.Delete(saveDirectory, true);
        }
        public string Identifier
        {
            get => model.Identifier;
            set
            {
                if (model.Identifier != value)
                {
                    model.Identifier = value;
                    Save(nameof(Identifier));
                }
            }
        }
        public int FenceX
        {
            get => model.FenceX;
            set
            {
                if (model.FenceX != value)
                {
                    model.FenceX = value;
                    Save(nameof(FenceX));
                }
            }
        }
        public int FenceY
        {
            get => model.FenceY;
            set
            {
                if (model.FenceY != value)
                {
                    model.FenceY = value;
                    Save(nameof(FenceY));
                }
            }
        }
        public int Width
        {
            get => model.Width;
            set
            {
                if (model.Width != value)
                {
                    model.Width = value;
                    Save(nameof(Width));
                }
            }
        }
        public int Height
        {
            get => model.Height;
            set
            {
                if (model.Height != value)
                {
                    model.Height = value;
                    Save(nameof(Height));
                }
            }
        }
        public ObservableCollection<Shortcut> Shortcuts
        {
            get => model.Shortcuts;
            set
            {
                if (model.Shortcuts != value)
                {
                    model.Shortcuts = value;
                    Save(nameof(Shortcuts));
                }
            }
        }

        private Shortcut? selectedShortcut;
        public Shortcut? SelectedShortcut
        {
            get => selectedShortcut;
            set
            {
                if (selectedShortcut != value)
                {
                    selectedShortcut = value;
                    Save(nameof(SelectedShortcut));
                }
            }
        }

        public string Name
        {
            get => model.Name;
            set
            {
                if (model.Name != value)
                {
                    model.Name = value;
                    Save(nameof(Name));
                }
            }
        }

        public Color HeaderColor
        {
            get => model.HeaderColor;
            set
            {
                if (model.HeaderColor != value)
                {
                    model.HeaderColor = value;
                    Save(nameof(HeaderColor));
                }
            }
        }

        public Color BodyColor
        {
            get => model.BodyColor;
            set
            {
                if (model.BodyColor != value)
                {
                    model.BodyColor = value;
                    Save(nameof(HeaderColor));
                }
            }
        }

        public SolidColorBrush TitleColor
        {
            get => new(model.TitleColor);
            set
            {
                if (model.TitleColor != value.Color)
                {
                    model.TitleColor = value.Color;
                    Save(nameof(TitleColor));
                }
            }
        }

        public SolidColorBrush LabelsColor
        {
            get => new(model.LabelsColor);
            set
            {
                if (model.LabelsColor != value.Color)
                {
                    model.LabelsColor = value.Color;
                    Save(nameof(LabelsColor));
                }
            }
        }

        public string EditPageTitleText => Loc.Get("EditTitle");
        public string NameLabelText => Loc.Get("NameLabel");
        public string HeaderColorLabelText => Loc.Get("HeaderColorLabel");
        public string BodyColorLabelText => Loc.Get("BodyColorLabel");
        public string TitleColorLabelText => Loc.Get("TitleColorLabel");
        public string LabelsColorLabelText => Loc.Get("LabelsColorLabel");

        public string SettingsText => Loc.Get("TraySettings");
        public string AboutText => Loc.Get("AboutTitle");
        public string EditFence => Loc.Get("EditFence");
        public string DeleteFence => Loc.Get("DeleteFence");
        public string NewFence => Loc.Get("NewFence");

        public string DeleteShortcutText => Loc.Get("DeleteShortcut");

        public PalisadeViewModel() : this(new PalisadeModel()) { }

        public PalisadeViewModel(PalisadeModel model)
        {
            this.model = model;
            Shortcuts.CollectionChanged += (_, _) => Save();
            Loc.LanguageChanged += RefreshLocalizedProperties;
        }
        private void RefreshLocalizedProperties(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(SettingsText));
            OnPropertyChanged(nameof(AboutText));
            OnPropertyChanged(nameof(EditFence));
            OnPropertyChanged(nameof(DeleteFence));
            OnPropertyChanged(nameof(NewFence));
            OnPropertyChanged(nameof(EditPageTitleText));
            OnPropertyChanged(nameof(NameLabelText));
            OnPropertyChanged(nameof(HeaderColorLabelText));
            OnPropertyChanged(nameof(BodyColorLabelText));
            OnPropertyChanged(nameof(TitleColorLabelText));
            OnPropertyChanged(nameof(LabelsColorLabelText));
        }

        // 命令：新建
        public ICommand NewPalisadeCommand { get; } = new RelayCommand(PalisadesManager.CreatePalisade);

        // 命令：删除
        public ICommand DeletePalisadeCommand { get; } = new RelayCommand<string>(PalisadesManager.DeletePalisade);

        // 命令：编辑
        public ICommand EditPalisadeCommand { get; } = new RelayCommand<PalisadeViewModel>(vm =>
        {
            var owner = PalisadesManager.GetPalisade(vm.Identifier) ?? Application.Current.MainWindow;

            var edit = new View.Edit
            {
                DataContext = vm
            };

            if (owner != null && owner.IsVisible)
            {
                edit.Owner = owner;
            }

            edit.ShowDialog();
        });

        // 命令：关于
        public ICommand OpenAboutCommand { get; } = new RelayCommand<PalisadeViewModel>(vm =>
        {
            var owner = PalisadesManager.GetPalisade(vm.Identifier) ?? Application.Current.MainWindow;

            var about = new View.About
            {
                DataContext = new AboutViewModel()
            };

            if (owner != null && owner.IsVisible)
            {
                about.Owner = owner;
            }

            about.ShowDialog();
        });

        // 命令：设置
        public ICommand OpenSettingsCommand { get; } = new RelayCommand(() =>
        {
            var owner = Application.Current.MainWindow;

            var settingsWindow = new View.Settings();

            if (owner != null && owner.IsVisible)
            {
                settingsWindow.Owner = owner;
            }

            settingsWindow.ShowDialog();
        });
        public ICommand DeleteShortcutCommand => new RelayCommand<Shortcut>(DeleteShortcut);
        public void DeleteShortcut(Shortcut? shortcut)
        {
            if (shortcut != null)
            {
                Shortcuts.Remove(shortcut);
                if (SelectedShortcut == shortcut)
                    SelectedShortcut = null;
            }
            else if (SelectedShortcut != null)
            {
                Shortcuts.Remove(SelectedShortcut);
                SelectedShortcut = null;
            }
        }

        public ICommand DropShortcut => new RelayCommand<DragEventArgs>(DropShortcutsHandler);

        public ICommand ClickShortcut => new RelayCommand<Shortcut>(SelectShortcut);

        public ICommand DelKeyPressed => new RelayCommand(DeleteShortcut);

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
                if (Directory.Exists(path))
                {
                    var folderShortcut = FolderShortcut.BuildFrom(path, Identifier);
                    Shortcuts.Add(folderShortcut);
                }
                else if (File.Exists(path))
                {
                    string? ext = Path.GetExtension(path)?.ToLowerInvariant();
                    Shortcut? shortcut = ext switch
                    {
                        ".lnk" => LnkShortcut.BuildFrom(path, Identifier),
                        ".url" => UrlShortcut.BuildFrom(path, Identifier),
                        _ => FileShortcut.BuildFrom(path, Identifier)
                    };

                    if (shortcut != null)
                        Shortcuts.Add(shortcut);
                }
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
 

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~PalisadeViewModel()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
