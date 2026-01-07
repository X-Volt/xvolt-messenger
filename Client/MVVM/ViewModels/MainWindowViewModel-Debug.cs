using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Client.MVVM.Models;

namespace Client.MVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        public void DebugApp()
        {
            // DebugChatUsers();
        }

        public void DebugChatUsers()
        {
            ChatUsers = new ObservableCollection<UserModel>(
                new List<UserModel>
                {
                    new UserModel {
                        GUID = Guid.NewGuid().ToString(),
                        Name = "John Doe",
                        Command = ChatUserViewMessagesCommand
                    },
                    new UserModel {
                        GUID = Guid.NewGuid().ToString(),
                        Name = "Jane Doe",
                        Command = ChatUserViewMessagesCommand
                    },
                    new UserModel {
                        GUID = Guid.NewGuid().ToString(),
                        Name = "Fido",
                        Command = ChatUserViewMessagesCommand
                    }
                }
            );

            ChatGroups[0].Users = ChatUsers;

            LoginVisible = false;
            ChatPageVisible = true;
        }
    }
}