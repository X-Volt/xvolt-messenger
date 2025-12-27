using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using AvRichTextBox;
using ReactiveUI;

using Client.MVVM.Models;
using Client.MVVM.Views;
using Client.Net;

namespace Client.MVVM.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private readonly IView _view;

        private readonly Server _server;
        private string _username = "";

        private bool _loginVisible = true;
        private string _loginUsername = "";
        private string _loginPassword = "";
        private string _loginError = "";

        private FlowDocument _chatPage;
        private string _chatPageKey = "";
        private FlowDocument _chatMessage;

        private bool _chatPageVisible = false;
        private bool _settingsPageVisible = false;
        private bool _newsPageVisible = false;

        public ReactiveCommand<Unit, Unit> LoginRegister { get; set; }
        public ReactiveCommand<Unit, Unit> LoginSignOn { get; set; }

        public bool LoginVisible
        {
            get => _loginVisible;
            set => this.RaiseAndSetIfChanged(ref _loginVisible, value);
        }

        public string LoginUsername
        {
            get => _loginUsername;
            set => this.RaiseAndSetIfChanged(ref _loginUsername, value);
        }

        public string LoginPassword
        {
            get => _loginPassword;
            set => this.RaiseAndSetIfChanged(ref _loginPassword, value);
        }

        public string LoginError
        {
            get => _loginError;
            set => this.RaiseAndSetIfChanged(ref _loginError, value);
        }

        public ObservableCollection<UserModel> ChatUsers { get; set; }
        public ReactiveCommand<string, Unit> ChatUserViewMessages { get; set; }

        public IDictionary<string, string> ChatPages = new Dictionary<string, string>();
        
        public FlowDocument ChatPage
        {
            get => _chatPage;
            set => this.RaiseAndSetIfChanged(ref _chatPage, value);
        }

        public bool ChatPageVisible
        {
            get => _chatPageVisible;
            set => this.RaiseAndSetIfChanged(ref _chatPageVisible, value);
        }

        public FlowDocument ChatMessage
        {
            get => _chatMessage;
            set => this.RaiseAndSetIfChanged(ref _chatMessage, value);
        }

        public ReactiveCommand<string, Unit> ChatMessageForeground { get; set; }
        public ReactiveCommand<string, Unit> ChatMessageBackground { get; set; }
        public ReactiveCommand<string, Unit> ChatMessageFontSize { get; set; }
        public ReactiveCommand<Unit, Unit> ChatMessageBold { get; set; }
        public ReactiveCommand<Unit, Unit> ChatMessageItalic { get; set; }
        public ReactiveCommand<Unit, Unit> ChatMessageUnderline { get; set; }

        public ObservableCollection<SmileyModel> ChatMessageSmileys { get; set; }
        public ReactiveCommand<string, Unit> ChatMessageSmiley { get; set; }
        
        public ReactiveCommand<Unit, Unit> ChatSend { get; set; }

        public ReactiveCommand<Unit, Unit> Settings { get; set; }
        public FlowDocument SettingsPage { get; set; }
        public bool SettingsPageVisible
        {
            get => _settingsPageVisible;
            set => this.RaiseAndSetIfChanged(ref _settingsPageVisible, value);
        }

        public ReactiveCommand<Unit, Unit> News { get; set; }
        public FlowDocument NewsPage { get; set; }
        public bool NewsPageVisible
        {
            get => _newsPageVisible;
            set => this.RaiseAndSetIfChanged(ref _newsPageVisible, value);
        }

        public ReactiveCommand<Unit, Unit> Artwork { get; set; }

        // a parameterless constructor is required
        // for the Avalonia XAML previewer to work
        #pragma warning disable CS8618
        public MainWindowViewModel() { }

        public MainWindowViewModel(IView view)
        {
            // TODO: separate code into partial classes
            // TODO: test the editor with unusual chars
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

            LoginRegister = ReactiveCommand.Create(() => {});

            LoginSignOn = ReactiveCommand.Create(() =>
            {
                LoginError = "";

                if (string.IsNullOrWhiteSpace(LoginUsername) || LoginUsername.Length <= 2)
                {
                    LoginError = "You have entered an invalid Screen Name";
                }
                else
                {
                    _server.SignOn(LoginUsername);
                }
            });

            ChatUsers = new ObservableCollection<UserModel>();
            ChatUserViewMessages = ReactiveCommand.Create<string>((username) =>
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
            });

            ChatMessage = new FlowDocument();

            ChatMessageForeground = ReactiveCommand.Create((string color) =>
            {
                ChatMessage.ApplyForeground(color);
                _view.GetButton("ChatMessageForegroundBTN").Flyout.Hide();
                ChatMessageFocus();
            });
            
            ChatMessageBackground = ReactiveCommand.Create((string color) =>
            {
                ChatMessage.ApplyBackground(color);
                _view.GetButton("ChatMessageBackgroundBTN").Flyout.Hide();
                ChatMessageFocus();
            });

            ChatMessageFontSize = ReactiveCommand.Create((string size) =>
            {
                ChatMessage.ApplyFontSize(double.Parse(size));
            });

            ChatMessageBold = ReactiveCommand.Create(() =>
            {
                ChatMessage.ToggleBold();
            });

            ChatMessageItalic = ReactiveCommand.Create(() =>
            {
                ChatMessage.ToggleItalic();
            });

            ChatMessageUnderline = ReactiveCommand.Create(() =>
            {
                ChatMessage.ToggleUnderline();
            });

            ChatMessageSmiley = ReactiveCommand.Create((string smiley) =>
            {
                var image = new Image();
                // image.Name = "smiley:" + smiley;
                image.Source = new Bitmap(SmileyModel.SourcePath + smiley);
                image.Height = 20; image.Width = 20;

                ChatMessage.InsertImage(image);
                _view.GetButton("SmileysBTN").Flyout.Hide();
                ChatMessageFocus();
            });

            ChatMessageSmileys = new ObservableCollection<SmileyModel>(
                new List<SmileyModel>
                {
                    new SmileyModel {
                        Name = "Angel", 
                        Source = "icon_angel.png", 
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Arrow",
                        Source = "icon_arrow.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Big Grin",
                        Source = "icon_biggrin.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Clap",
                        Source = "icon_clap.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Confused",
                        Source = "icon_confused.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Cool",
                        Source = "icon_cool.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Crazy",
                        Source = "icon_crazy.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Cry",
                        Source = "icon_cry.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Eek",
                        Source = "icon_eek.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Eh",
                        Source = "icon_eh.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Evil",
                        Source = "icon_evil.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Exclaim",
                        Source = "icon_exclaim.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Geek",
                        Source = "icon_geek.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Idea",
                        Source = "icon_idea.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "LOL",
                        Source = "icon_lol.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "LOL No",
                        Source = "icon_lolno.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Mad",
                        Source = "icon_mad.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Mr. Green",
                        Source = "icon_mrgreen.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Neutral",
                        Source = "icon_neutral.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Problem",
                        Source = "icon_problem.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Question",
                        Source = "icon_question.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Razz",
                        Source = "icon_razz.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Red Face",
                        Source = "icon_redface.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Roll Eyes",
                        Source = "icon_rolleyes.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Sad",
                        Source = "icon_sad.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Shh",
                        Source = "icon_shh.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Shifty",
                        Source = "icon_shifty.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Sick",
                        Source = "icon_sick.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Silent",
                        Source = "icon_silent.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Smile",
                        Source = "icon_smile.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Surprised",
                        Source = "icon_surprised.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Think",
                        Source = "icon_think.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Thumb Down",
                        Source = "icon_thumbdown.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Thumb Up",
                        Source = "icon_thumbup.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Twisted",
                        Source = "icon_twisted.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Uber Geek",
                        Source = "icon_ugeek.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Wave",
                        Source = "icon_wave.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Wink",
                        Source = "icon_wink.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "WTF",
                        Source = "icon_wtf.png",
                        Command = ChatMessageSmiley
                    },
                    new SmileyModel {
                        Name = "Yawn",
                        Source = "icon_yawn.png",
                        Command = ChatMessageSmiley
                    }
                }
            );

            ChatSend = ReactiveCommand.Create(() =>
            {
                if (ChatMessage.Blocks.Count > 0)
                {
                    _server.SendMessageToUser(_chatPageKey, ChatMessage.SaveXaml());

                    ChatMessage.NewDocument();
                    ChatMessageFocus();
                }
            });

            SettingsPage = InitPage("Settings");
            SettingsPageVisible = false;
            Settings = ReactiveCommand.Create(() => 
            {
                ChatPageVisible = false;
                NewsPageVisible = false;
                SettingsPageVisible = true;
            });

            NewsPage = InitPage("Latest News");
            NewsPageVisible = true;
            News = ReactiveCommand.Create(() =>
            {
                ChatPageVisible = false;
                SettingsPageVisible = false;
                NewsPageVisible = true;
            });

            Artwork = ReactiveCommand.Create(() => {});
        }

        static private FlowDocument InitPage(string title = "")
        {
            var page = new FlowDocument();
            page.IsEditable = false;
            page.ClearDocument();

            // padding must be applied after it is cleared
            page.PagePadding = new(10, 0, 0, 10);

            if (!String.IsNullOrEmpty(title))
            {
                // heading
                var pageRun = new EditableRun(title);
                pageRun.FontSize = 20;
                pageRun.FontWeight = Avalonia.Media.FontWeight.Bold;

                var pagePar = new Paragraph();
                pagePar.Inlines.Add(pageRun);

                page.Blocks.Add(pagePar);
                page.Blocks.Add(new Paragraph());
            }
            else
            {
                page.Blocks.Add(new Paragraph());
            }

            return page;
        }

        private string InitChatPage(string username)
        {
            FlowDocument page;
            
            if (username == _username)
            {
                page = InitPage("My Private Notes");
            }
            else
            {
                page = InitPage($"Message History for {username}");
            }
                
            var pageXaml = page.SaveXaml();

            ChatPages.Add(username, pageXaml);

            return pageXaml;
        }

        async internal void ChatMessageFocus()
        {
            // wait until the RichTextBox has initialized
            // it must be longer than 70ms
            await Task.Delay(150);

            // set the focus by double clicking the control
            _view.GetRichTextBox("ChatMessageRTB").Focus(NavigationMethod.Pointer);
            _view.GetRichTextBox("ChatMessageRTB").Focus(NavigationMethod.Pointer);
        }

        private void UserConnected()
        {
            if (_server.PacketReader != null)
            {
                var user = new UserModel
                {
                    UID = _server.PacketReader.ReadMessage(),
                    Username = _server.PacketReader.ReadMessage(),
                    Command = ChatUserViewMessages
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