using Avalonia.Controls;

using Client.MVVM.ViewModels;

namespace Client.MVVM.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}