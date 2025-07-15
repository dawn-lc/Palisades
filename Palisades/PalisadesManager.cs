using Palisades.Helpers;
using Palisades.Model;
using Palisades.View;
using Palisades.ViewModel;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace Palisades
{
    internal static class PalisadesManager
    {
        public static readonly Dictionary<string, Palisade> palisades = [];

        public static void LoadPalisades()
        {
            string saveDirectory = PDirectory.GetPalisadesDirectory();
            PDirectory.EnsureExists(saveDirectory);

            foreach (string identifierDirname in Directory.GetDirectories(saveDirectory))
            {
                string modelPath = Path.Combine(identifierDirname, "state.xml");
                try
                {
                    PalisadeModel model;
                    XmlSerializer deserializer = new(typeof(PalisadeModel));
                    using StreamReader reader = new(modelPath);
                    model = deserializer.Deserialize(reader) as PalisadeModel ?? new PalisadeModel();
                    if (!palisades.ContainsKey(model.Identifier))
                    {
                        palisades[model.Identifier] = new Palisade(new PalisadeViewModel(model));
                    }
                }
                catch
                {
                    File.Delete(modelPath);
                    continue;
                }
            }
        }

        public static void CreatePalisade()
        {
            var viewModel = new PalisadeViewModel();
            var palisade = new Palisade(viewModel);

            palisades[viewModel.Identifier] = palisade;
            viewModel.Save();
        }

        public static void DeletePalisade(string identifier)
        {
            // 检查是否为最后一个Palisade
            if (palisades.Count <= 1)
            {
                MessageBox.Show("不能删除最后一个Palisade！", "操作被拒绝", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!palisades.TryGetValue(identifier, out Palisade? palisade) || palisade == null)
                return;

            if (palisade.DataContext is PalisadeViewModel viewModel)
                viewModel.Delete();

            palisade.Close();
            palisades.Remove(identifier);
        }

        public static Palisade GetPalisade(string identifier)
        {
            if (palisades.TryGetValue(identifier, out Palisade? palisade) && palisade != null)
                return palisade;

            throw new KeyNotFoundException($"未找到标识符为 '{identifier}' 的 Palisade。");
        }
    }
}
