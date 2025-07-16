using Palisades.Helpers;
using Palisades.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static Palisades.Helpers.Extensions;

namespace Palisades.ViewModel
{
    public partial class PalisadeViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool disposedValue;

        public Palisade Config;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void Save(string? propertyName = null)
        {
            Debounce(200 + Config.Shortcuts.Count * 10, () =>
            {
                PalisadesManager.SavePalisade(Identifier);
                OnPropertyChanged(propertyName);
                return Task.CompletedTask;
            });
        }
        public string Identifier
        {
            get => Config.Identifier;
            set
            {
                if (Config.Identifier != value)
                {
                    Config.Identifier = value;
                    Save(nameof(Identifier));
                }
            }
        }
        public int FenceX
        {
            get => Config.FenceX;
            set
            {
                if (Config.FenceX != value)
                {
                    Config.FenceX = value;
                    Save(nameof(FenceX));
                }
            }
        }
        public int FenceY
        {
            get => Config.FenceY;
            set
            {
                if (Config.FenceY != value)
                {
                    Config.FenceY = value;
                    Save(nameof(FenceY));
                }
            }
        }
        public int Width
        {
            get => Config.Width;
            set
            {
                if (Config.Width != value)
                {
                    Config.Width = value;
                    Save(nameof(Width));
                }
            }
        }
        public int Height
        {
            get => Config.Height;
            set
            {
                if (Config.Height != value)
                {
                    Config.Height = value;
                    Save(nameof(Height));
                }
            }
        }
        public ObservableCollection<Shortcut> Shortcuts
        {
            get => Config.Shortcuts;
            set
            {
                if (Config.Shortcuts != value)
                {
                    Config.Shortcuts = value;
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
            get => Config.Name;
            set
            {
                if (Config.Name != value)
                {
                    Config.Name = value;
                    Save(nameof(Name));
                }
            }
        }

        public Color HeaderColor
        {
            get => Config.HeaderColor;
            set
            {
                if (Config.HeaderColor != value)
                {
                    Config.HeaderColor = value;
                    Save(nameof(HeaderColor));
                }
            }
        }

        public Color BodyColor
        {
            get => Config.BodyColor;
            set
            {
                if (Config.BodyColor != value)
                {
                    Config.BodyColor = value;
                    Save(nameof(BodyColor));
                }
            }
        }

        public SolidColorBrush TitleColor
        {
            get => new(Config.TitleColor);
            set
            {
                if (Config.TitleColor != value.Color)
                {
                    Config.TitleColor = value.Color;
                    Save(nameof(TitleColor));
                }
            }
        }

        public SolidColorBrush LabelsColor
        {
            get => new(Config.LabelsColor);
            set
            {
                if (Config.LabelsColor != value.Color)
                {
                    Config.LabelsColor = value.Color;
                    Save(nameof(LabelsColor));
                }
            }
        }

        public PalisadeViewModel()
        {
            Config = PalisadesManager.defaultConfig;
            return;
        }
        public PalisadeViewModel(Palisade config)
        {
            Config = config;
            Loc.LanguageChanged += RefreshLocalizedProperties;
            Config.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
            Shortcuts.CollectionChanged += (s, e) => Save(nameof(Shortcuts));
        }
        public string Edit_Title => Loc.Get("Palisade.Edit.Title");
        public string Edit_NameLabel => Loc.Get("Palisade.Edit.NameLabel");
        public string Edit_HeaderColorLabel => Loc.Get("Palisade.Edit.HeaderColorLabel");
        public string Edit_BodyColorLabel => Loc.Get("Palisade.Edit.BodyColorLabel");
        public string Edit_TitleColorLabel => Loc.Get("Palisade.Edit.TitleColorLabel");
        public string Edit_LabelsColorLabel => Loc.Get("Palisade.Edit.LabelsColorLabel");
        public string ContextMenu_Settings => Loc.Get("Palisade.ContextMenu.Settings");
        public string ContextMenu_About => Loc.Get("Palisade.ContextMenu.About");
        public string ContextMenu_Edit => Loc.Get("Palisade.ContextMenu.Edit");
        public string ContextMenu_Delete => Loc.Get("Palisade.ContextMenu.Delete");
        public string ContextMenu_New => Loc.Get("Palisade.ContextMenu.New");
        public string Shortcut_ContextMenu_Delete => Loc.Get("Palisade.Shortcut.ContextMenu.Delete");
        private void RefreshLocalizedProperties(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Edit_Title));
            OnPropertyChanged(nameof(Edit_NameLabel));
            OnPropertyChanged(nameof(Edit_HeaderColorLabel));
            OnPropertyChanged(nameof(Edit_BodyColorLabel));
            OnPropertyChanged(nameof(Edit_TitleColorLabel));
            OnPropertyChanged(nameof(Edit_LabelsColorLabel));
            OnPropertyChanged(nameof(ContextMenu_Settings));
            OnPropertyChanged(nameof(ContextMenu_About));
            OnPropertyChanged(nameof(ContextMenu_Edit));
            OnPropertyChanged(nameof(ContextMenu_Delete));
            OnPropertyChanged(nameof(ContextMenu_New));
            OnPropertyChanged(nameof(Shortcut_ContextMenu_Delete));
        }

        // 命令：新建
        public ICommand NewPalisadeCommand { get; } = new RelayCommand(() =>
        {
            PalisadesManager.CreatePalisade();
        });

        // 命令：删除
        public ICommand DeletePalisadeCommand { get; } = new RelayCommand<string>(PalisadesManager.DeletePalisade);

        // 命令：编辑
        public ICommand EditPalisadeCommand { get; } = new RelayCommand<PalisadeViewModel>(viewModel =>
        {
            var owner = PalisadesManager.GetPalisade(viewModel.Identifier) ?? Application.Current.MainWindow;
            
            var edit = new View.Edit(viewModel);

            if (owner != null && owner.IsVisible)
            {
                edit.Owner = owner;
            }

            edit.ShowDialog();
        });

        // 命令：关于
        public ICommand OpenAboutCommand { get; } = new RelayCommand<PalisadeViewModel>(viewModel =>
        {
            var owner = PalisadesManager.GetPalisade(viewModel.Identifier) ?? Application.Current.MainWindow;

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
