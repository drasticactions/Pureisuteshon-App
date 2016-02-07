using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Database.Tools;
using PlayStation_App.Models.Authentication;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;

namespace PlayStation_App.Database
{
    public class UserAccountDataSource : IDisposable
    {
        protected SQLiteAsyncConnection Db { get; set; }

        public Repository<AccountUser> AccountUserRepository { get; set; }

        public UserAccountDataSource(ISQLitePlatform platform, string dbPath)
        {
            var connectionFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(platform, new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: false)));
            Db = new SQLiteAsyncConnection(connectionFactory);

            AccountUserRepository = new Repository<AccountUser>(Db);
        }
        public void Dispose()
        {
            this.Db = null;
            AccountUserRepository = null;
        }

        public void CreateDatabase()
        {
            var existingTables =
                Db.QueryAsync<sqlite_master>("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;")
                  .GetAwaiter()
                  .GetResult();

            if (existingTables.Any(x => x.name == "AccountUser") != true)
            {
                Db.CreateTableAsync<AccountUser>().GetAwaiter().GetResult();
            }
        }
    }
}
