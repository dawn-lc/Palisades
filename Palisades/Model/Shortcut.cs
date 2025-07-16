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
    [XmlInclude(typeof(FileShortcut))]
    [XmlInclude(typeof(FolderShortcut))]
    public abstract class Shortcut
    {
        protected Shortcut()
        {
        }

        protected Shortcut(string name, string iconPath, string uriOrFileAction)
        {
            Name = name;
            IconPath = iconPath;
            UriOrFileAction = uriOrFileAction;
        }

        public string Name { get; set; } = string.Empty;

        public string Identifier { get; set; } = string.Empty;

        public string IconPath { get; set; } = string.Empty;

        public string UriOrFileAction { get; set; } = string.Empty;

        public static string GetName(string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }

        public static string GetIcon(string filename, string palisadeIdentifier)
        {
            using Bitmap? icon = IconExtractor.GetFileImageFromPath(filename, Helpers.Native.IconSizeEnum.LargeIcon48) ?? throw new InvalidOperationException("Could not extract icon from file: " + filename);
            string iconDir = Path.Combine(PalisadesManager.PalisadesPath, palisadeIdentifier);
            string iconFilename = Guid.NewGuid().ToString() + ".png";
            string iconPath = Path.Combine(iconDir, iconFilename);
            using FileStream fileStream = new(iconPath, FileMode.Create);
            icon.Save(fileStream, ImageFormat.Png);

            return iconPath;
        }

        /// <summary>
        /// 抽象的执行操作，比如打开文件、打开链接等
        /// </summary>
        public abstract void Execute();
    }
}
