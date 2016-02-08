using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using PlayStation.Entities.User;
using PlayStation_App.Database;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Tools.Debug;
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

        public async Task LoginDefaultUser()
        {
            string errorMessage;
            try
            {
                var defaultUsers = await _udb.GetDefaultUserAccounts();
                if (!defaultUsers.Any()) return;
                CurrentUser = defaultUsers.First();
                IsLoggedIn = true;
                return;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            // Failed to log in with default user, tell them.
            await ResultChecker.SendMessageDialogAsync(errorMessage, false);
        }
    }
}
