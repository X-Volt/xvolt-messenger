using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using AvRichTextBox;
using ReactiveUI;

using Client.MVVM.Models;

namespace Client.MVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        private FlowDocument _chatPage;
        private bool _chatPageVisible = false;
        private string _chatPageKey = "";
        private FlowDocument _chatMessage;

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

        public ReactiveCommand<string, Unit> ChatMessageForegroundCommand { get; set; }
        public ReactiveCommand<string, Unit> ChatMessageBackgroundCommand { get; set; }
        public ReactiveCommand<string, Unit> ChatMessageFontSizeCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ChatMessageBoldCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ChatMessageItalicCommand { get; set; }
        public ReactiveCommand<Unit, Unit> ChatMessageUnderlineCommand { get; set; }

        public ObservableCollection<SmileyModel> ChatMessageSmileys { get; set; }
        public ReactiveCommand<string, Unit> ChatMessageSmileyCommand { get; set; }

        public ReactiveCommand<Unit, Unit> ChatMessageSendCommand { get; set; }

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

        public void ChatMessageForeground(string color)
        {
            ChatMessage.ApplyForeground(color);
            _view.GetButton("ChatMessageForegroundBTN").Flyout!.Hide();
            ChatMessageFocus();
        }

        public void ChatMessageBackground(string color)
        {
            ChatMessage.ApplyBackground(color);
            _view.GetButton("ChatMessageBackgroundBTN").Flyout!.Hide();
            ChatMessageFocus();
        }

        public void ChatMessageFontSize(string size)
        {
            ChatMessage.ApplyFontSize(double.Parse(size));
            ChatMessageFocus();
        }

        public void ChatMessageBold()
        {
            ChatMessage.ToggleBold();
            ChatMessageFocus();
        }

        public void ChatMessageItalic()
        {
            ChatMessage.ToggleItalic();
            ChatMessageFocus();
        }

        public void ChatMessageUnderline()
        {
            ChatMessage.ToggleUnderline();
            ChatMessageFocus();
        }

        public void ChatMessageSmiley(string smiley)
        {
            var image = new Image();
            // image.Name = "smiley:" + smiley;
            image.Source = new Bitmap(SmileyModel.SourcePath + smiley);
            image.Height = 20; image.Width = 20;

            ChatMessage.InsertImage(image);
            _view.GetButton("SmileysBTN").Flyout!.Hide();
            ChatMessageFocus();
        }

        public ObservableCollection<SmileyModel> ChatMessageSmileyList()
        {
            return new ObservableCollection<SmileyModel>(
                new List<SmileyModel>
                {
                    new SmileyModel {
                        Name = "Angel",
                        Source = "icon_angel.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Arrow",
                        Source = "icon_arrow.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Big Grin",
                        Source = "icon_biggrin.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Clap",
                        Source = "icon_clap.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Confused",
                        Source = "icon_confused.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Cool",
                        Source = "icon_cool.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Crazy",
                        Source = "icon_crazy.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Cry",
                        Source = "icon_cry.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Eek",
                        Source = "icon_eek.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Eh",
                        Source = "icon_eh.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Evil",
                        Source = "icon_evil.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Exclaim",
                        Source = "icon_exclaim.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Geek",
                        Source = "icon_geek.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Idea",
                        Source = "icon_idea.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "LOL",
                        Source = "icon_lol.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "LOL No",
                        Source = "icon_lolno.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Mad",
                        Source = "icon_mad.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Mr. Green",
                        Source = "icon_mrgreen.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Neutral",
                        Source = "icon_neutral.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Problem",
                        Source = "icon_problem.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Question",
                        Source = "icon_question.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Razz",
                        Source = "icon_razz.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Red Face",
                        Source = "icon_redface.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Roll Eyes",
                        Source = "icon_rolleyes.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Sad",
                        Source = "icon_sad.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Shh",
                        Source = "icon_shh.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Shifty",
                        Source = "icon_shifty.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Sick",
                        Source = "icon_sick.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Silent",
                        Source = "icon_silent.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Smile",
                        Source = "icon_smile.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Surprised",
                        Source = "icon_surprised.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Think",
                        Source = "icon_think.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Thumb Down",
                        Source = "icon_thumbdown.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Thumb Up",
                        Source = "icon_thumbup.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Twisted",
                        Source = "icon_twisted.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Uber Geek",
                        Source = "icon_ugeek.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Wave",
                        Source = "icon_wave.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Wink",
                        Source = "icon_wink.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "WTF",
                        Source = "icon_wtf.png",
                        Command = ChatMessageSmileyCommand
                    },
                    new SmileyModel {
                        Name = "Yawn",
                        Source = "icon_yawn.png",
                        Command = ChatMessageSmileyCommand
                    }
                }
            );
        }

        public void ChatMessageSend()
        {
            if (ChatMessage.Blocks.Count > 0)
            {
                _server.SendMessageToUser(_chatPageKey, ChatMessage.SaveXaml());

                ChatMessage.NewDocument();
                ChatMessageFocus();
            }
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
    }
}
