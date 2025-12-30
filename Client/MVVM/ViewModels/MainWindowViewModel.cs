using System.Collections.ObjectModel;

using AvRichTextBox;
using ReactiveUI;

using Client.MVVM.Models;
using Client.MVVM.Views;
using Client.Net;

namespace Client.MVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IView _view;

        // a parameterless constructor is required
        // for the Avalonia XAML previewer to work
        #pragma warning disable CS8618
        public MainWindowViewModel() { }

        public MainWindowViewModel(IView view)
        {
            // TODO: get the chat page messages formatted properly
            // TODO: get login / register working
            // TODO: get add / group buddies working
            // TODO: get group chats working

            _view = view;

            _server = new Server();
            _server.ConnectEvent += UserConnected;
            _server.DisconnectEvent += UserDisconnected;
            _server.MessageReceivedEvent += MessageReceived;

            _chatPage = InitPage();

            LoginRegisterCommand = ReactiveCommand.Create(LoginRegister);
            LoginSignOnCommand = ReactiveCommand.Create(LoginSignOn);

            ChatUsers = new ObservableCollection<UserModel>();
            ChatUserViewMessagesCommand = ReactiveCommand.Create<string>(ChatUserViewMessages);

            ChatMessage = new FlowDocument();

            ChatMessageForegroundCommand = ReactiveCommand.Create<string>(ChatMessageForeground);
            ChatMessageBackgroundCommand = ReactiveCommand.Create<string>(ChatMessageBackground);
            ChatMessageFontSizeCommand = ReactiveCommand.Create<string>(ChatMessageFontSize);

            ChatMessageBoldCommand = ReactiveCommand.Create(ChatMessageBold);
            ChatMessageItalicCommand = ReactiveCommand.Create(ChatMessageItalic);
            ChatMessageUnderlineCommand = ReactiveCommand.Create(ChatMessageUnderline);

            ChatMessageSmileyCommand = ReactiveCommand.Create<string>(ChatMessageSmiley);
            ChatMessageSmileys = ChatMessageSmileyList();

            ChatMessageSendCommand = ReactiveCommand.Create(ChatMessageSend);

            SettingsPage = InitPage("Settings");
            SettingsCommand = ReactiveCommand.Create(Settings);

            NewsPage = InitPage("Latest News");
            NewsCommand = ReactiveCommand.Create(News);

            ArtworkCommand = ReactiveCommand.Create(Artwork);
        }
    }
}