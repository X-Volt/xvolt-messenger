using System.Reactive;

using ReactiveUI;

namespace Client.MVVM.Models
{
    internal class UserModel
    {
        public required string GUID { get; set; }
        public required string Name { get; set; }
        public ReactiveCommand<string, Unit>? Command { get; set; }
    }
}
