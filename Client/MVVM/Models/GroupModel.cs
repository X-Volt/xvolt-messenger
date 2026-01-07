using System.Collections.ObjectModel;
using System.Reactive;

using ReactiveUI;

namespace Client.MVVM.Models
{
    internal class GroupModel
    {
        public required string Name { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ReactiveCommand<string, Unit>? Command { get; set; }

        public GroupModel()
        {
            Users = new ObservableCollection<UserModel>();
        }
    }
}
