using Palisades.ViewModel;
using System.Windows;
using System.Windows.Input;
namespace Palisades.View
{
    public partial class Palisade : Window
    {
        public Palisade()
        {
            InitializeComponent();
            Show();
        }
        private void DragMove(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
