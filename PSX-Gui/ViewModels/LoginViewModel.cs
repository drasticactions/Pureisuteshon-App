using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation.Tools;
using PlayStation_App.Common;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.User;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_Gui.Views;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthenticationManager _authManager = new AuthenticationManager();
        private readonly UserManager _userManager = new UserManager();
        private string _password;
        private string _userName;
        public LoginViewModel()
        {
            ClickLoginButtonCommand = new AsyncDelegateCommand(async o => { await ClickLoginButton(); },
                o => CanClickLoginButton);
        }

        public bool CanClickLoginButton => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password);

        private bool _isLoading;


        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                Set(ref _isLoading, value);
            }
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (_userName == value) return;
                Set(ref _userName, value);
                ClickLoginButtonCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password == value) return;
                Set(ref _password, value);
                ClickLoginButtonCommand.RaiseCanExecuteChanged();
            }
        }


        public AsyncDelegateCommand ClickLoginButtonCommand { get; private set; }

        public async Task ClickLoginButton()
        {
            await Login();
        }

        public async Task Login()
        {
            Result loginResult = new Result();
            IsLoading = true;
            try
            {
                loginResult = await _authManager.SendLoginData(UserName, Password);
            }
            catch (Exception ex)
            {
                loginResult.IsSuccess = false;
                loginResult.ResultJson = ex.Message;
            }
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            if (!loginResult.IsSuccess)
            {
                loginResult.ResultJson = loader.GetString("LoginError/Text");
            }
            else if (loginResult.IsSuccess)
            {
                try
                {
                    var authTokens = JsonConvert.DeserializeObject<Tokens>(loginResult.Tokens);
                    var expiresInDate = AuthHelpers.GetUnixTime(DateTime.Now) + authTokens.ExpiresIn;
                    if (!string.IsNullOrEmpty(authTokens.AccessToken) && !string.IsNullOrEmpty(authTokens.RefreshToken))
                    {
                        var loginUserResult =
                            await
                                _authManager.GetUserEntity(new UserAuthenticationEntity(authTokens.AccessToken, authTokens.RefreshToken, expiresInDate), "ja");
                        var loginUser = JsonConvert.DeserializeObject<LogInUser>(loginUserResult.ResultJson);
                        var userResult =
                            await
                                _userManager.GetUser(loginUser.OnlineId,
                                    new UserAuthenticationEntity(authTokens.AccessToken, authTokens.RefreshToken,
                                        expiresInDate), loginUser.Region, loginUser.Language);
                        var user = JsonConvert.DeserializeObject<User>(userResult.ResultJson);
                        var newAccountResult = await AccountAuthHelpers.CreateUserAccount(authTokens, loginUser, user);
                        if (!newAccountResult)
                        {
                            loginResult.IsSuccess = false;
                            loginResult.Error = "Failed to create new user in database.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    loginResult.IsSuccess = false;
                    loginResult.ResultJson = ex.Message;
                }
            }

            // Check if the result was good. If not, show error.
            await ResultChecker.CheckSuccess(loginResult);
            IsLoading = false;
            if (loginResult.IsSuccess)
            {
                
                NavigationService.Navigate(typeof (AccountPage));
            }
        }
    }
}
