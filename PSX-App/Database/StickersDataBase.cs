using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Models.Response;

namespace PlayStation_App.Database
{
    public class StickersDatabase
    {
        public async Task<List<StickerResponse>> GetStickers()
        {
            using (var ds = new StickersDataSource())
            {
                return await ds.StickersRepository.GetAllWithChildren();
            }
        }

        public async Task<int> InsertStickers(List<StickerResponse> stickers)
        {
            using (var ds = new StickersDataSource())
            {
                return await ds.StickersRepository.CreateAll(stickers);
            }
        }

        public async Task<int> InsertSticker(StickerResponse sticker)
        {
            using (var ds = new StickersDataSource())
            {
                return await ds.StickersRepository.Create(sticker);
            }
        }

        public async Task RemoveStickers()
        {
            using (var ds = new StickersDataSource())
            {
                await ds.StickersRepository.RemoveAll();
            }
        }

        public async Task RemoveSticker(StickerResponse sticker)
        {
            using (var ds = new StickersDataSource())
            {
                await ds.StickersRepository.Remove(sticker);
            }
        }
    }
}
