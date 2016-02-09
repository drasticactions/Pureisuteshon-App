using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation_App.Database;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.User;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_Gui.Tools.Database;
using PlayStation_Gui.Tools.Debug;
using PlayStation_Gui.Views;
using SQLite.Net.Platform.WinRT;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private bool _isLoggedIn = default(bool);
        private readonly AuthenticationManager _authManager = new AuthenticationManager();
        private readonly UserManager _userManager = new UserManager();
        private readonly UserAccountDatabase _udb = new UserAccountDatabase(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                Set(ref _isLoggedIn, value);
            }
        }

        private AccountUser _currentUser;

        public AccountUser CurrentUser
        {
            get { return _currentUser; }
            set
            {
                Set(ref _currentUser, value);
            }
        }

        public UserAuthenticationEntity CurrentTokens => new UserAuthenticationEntity(CurrentUser.AccessToken, CurrentUser.RefreshToken, CurrentUser.RefreshDate);

        public async Task<bool> LoginDefaultUser()
        {
            string errorMessage;
            try
            {
                var defaultUsers = await _udb.GetDefaultUserAccounts();
                if (!defaultUsers.Any()) return false;
                var defaultUser = defaultUsers.First();
                var loginResult = await LoginTest(defaultUser);
                if (loginResult)
                {
                    if (Shell.Instance.ViewModel.CurrentUser != null)
                    {
                        await AccountAuthHelpers.UpdateUserIsDefault(Shell.Instance.ViewModel.CurrentUser);
                    }
                    CurrentUser = defaultUser;
                    IsLoggedIn = true;
                    return true;
                    //new NavigateToWhatsNewPage().Execute(null);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            // Failed to log in with default user, tell them.
            await ResultChecker.SendMessageDialogAsync(errorMessage, false);
            return false;
        }


        private async Task<bool> LoginTest(AccountUser user)
        {
            Result result = new Result();
            try
            {
                result = await _authManager.RefreshAccessToken(user.RefreshToken);
                var tokenResult = JsonConvert.DeserializeObject<Tokens>(result.Tokens);
                result = await _userManager.GetUser(user.Username,
                    new UserAuthenticationEntity(tokenResult.AccessToken, tokenResult.RefreshToken, tokenResult.ExpiresIn),
                    user.Region, user.Language);
                var userResult = JsonConvert.DeserializeObject<User>(result.ResultJson);


                var didUpdate = await AccountAuthHelpers.UpdateUserAccount(user, tokenResult, null, userResult);
                result.IsSuccess = didUpdate;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error = ex.Message;
            }
            await ResultChecker.CheckSuccess(result);
            return result.IsSuccess;
        }
    }
}
