using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Exceptions;
using PlayStation_App.Core.Managers;

namespace PlayStation_App.ViewModels
{
    public class LoginPageViewModel : NotifierBase
    {
        private readonly AuthenticationManager _authManager = new AuthenticationManager();
        private string _password;
        private string _userName;

        public LoginPageViewModel()
        {
            ClickLoginButtonCommand = new AsyncDelegateCommand(async o => { await ClickLoginButton(); },
                o => CanClickLoginButton);
        }

        public bool CanClickLoginButton => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName == value) return;
                _userName = value;
                OnPropertyChanged();
                ClickLoginButtonCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value) return;
                _password = value;
                OnPropertyChanged();
                ClickLoginButtonCommand.RaiseCanExecuteChanged();
            }
        }


        public AsyncDelegateCommand ClickLoginButtonCommand { get; private set; }
        public event EventHandler<EventArgs> LoginSuccessful;
        public event EventHandler<EventArgs> LoginFailed;

        public async Task ClickLoginButton()
        {
            var loginResult = new UserAccountEntity();
            IsLoading = true;
            try
            {
                loginResult = await _authManager.Authenticate(UserName, Password);
            }
            catch (Exception ex)
            {
            }
            IsLoading = false;
            base.RaiseEvent(loginResult != null ? LoginSuccessful : LoginFailed, EventArgs.Empty);
        }
    }
}
