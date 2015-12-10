using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
{
    public class StickerManager
    {
        private readonly IWebManager _webManager;

        public StickerManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public StickerManager()
            : this(new WebManager())
        {
        }

        public async Task<Result> GetStickerPackList(string region = "jp")
        {
            var url = string.Format(EndPoints.StickerPreset, region);
            return await GetData(new Uri(url));
        }

        public async Task<Result> GetStickerAndManifestPack(string url)
        {
            return await GetData(new Uri(url));
        }

        private async Task<Result> GetData(Uri uri)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    Result result = new Result(false, "");
                    var response = await httpClient.GetAsync(uri);
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result.IsSuccess = response.IsSuccessStatusCode;
                    result.ResultJson = responseContent;
                    return result;
                }
                catch
                {
                    // TODO: Add detail error result to json object.
                    return new Result(false, string.Empty);
                }
            }
        }
    }
}
