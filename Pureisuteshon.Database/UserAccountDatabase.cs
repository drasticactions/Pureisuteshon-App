using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Models.Authentication;
using SQLite.Net.Interop;

namespace PlayStation_App.Database
{
    public class UserAccountDatabase
    {
        public static ISQLitePlatform Platform { get; set; }

        public static string DbLocation { get; set; }

        public async Task<List<AccountUser>> GetUserAccounts()
        {
            using (var ds = new UserAccountDataSource(Platform, DbLocation))
            {
                return await ds.AccountUserRepository.GetAllWithChildren();
            }
        }

        public async Task<int> CreateAccountUser(AccountUser user)
        {
            using (var db = new UserAccountDataSource(Platform, DbLocation))
            {
               return await db.AccountUserRepository.Create(user);
            }
        }

        public async Task<int> UpdateAccountUser(AccountUser user)
        {
            using (var db = new UserAccountDataSource(Platform, DbLocation))
            {
                return await db.AccountUserRepository.Update(user);
            }
        }


        public async Task<int> RemoveAccountUser(AccountUser user)
        {
            using (var db = new UserAccountDataSource(Platform, DbLocation))
            {
                return await db.AccountUserRepository.Delete(user);
            }
        }
    }
}
