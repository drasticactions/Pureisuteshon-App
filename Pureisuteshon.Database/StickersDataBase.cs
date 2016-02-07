using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Models.Response;
using SQLite.Net.Interop;

namespace PlayStation_App.Database
{
    public class StickersDatabase
    {
        public static ISQLitePlatform Platform { get; set; }

        public static string DbLocation { get; set; }

        public StickersDatabase(ISQLitePlatform platform, string location)
        {
            Platform = platform;
            DbLocation = location;
        }

        public async Task<List<StickerResponse>> GetStickers()
        {
            using (var ds = new StickersDataSource(Platform, DbLocation))
            {
                return await ds.StickersRepository.GetAllWithChildren();
            }
        }

        public async Task<int> InsertStickers(List<StickerResponse> stickers)
        {
            using (var ds = new StickersDataSource(Platform, DbLocation))
            {
                return await ds.StickersRepository.CreateAll(stickers);
            }
        }

        public async Task<int> InsertSticker(StickerResponse sticker)
        {
            using (var ds = new StickersDataSource(Platform, DbLocation))
            {
                return await ds.StickersRepository.Create(sticker);
            }
        }

        public async Task RemoveStickers()
        {
            using (var ds = new StickersDataSource(Platform, DbLocation))
            {
                await ds.StickersRepository.RemoveAll();
            }
        }

        public async Task RemoveSticker(StickerResponse sticker)
        {
            using (var ds = new StickersDataSource(Platform, DbLocation))
            {
                await ds.StickersRepository.Remove(sticker);
            }
        }
    }
}
