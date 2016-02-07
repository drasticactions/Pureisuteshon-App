using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Database.Tools;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.Response;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Interop;

namespace PlayStation_App.Database
{
    public class StickersDataSource : IDisposable
    {        protected SQLiteAsyncConnection Db { get; set; }

        public Repository<StickerResponse> StickersRepository { get; set; }

        public StickersDataSource(ISQLitePlatform platform, string dbPath)
        {
            var connectionFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(platform, new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: false)));
            Db = new SQLiteAsyncConnection(connectionFactory);

            StickersRepository = new Repository<StickerResponse>(Db);
        }

        public void Dispose()
        {
            this.Db = null;
            StickersRepository = null;
        }

        public void CreateDatabase()
        {
            var existingTables =
                Db.QueryAsync<sqlite_master>("SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;")
                  .GetAwaiter()
                  .GetResult();

            if (existingTables.Any(x => x.name == "StickerResponse") != true)
            {
                Db.CreateTableAsync<StickerResponse>().GetAwaiter().GetResult();
            }
        }
    }

}
