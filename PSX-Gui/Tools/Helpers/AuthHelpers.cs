using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Entities.Web;
using PlayStation_App.Database;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.User;
using PlayStation_Gui.Tools.Database;
using PlayStation_Gui.Tools.Debug;
using SQLite.Net.Platform.WinRT;

namespace PlayStation_App.Tools.Helpers
{
    public class AccountAuthHelpers
    {
        private static readonly UserAccountDataSource Db = new UserAccountDataSource(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));

        private static void UpdateUserObject(AccountUser user, Tokens tokens, LogInUser loginUser, User userUpdate)
        {
            if (tokens != null)
            {
                user.AccessToken = tokens.AccessToken;
                user.RefreshToken = tokens.RefreshToken;
                user.RefreshDate = GetUnixTime(DateTime.Now) + tokens.ExpiresIn;
            }

            if (loginUser != null)
            {
                user.Username = loginUser.OnlineId;
                user.AccountId = loginUser.AccountId;
                user.Region = loginUser.Region;
                user.Language = loginUser.Language;
            }

            if (userUpdate != null)
            {
                var avatars = userUpdate.AvatarUrls.LastOrDefault();
                if (avatars != null)
                {
                    user.AvatarUrl = avatars.AvatarUrlLink;
                }
                user.Username = userUpdate.OnlineId;
            }
        }

        public async static Task<bool> UpdateUserIsDefault(AccountUser user, bool isDefault = false)
        {
            user.IsDefaultLogin = isDefault;
            var result = await Db.AccountUserRepository.Update(user);
            return result > 0;
        }

        public async static Task<bool> UpdateAllUserAccountsDefaultFalse()
        {
            var defaultUserAccounts = await Db.AccountUserRepository.Items.Where(node => node.IsDefaultLogin).ToListAsync();
            if (defaultUserAccounts.Any())
            {
                foreach (var account in defaultUserAccounts)
                {
                    account.IsDefaultLogin = false;
                }
                var result = await Db.AccountUserRepository.UpdateAll(defaultUserAccounts);
                return result > 0;
            }

            return true;
        }

        public async static Task<bool> CreateUserAccount(Tokens tokens, LogInUser loginUser, User userUpdate)
        {
            var user = new AccountUser();
            UpdateUserObject(user, tokens, loginUser, userUpdate);
            var result = await Db.AccountUserRepository.Create(user);
            return result > 0;
        }

        public async static Task<bool> DeleteUserAccount(AccountUser user)
        {
            var result = await Db.AccountUserRepository.Delete(user);
            return result > 0;
        }

        public async static Task<bool> UpdateUserAccount(AccountUser user, Tokens tokens, LogInUser loginUser, User userUpdate)
        {
            UpdateUserObject(user, tokens, loginUser, userUpdate);
            var result = await Db.AccountUserRepository.Update(user);
            return result > 0;
        }

        public async static Task<bool> UpdateTokens(AccountUser user, Result result)
        {
            if (result?.Tokens == null)
            {
                return false;
            }

            var tokens = JsonConvert.DeserializeObject<Tokens>(result.Tokens);
            return await UpdateUserAccount(user, tokens, null, null);
        }

        public static async Task<bool> DoesUserExist(string accountId)
        {
            var result = Db.AccountUserRepository.Items.Where(node => node.AccountId == accountId);
            return await result.CountAsync() > 0;
        }

        public static long GetUnixTime(DateTime time)
        {
            time = time.ToUniversalTime();
            TimeSpan timeSpam = time - (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));
            return (long)timeSpam.TotalSeconds;
        }
    }
}
