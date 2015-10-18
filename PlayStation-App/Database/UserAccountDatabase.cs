using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Models.Authentication;

namespace PlayStation_App.Database
{
    public class UserAccountDatabase
    {
        public async Task<List<AccountUser>> GetUserAccounts()
        {
            using (var ds = new UserAccountDataSource())
            {
                return await ds.AccountUserRepository.GetAllWithChildren();
            }
        }

        public async Task<int> CreateAccountUser(AccountUser user)
        {
            using (var db = new UserAccountDataSource())
            {
               return await db.AccountUserRepository.Create(user);
            }
        }

        public async Task<int> UpdateAccountUser(AccountUser user)
        {
            using (var db = new UserAccountDataSource())
            {
                return await db.AccountUserRepository.Update(user);
            }
        }


        public async Task<int> RemoveAccountUser(AccountUser user)
        {
            using (var db = new UserAccountDataSource())
            {
                return await db.AccountUserRepository.Delete(user);
            }
        }
    }
}
