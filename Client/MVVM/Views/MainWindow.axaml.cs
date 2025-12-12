using Avalonia.Controls;
using AvRichTextBox;

using Client.MVVM.ViewModels;

namespace Client.MVVM.Views
{
    public interface IView
    {
        RichTextBox GetChatMessageRTB();
    }

    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel(this);

            InitializeComponent();
        }

        public RichTextBox GetChatMessageRTB()
        {
            return ChatMessageRTB;
        }
    }
}