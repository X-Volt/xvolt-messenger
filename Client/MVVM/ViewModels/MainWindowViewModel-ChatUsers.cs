using System.Collections.ObjectModel;
using System.Reactive;

using ReactiveUI;

using Client.MVVM.Models;

namespace Client.MVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<UserModel> ChatUsers { get; set; }
        public ReactiveCommand<string, Unit> ChatUserViewMessagesCommand { get; set; }

        public void ChatUserViewMessages(string username)
        {
            NewsPageVisible = false;
            SettingsPageVisible = false;

            string? pageXaml;

            if (!ChatPages.TryGetValue(username, out pageXaml))
            {
                pageXaml = InitChatPage(username);
            }

            _chatPageKey = username;
            ChatPage.LoadXaml(pageXaml);
            ChatPageVisible = true;
        }
    }
}
