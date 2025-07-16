
using Palisades.Helpers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Palisades.Model
{
    [XmlRoot(Namespace = "io.stouder", ElementName = "Palisade")]
    public class Palisade : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private string identifier;
        private string name;
        private int fenceX;
        private int fenceY;
        private int width;
        private int height;
        private Color headerColor;
        private Color bodyColor;
        private Color titleColor;
        private Color labelsColor;

        private ObservableCollection<Shortcut> shortcuts;
        public Palisade()
        {
            identifier = Guid.NewGuid().ToString();
            name = "Default";
            headerColor = Color.FromArgb(200, 0, 0, 0);
            bodyColor = Color.FromArgb(120, 0, 0, 0);
            titleColor = Color.FromArgb(255, 255, 255, 255);
            labelsColor = Color.FromArgb(255, 255, 255, 255);
            width = 720;
            height = 480;
            shortcuts = [];
        }

        public string Identifier
        {
            get => identifier;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Identifier cannot be null or whitespace.", nameof(value));
                }
                identifier = value;
                OnPropertyChanged(propertyName: nameof(Identifier));
            }
        }
        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Name cannot be null or whitespace.", nameof(value));
                }
                name = value;
                OnPropertyChanged(propertyName: nameof(Name));
            }
        }

        public int FenceX
        {
            get => fenceX;
            set
            {
                fenceX = value;
                OnPropertyChanged(propertyName: nameof(FenceX));
            }
        }

        public int FenceY
        {
            get => fenceY;
            set
            {
                fenceY = value;
                OnPropertyChanged(propertyName: nameof(FenceY));
            }
        }

        public int Width
        {
            get => width;
            set
            {
                width = value;
                OnPropertyChanged(propertyName: nameof(Width));
            }
        }

        public int Height
        {
            get => height;
            set
            {
                height = value;
                OnPropertyChanged(propertyName: nameof(Height));
            }
        }

        public Color HeaderColor
        {
            get => headerColor;
            set
            {
                headerColor = value;
                OnPropertyChanged(propertyName: nameof(HeaderColor));
            }
        }

        public Color BodyColor
        {
            get => bodyColor;
            set
            {
                bodyColor = value;
                OnPropertyChanged(propertyName: nameof(BodyColor));
            }
        }

        public Color TitleColor
        {
            get => titleColor;
            set
            {
                titleColor = value;
                OnPropertyChanged(propertyName: nameof(TitleColor));
            }
        }

        public Color LabelsColor
        {
            get => labelsColor;
            set
            {
                labelsColor = value;
                OnPropertyChanged(propertyName: nameof(LabelsColor));
            }
        }


        [XmlArray("Shortcuts")]
        [XmlArrayItem(typeof(LnkShortcut))]
        [XmlArrayItem(typeof(UrlShortcut))]
        [XmlArrayItem(typeof(FileShortcut))]
        [XmlArrayItem(typeof(FolderShortcut))]
        public ObservableCollection<Shortcut> Shortcuts 
        { 
            get => shortcuts; 
            set 
            { 
                shortcuts = value;
                OnPropertyChanged(propertyName: nameof(Shortcuts));
            }
        }
    }
}
