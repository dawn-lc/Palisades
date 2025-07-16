using Palisades.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Palisades.View
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }
        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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