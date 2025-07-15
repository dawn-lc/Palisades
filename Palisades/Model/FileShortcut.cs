using System;
using System.Diagnostics;
using System.IO;

namespace Palisades.Model
{
    public class FileShortcut : Shortcut
    {
        public FileShortcut() : base()
        {
        }

        public FileShortcut(string filePath, string identifier) : base(GetName(filePath), GetIcon(filePath, identifier), filePath)
        {
            FilePath = filePath;
            Identifier = identifier;
        }

        public string FilePath { get; set; } = string.Empty;

        public static FileShortcut BuildFrom(string path, string identifier)
        {
            return new FileShortcut(path, identifier);
        }

        public override void Execute()
        {
            if (File.Exists(FilePath))
            {
                Process.Start(new ProcessStartInfo(FilePath) { UseShellExecute = true });
            }
            else
            {
                throw new FileNotFoundException("文件不存在", FilePath);
            }
        }
    }
}