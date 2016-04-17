using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation_App.Common;
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
using Template10.Services.NavigationService;

namespace PlayStation_Gui.ViewModels
{
    public class AccountViewModel : ViewModelBase
    {
        private readonly AuthenticationManager _authManager = new AuthenticationManager();
        private readonly UserManager _userManager = new UserManager();
        public readonly UserAccountDatabase AccountDatabase = new UserAccountDatabase(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                Set(ref _isLoading, value);
            }
        }

        public AccountViewModel()
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

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await base.OnNavigatedToAsync(parameter, mode, state);
            AccountUsers = new ObservableCollection<AccountUser>();
            var users = await AccountDatabase.GetUserAccounts();
            foreach (var user in users)
            {
                AccountUsers.Add(user);
            }
        }

        private ObservableCollection<AccountUser> _accountUsers;
        public ObservableCollection<AccountUser> AccountUsers
        {
            get { return _accountUsers; }
            set
            {
                Set(ref _accountUsers, value);
            }
        }

        public async Task CheckAndNavigateToMainShell(object sender, ItemClickEventArgs e)
        {
            if (IsLoading)
            {
                return;
            }
            var user = e.ClickedItem as AccountUser;
            if (user == null) return;
            var loginResult = false;
            loginResult = await LoginTest(user);
            if (loginResult)
            {
                if (Shell.Instance.ViewModel.CurrentUser != null)
                {
                    await AccountAuthHelpers.UpdateUserIsDefault(Shell.Instance.ViewModel.CurrentUser);
                }
                Shell.Instance.ViewModel.CurrentUser = user;
                Shell.Instance.ViewModel.IsLoggedIn = true;
                await AccountAuthHelpers.UpdateUserIsDefault(user, true);
                NavigationService.Navigate(typeof (MainPage));
                //new NavigateToWhatsNewPage().Execute(null);
            }
        }

        public async Task NavigateToLoginPage()
        {
            NavigationService.Navigate(typeof (LoginPage));
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
                    new UserAuthenticationEntity(tokenResult.AccessToken, tokenResult.RefreshToken, AccountAuthHelpers.GetUnixTime(DateTime.Now) + tokenResult.ExpiresIn),
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
                if (Shell.Instance.ViewModel.CurrentUser != null && Shell.Instance.ViewModel.CurrentUser.Username == user.Username)
                {
                    Shell.Instance.ViewModel.CurrentUser = null;
                    Shell.Instance.ViewModel.IsLoggedIn = false;
                    try
                    {
                        var pages = App.Frame.BackStack;
                        foreach (var page in pages)
                        {
                            App.Frame.BackStack.Remove(page);
                        }
                    }
                    catch (Exception)
                    {
                        // Failed to delete backstack :\
                    }
                }
                if (!AccountUsers.Any())
                {
                    NavigationService.Navigate(typeof(LoginPage));
                }
            }
            return resultCheck;
        }
    }
}
