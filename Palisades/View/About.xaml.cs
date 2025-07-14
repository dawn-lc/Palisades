using Palisades.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Palisades.View
{
    /// <summary>
    /// Logique d'interaction pour About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is (string code, string _))
            {
                Loc.SetLanguage(code);
            }
        }
    }
}