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

        public UserAccountDatabase(ISQLitePlatform platform, string dblocation)
        {
            Platform = platform;
            DbLocation = dblocation;
        }

        public async Task<bool> HasAccounts()
        {
            using (var ds = new UserAccountDataSource(Platform, DbLocation))
            {
                var accounts = await ds.AccountUserRepository.GetAllWithChildren();
                return accounts.Any();
            }
        }

        public async Task<bool> HasDefaultAccounts()
        {
            using (var ds = new UserAccountDataSource(Platform, DbLocation))
            {
                var accounts = await ds.AccountUserRepository.GetAllWithChildren();
                return accounts.Any(node => node.IsDefaultLogin);
            }
        }

        public async Task<List<AccountUser>> GetUserAccounts()
        {
            using (var ds = new UserAccountDataSource(Platform, DbLocation))
            {
                return await ds.AccountUserRepository.GetAllWithChildren();
            }
        }

        public async Task<List<AccountUser>> GetDefaultUserAccounts()
        {
            using (var ds = new UserAccountDataSource(Platform, DbLocation))
            {
                var accounts = await ds.AccountUserRepository.GetAllWithChildren();
                return accounts.Where(node => node.IsDefaultLogin).ToList();
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
