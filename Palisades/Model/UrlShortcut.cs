using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Palisades.Model
{
    public class UrlShortcut : Shortcut
    {
        public UrlShortcut() : base()
        {
        }

        public UrlShortcut(string name, string iconPath, string uriOrFileAction)
            : base(name, iconPath, uriOrFileAction)
        {
        }

        public static UrlShortcut? BuildFrom(string shortcutPath, string palisadeIdentifier)
        {
            try
            {
                // 读取 .url 文件，查找以 "URL=" 开头的行
                string? line = File.ReadLines(shortcutPath).FirstOrDefault(value => value.StartsWith("URL="));
                if (line == null)
                {
                    return null;
                }

                // 处理 URL 字符串
                string url = line["URL=".Length..].Trim().Replace("\"", "").Replace("BASE", "");

                string name = GetName(shortcutPath);
                string iconPath = GetIcon(shortcutPath, palisadeIdentifier);

                return new UrlShortcut(name, iconPath, url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UrlShortcut.BuildFrom] 失败: {ex.Message}");
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
                    Console.WriteLine($"打开 URL 失败: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("URL 为空");
            }
        }
    }
}
