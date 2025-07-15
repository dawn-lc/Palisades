using System;
using System.Diagnostics;
using System.IO;

namespace Palisades.Model
{
    public class FolderShortcut : Shortcut
    {
        public FolderShortcut() : base()
        {
        }

        public FolderShortcut(string folderPath, string identifier) : base(GetName(folderPath), GetIcon(folderPath, identifier), folderPath)
        {
            FolderPath = folderPath;
            Identifier = identifier;
        }

        public string FolderPath { get; set; } = string.Empty;

        public static FolderShortcut BuildFrom(string path, string identifier)
        {
            return new FolderShortcut(path, identifier);
        }

        public override void Execute()
        {
            if (Directory.Exists(FolderPath))
            {
                Process.Start(new ProcessStartInfo("explorer.exe", FolderPath) { UseShellExecute = true });
            }
            else
            {
                throw new DirectoryNotFoundException("文件夹不存在：" + FolderPath);
            }
        }
    }
}
