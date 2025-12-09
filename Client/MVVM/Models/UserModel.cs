using ReactiveUI;
using System.Reactive;

namespace Client.MVVM.Models
{
    internal class UserModel
    {
        public required string UID { get; set; }
        public required string Username { get; set; }
        public ReactiveCommand<string, Unit>? Command { get; set; }
    }
}
