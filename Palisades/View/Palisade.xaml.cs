using Palisades.ViewModel;
using System.Windows;
namespace Palisades.View
{
    public partial class Palisade : Window
    {
        private readonly PalisadeViewModel viewModel;

        public Palisade(PalisadeViewModel defaultModel)
        {
            InitializeComponent();
            DataContext = defaultModel;
            viewModel = defaultModel;
            Show();
        }

        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}
