
using Palisades.Model;
using Palisades.ViewModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Palisades
{


    internal static class PalisadesManager
    {
        public class PalisadeHandle
        {
            public required Palisade Config { get; set; }
            public required View.Palisade View { get; set; }
            public required PalisadeViewModel Model { get; set; }
        }
        public static readonly string PalisadesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Palisades", "data");
        public static readonly Palisade defaultConfig = new();

        public static readonly ConcurrentDictionary<string, PalisadeHandle> Palisades = [];

        static PalisadesManager()
        {
            Directory.CreateDirectory(PalisadesPath);
        }

        public static void SavePalisade(string identifier)
        {
            if (!Palisades.TryGetValue(identifier, out PalisadeHandle? handle) || handle == null)
            {
                return;
            }
            try
            {
                XmlSerializer serializer = new(typeof(Palisade));
                using StreamWriter writer = new(Path.Combine(PalisadesPath, $"{identifier}.xml"));
                serializer.Serialize(writer, handle.Config);
            }
            catch (Exception ex)
            {
                throw new Exception($"保存 Palisade '{handle.Config.Name}' 时出错: {ex.Message}");
            }
        }
        public static void LoadPalisades()
        {

            foreach (var file in Directory.GetFiles(PalisadesPath))
            {
                try
                {
                    Palisade config;
                    XmlSerializer deserializer = new(typeof(Palisade));
                    using (FileStream fs = new(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    config = deserializer.Deserialize(fs) as Palisade ?? new Palisade();
                    if (!Palisades.ContainsKey(config.Identifier))
                    {
                        CreatePalisade(config);
                    }
                }
                catch
                {
                    File.Delete(file);
                    continue;
                }
            }

            if (Palisades.IsEmpty)
            {
                CreatePalisade();
            }
        }

        public static void CreatePalisade(Palisade? config = null)
        {
            config ??= new Palisade();
            var viewModel = new PalisadeViewModel(config);
            Palisades.AddOrUpdate(config.Identifier, new PalisadeHandle()
            {
                Config = config,
                Model = viewModel,
                View = new View.Palisade() { DataContext = viewModel }
            }, (key, existing) =>
            {
                existing.Config = config;
                existing.Model = viewModel;
                existing.View.DataContext = viewModel;
                return existing;
            });
            Directory.CreateDirectory(Path.Combine(PalisadesPath, viewModel.Identifier));
            SavePalisade(config.Identifier);
        }

        public static void DeletePalisade(string identifier)
        {
            if (Palisades.Count <= 1)
            {
                MessageBox.Show("不能删除最后一个Palisade！", "操作被拒绝", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!Palisades.TryGetValue(identifier, out PalisadeHandle? handle) || handle == null)
            {
                return;
            }
            File.Delete(Path.Combine(PalisadesPath, $"{handle.Config.Identifier}.xml"));
            Directory.Delete(Path.Combine(PalisadesPath, handle.Config.Identifier), recursive: true);
            handle.View.Close();
            Palisades.TryRemove(identifier, out _);
        }

        public static View.Palisade GetPalisade(string identifier)
        {
            if (Palisades.TryGetValue(identifier, out PalisadeHandle? handle) && handle != null)
            {
                return handle.View;
            }
            throw new KeyNotFoundException($"未找到标识符为 '{identifier}' 的 Palisade。");
        }
    }
}
