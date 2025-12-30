using System.Reactive;

using ReactiveUI;

namespace Client.MVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        private bool _loginVisible = true;
        private string _loginUsername = "";
        private string _loginPassword = "";
        private string _loginError = "";

        public ReactiveCommand<Unit, Unit> LoginRegisterCommand { get; set; }
        public ReactiveCommand<Unit, Unit> LoginSignOnCommand { get; set; }

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

        public void LoginRegister()
        {
            // TODO - Create a new account
        }

        public void LoginSignOn()
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
        }
    }
}
