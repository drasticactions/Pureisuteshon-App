using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using PlayStation_App.Database.Tools;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.Response;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.WinRT;

namespace PlayStation_App.Database
{
    public class StickersDataSource : IDisposable
    {
        private const string Dbfilename = "Stickers.db";
        protected StorageFolder UserFolder { get; set; }
        protected SQLiteAsyncConnection Db { get; set; }

        public Repository<StickerResponse> StickersRepository { get; set; }

        public StickersDataSource()
        {
            UserFolder = ApplicationData.Current.LocalFolder;
            var dbPath = Path.Combine(UserFolder.Path, Dbfilename);
            var connectionFactory = new Func<SQLiteConnectionWithLock>(() => new SQLiteConnectionWithLock(new SQLitePlatformWinRT(), new SQLiteConnectionString(dbPath, storeDateTimeAsTicks: false)));
            Db = new SQLiteAsyncConnection(connectionFactory);

            StickersRepository = new Repository<StickerResponse>(Db);
        }

        public void Dispose()
        {
            UserFolder = null;
            this.Db = null;
            StickersRepository = null;
        }

        public void InitDatabase()
        {
            //Check to ensure db file exists
            try
            {
                //Try to read the database file
                UserFolder.GetFileAsync(Dbfilename).GetAwaiter().GetResult();
            }
            catch
            {
                //Will throw an exception if not found
                UserFolder.CreateFileAsync(Dbfilename).GetAwaiter().GetResult();
            }
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
