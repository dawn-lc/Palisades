
using Palisades.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;
namespace Palisades.Model
{
    [XmlRoot(Namespace = "io.stouder", ElementName = "PalisadeModel")]
    public class PalisadeModel
    {
        private string identifier;
        private string name;
        private int fenceX;
        private int fenceY;
        private int width;
        private int height;
        private ObservableCollection<Shortcut> shortcuts;
        private Color headerColor;
        private Color bodyColor;
        private Color titleColor;
        private Color labelsColor;

        public PalisadeModel()
        {
            identifier = Guid.NewGuid().ToString();
            name = "Default";
            headerColor = Color.FromArgb(200, 0, 0, 0);
            bodyColor = Color.FromArgb(120, 0, 0, 0);
            titleColor = Color.FromArgb(255, 255, 255, 255);
            labelsColor = Color.FromArgb(255, 255, 255, 255);
            width = 800;
            height = 450;
            shortcuts = [];
        }

        public string Identifier { get { return identifier; } set { identifier = value; } }
        public string Name { get { return name; } set { name = value; } }

        public int FenceX { get { return fenceX; } set { fenceX = value; } }
        public int FenceY { get { return fenceY; } set { fenceY = value; } }

        public int Width { get { return width; } set { width = value; } }
        public int Height { get { return height; } set { height = value; } }

        public Color HeaderColor { get { return headerColor; } set { headerColor = value; } }
        public Color BodyColor { get { return bodyColor; } set { bodyColor = value; } }
        public Color TitleColor { get { return titleColor; } set { titleColor = value; } }
        public Color LabelsColor { get { return labelsColor; } set { labelsColor = value; } }

        [XmlArray("Shortcuts")]
        [XmlArrayItem(typeof(LnkShortcut))]
        [XmlArrayItem(typeof(UrlShortcut))]
        [XmlArrayItem(typeof(FileShortcut))]
        [XmlArrayItem(typeof(FolderShortcut))]
        public ObservableCollection<Shortcut> Shortcuts { get { return shortcuts; } set { shortcuts = value; } }
        public void Save(string? propertyName)
        {
            string saveDirectory = PDirectory.GetPalisadeDirectory(Identifier);
            PDirectory.EnsureExists(saveDirectory);

            using StreamWriter writer = new(Path.Combine(saveDirectory, "state.xml"));
            XmlSerializer serializer = new(
                typeof(PalisadeModel),
                [
                    typeof(Shortcut),
                    typeof(LnkShortcut),
                    typeof(UrlShortcut),
                    typeof(FileShortcut),
                    typeof(FolderShortcut)
                ]
            );

            serializer.Serialize(writer, this);
        }
    }
}
