using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation_App.Commands.Navigation;
using PlayStation_App.Commands.SelectAccount;
using PlayStation_App.Common;
using PlayStation_App.Database;
using PlayStation_App.Models;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.User;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_App.Views;
using PlayStation_App.Views.Account;


namespace PlayStation_App.ViewModels
{
    public class AccountSelectScreenViewModel : NotifierBase
    {
        private readonly AuthenticationManager _authManager = new AuthenticationManager();
        private readonly UserManager _userManager = new UserManager();
        public readonly UserAccountDatabase AccountDatabase = new UserAccountDatabase();
        public DeleteAccountCommand DeleteAccountCommand { get; set; } = new DeleteAccountCommand();
        public CheckAndNavigateToMainShellCommand CheckAndNavigateToMainShellCommand { get; set; } = new CheckAndNavigateToMainShellCommand();
        public AccountSelectScreenViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                AccountUsers = new ObservableCollection<AccountUser>()
            {
                new AccountUser()
                {
                    AvatarUrl = "http://static-resource.np.community.playstation.net/avatar_xl/WWS_J/JP90031402F04_A779218F638383DA3908_XL.png",
                    Username = "DrasticOverload"
                },
                new AccountUser()
                {
                    AvatarUrl = "http://static-resource.np.community.playstation.net/avatar_xl/WWS_J/JP90031402F04_A779218F638383DA3908_XL.png",
                    Username = "DrasticOverload"
                }
            };
            }
        }

        public async Task<bool> Initialize()
        {
            AccountUsers = new ObservableCollection<AccountUser>();
            var users = await AccountDatabase.GetUserAccounts();
            foreach (var user in users)
            {
                AccountUsers.Add(user);
            }
            return AccountUsers.Any();
        }

        private ObservableCollection<AccountUser> _accountUsers;
        public ObservableCollection<AccountUser> AccountUsers
        {
            get { return _accountUsers; }
            set
            {
                SetProperty(ref _accountUsers, value);
                OnPropertyChanged();
            }
        }

        public async Task CheckAndNavigateToMainShell(AccountUser user)
        {
            if (IsLoading)
            {
                return;
            }
            var loginResult = false;
            loginResult = await LoginTest(user);
            if (loginResult)
            {
                Locator.ViewModels.MainPageVm.CurrentUser = user;
                Locator.ViewModels.MainPageVm.IsLoggedIn = true;
                Locator.ViewModels.MainPageVm.PopulateMenu();
                new NavigateToWhatsNewPage().Execute(null);
            }
        }

        private async Task<bool> LoginTest(AccountUser user)
        {
            IsLoading = true;
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
            IsLoading = false;
            return result.IsSuccess;
        }

        public async Task<bool> DeleteUserAccount(AccountUser user)
        {
            Locator.ViewModels.MainPageVm.MenuItems = new List<MenuItem>();
            try
            {
                var pages = App.RootFrame.BackStack;
                foreach (var page in pages)
                {
                    App.RootFrame.BackStack.Remove(page);
                }
            }
            catch (Exception)
            {
                // Failed to delete backstack :\
            }
            var result = new Result();
            try
            {
                result.IsSuccess = await AccountAuthHelpers.DeleteUserAccount(user);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error = "Failed to delete user";
            }
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (resultCheck)
            {
                AccountUsers.Remove(user);
                if (AccountUsers.Any())
                {
                    OnPropertyChanged("AccountUsers");
                    return true;
                }
                App.RootFrame.Navigate(typeof (LoginPage));
                
            }
            return resultCheck;
        }
    }
}
