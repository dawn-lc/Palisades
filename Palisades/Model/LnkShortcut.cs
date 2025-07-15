using System;
using System.Diagnostics;

namespace Palisades.Model
{
    public class LnkShortcut : Shortcut
    {
        public LnkShortcut() : base()
        {
        }

        public LnkShortcut(string name, string iconPath, string uriOrFileAction)
            : base(name, iconPath, uriOrFileAction)
        {
        }

        public static LnkShortcut? BuildFrom(string shortcutPath, string palisadeIdentifier)
        {
            try
            {
                IWshRuntimeLibrary.WshShell shell = new();
                IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);

                string name = GetName(shortcutPath);
                string iconPath = GetIcon(shortcutPath, palisadeIdentifier);

                return new LnkShortcut(name, iconPath, link.TargetPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LnkShortcut.BuildFrom] 失败: {ex.Message}");
                return null;
            }
        }

        public override void Execute()
        {
            if (!string.IsNullOrEmpty(UriOrFileAction))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(UriOrFileAction) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"打开快捷方式失败: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("快捷方式目标路径为空");
            }
        }
    }
}
