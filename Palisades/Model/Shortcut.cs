using Palisades.Helpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;

namespace Palisades.Model
{
    [XmlInclude(typeof(LnkShortcut))]
    [XmlInclude(typeof(UrlShortcut))]
    public abstract class Shortcut(string name, string iconPath, string uriOrFileAction)
    {
        public Shortcut() : this("", "", "")
        {

        }

        public string Name { get { return name; } set { name = value; } }

        public string IconPath { get { return iconPath; } set { iconPath = value; } }
        public string UriOrFileAction { get { return uriOrFileAction; } set { uriOrFileAction = value; } }


        public static string GetName(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

        public static string GetIcon(string filename, string palisadeIdentifier)
        {
            using Bitmap? icon = IconExtractor.GetFileImageFromPath(filename, Helpers.Native.IconSizeEnum.LargeIcon48) ?? throw new InvalidOperationException("Could not extract icon from file: " + filename);
            string iconDir = PDirectory.GetPalisadeIconsDirectory(palisadeIdentifier);
            PDirectory.EnsureExists(iconDir);

            string iconFilename = Guid.NewGuid().ToString() + ".png";
            string iconPath = Path.Combine(iconDir, iconFilename);
            using FileStream fileStream = new(iconPath, FileMode.Create);
            icon.Save(fileStream, ImageFormat.Png);

            return iconPath;
        }
    }
}
