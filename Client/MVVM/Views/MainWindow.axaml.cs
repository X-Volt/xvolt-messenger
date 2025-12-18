using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvRichTextBox;

using Client.MVVM.ViewModels;

namespace Client.MVVM.Views
{
    public interface IView
    {
        Button GetButton(string name);
        RichTextBox GetRichTextBox(string name);
        TextBox GetTextBox(string name);
    }

    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(this);

            InitializeComponent();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            LoginUsernameTB.Focus(NavigationMethod.Pointer);
            LoginUsernameTB.Focus(NavigationMethod.Pointer);
        }

        public RichTextBox GetRichTextBox(string name)
        {
            return this.GetControl<RichTextBox>(name);
        }

        public TextBox GetTextBox(string name)
        {
            return this.GetControl<TextBox>(name);
        }

        public Button GetButton(string name)
        {
            return this.GetControl<Button>(name);
        }
    }
}