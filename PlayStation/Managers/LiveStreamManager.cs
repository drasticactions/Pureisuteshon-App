using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
{
    public class LiveStreamManager
    {
        private readonly IWebManager _webManager;

        public LiveStreamManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public LiveStreamManager()
            : this(new WebManager())
        {
        }

        public async Task<Result> GetNicoFeed(string status, string platform, bool titlePreset, int offset,
            int limit, string sort, string query,
            UserAuthenticationEntity userAuthenticationEntity)
        {
            var url = EndPoints.NicoNicoBaseUrl;
            // Sony's app hardcodes this value to 0. 
            // This app could, in theory, allow for more polling of data, so these options are left open to new values and limits.
            if (!string.IsNullOrEmpty(query))
            {
                url += $"keyword={query}&";
            }
            url += $"offset={offset}&";
            url += $"limit={limit}&";
            url += $"status={status}&";
            url += $"sce_platform={platform}&";
            if (titlePreset)
            {
                url += "sce_title_preset=true&";
            }
            url += $"sort={sort}";
            // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
            url += "&r=" + Guid.NewGuid();
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetTwitchFeed(int offset, int limit, string platform,
            bool titlePreset, string query, UserAuthenticationEntity userAuthenticationEntity, string language = "ja")
        {
            var url = EndPoints.TwitchBaseUrl;
            // Sony's app hardcodes this value to 0. 
            // This app could, in theory, allow for more polling of data, so these options are left open to new values and limits.
            url += $"sce_platform=PS4&offset={offset}&";
            url += $"limit={limit}&";
            if (!string.IsNullOrEmpty(query))
            {
                url += $"q={query}&";
            }
            if (titlePreset)
            {
                url += "sce_title_preset=true";
            }

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v1+json"));
                httpClient.DefaultRequestHeaders.Add("Platform", "54bd6377db3b48cba9ecc44bff5a410b");
                httpClient.DefaultRequestHeaders.Add("Accept-Language", language);

                var response = await httpClient.GetAsync(new Uri(url));
                var responseContent = await response.Content.ReadAsStringAsync();
                return new Result(response.IsSuccessStatusCode, responseContent);
            }
        }

        public async Task<Result> GetUstreamFeed(int pageNumber, int pageSize, string detailLevel,
            Dictionary<string, string> filterList, string sortBy, string query, UserAuthenticationEntity userAuthenticationEntity)
        {
            var url = EndPoints.UstreamBaseUrl;
            url += $"p={pageNumber}&";
            url += $"pagesize={pageSize}&";
            url += $"detail_level={detailLevel}&";
            foreach (var item in filterList)
            {
                if (item.Key.Equals(EndPoints.UstreamUrlConstants.Interactive))
                {
                    url += string.Format(EndPoints.UstreamUrlConstants.FilterBase, EndPoints.UstreamUrlConstants.PlatformPs4) +
                                     "[interactive]=" + item.Value + "&";
                }
                else
                {
                    url += string.Concat(string.Format(EndPoints.UstreamUrlConstants.FilterBase, item.Key), "=", item.Value + "&");
                }
            }
            url += $"sort={sortBy}";
            if (!string.IsNullOrEmpty(query))
            {
                url += $"&q={query}";
            }
            url += "&r=" + Guid.NewGuid();
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetVideoFeedFromPsn(string serviceType, string sort, UserAuthenticationEntity userAuthenticationEntity, string language = "ja")
        {
            return await _webManager.GetData(new Uri(string.Format(EndPoints.LiveBroadcast, serviceType, sort, language)),
                userAuthenticationEntity);
        }
    }
}
