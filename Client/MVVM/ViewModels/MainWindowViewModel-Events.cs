using System;
using System.Linq;
using System.Reactive;

using Avalonia.Threading;
using AvRichTextBox;
using ReactiveUI;

using Client.MVVM.Models;
using Client.Net;

namespace Client.MVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        private readonly Server _server;
        private string _username = "";

        private void UserConnected()
        {
            if (_server.PacketReader != null)
            {
                var user = new UserModel
                {
                    UID = _server.PacketReader.ReadMessage(),
                    Username = _server.PacketReader.ReadMessage(),
                    Command = ChatUserViewMessagesCommand
                };

                if (ChatUsers.Where(x => x.UID == user.UID).FirstOrDefault() == null)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        ChatUsers.Add(user);

                        if (user.Username == LoginUsername)
                        {
                            _username = user.Username;
                            _chatPageKey = _username;

                            LoginUsername = "";
                            LoginVisible = false;

                            NewsPageVisible = false;
                            SettingsPageVisible = false;

                            ChatPage.LoadXaml(InitChatPage(_username));
                            ChatPageVisible = true;

                            ChatMessage.NewDocument();
                            ChatMessageFocus();
                        }
                    });
                }
            }
        }

        private void UserDisconnected()
        {
            if (_server.PacketReader != null)
            {
                var uid = _server.PacketReader.ReadMessage();
                var username = _server.PacketReader.ReadMessage();

                var user = ChatUsers.Where(x => x.UID == uid).FirstOrDefault();

                Dispatcher.UIThread.Invoke(() =>
                {
                    if (user != null)
                    {
                        ChatUsers.Remove(user);
                    }

                    string? pageXaml;

                    if (ChatPages.TryGetValue(username, out pageXaml))
                    {
                        var pageToUpdate = InitPage();
                        pageToUpdate.LoadXaml(pageXaml);

                        var messageParagraph = new Paragraph();
                        var messageDisconnected = $"[{DateTime.Now}]: [{username}]: Disconnected";
                        messageParagraph.Inlines.Add(new EditableRun(messageDisconnected));

                        pageToUpdate.Blocks.Add(messageParagraph);

                        ChatPages[username] = pageToUpdate.SaveXaml();

                        if (username == _chatPageKey)
                        {
                            ChatPage.LoadXaml(ChatPages[username]);
                        }
                    }
                });
            }
        }

        private void MessageReceived()
        {
            if (_server.PacketReader != null)
            {
                var uid = _server.PacketReader.ReadMessage();
                var usernameFrom = _server.PacketReader.ReadMessage();
                var usernameTo = _server.PacketReader.ReadMessage();
                var message = _server.PacketReader.ReadMessage();

                var usernamePage = usernameFrom;

                if (usernameFrom == _username)
                {
                    usernamePage = usernameTo;
                }

                Dispatcher.UIThread.Invoke(() =>
                {
                    string? pageXaml;

                    if (!ChatPages.TryGetValue(usernamePage, out pageXaml))
                    {
                        pageXaml = InitChatPage(usernamePage);
                    }

                    var pageToUpdate = InitPage();
                    pageToUpdate.LoadXaml(pageXaml);

                    var messageParagraph = new Paragraph();
                    var messageTimestamp = $"[{DateTime.Now}]: [{usernameFrom}]:";
                    messageParagraph.Inlines.Add(new EditableRun(messageTimestamp));

                    pageToUpdate.Blocks.Add(messageParagraph);

                    var messageFlow = InitPage();
                    messageFlow.LoadXaml(message);

                    foreach (var block in messageFlow.Blocks)
                    {
                        pageToUpdate.Blocks.Add(block);
                    }

                    ChatPages[usernamePage] = pageToUpdate.SaveXaml();

                    if (usernamePage == _chatPageKey)
                    {
                        ChatPage.LoadXaml(ChatPages[usernamePage]);
                    }
                });
            }
        }
    }
}
