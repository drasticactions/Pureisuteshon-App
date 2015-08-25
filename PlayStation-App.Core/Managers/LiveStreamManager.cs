using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.Live;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Interfaces;
using PlayStation_App.Core.Tools;

namespace PlayStation_App.Core.Managers
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

        public async Task<NicoNicoEntity> GetNicoFeed(string status, string platform, bool titlePreset, int offset, int limit, string sort,string query,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var url = EndPoints.NicoNicoBaseUrl;
                // Sony's app hardcodes this value to 0. 
                // This app could, in theory, allow for more polling of data, so these options are left open to new values and limits.
                if (!string.IsNullOrEmpty(query))
                {
                    url += string.Format("keyword={0}&", query);
                }
                url += string.Format("offset={0}&", offset);
                url += string.Format("limit={0}&", limit);
                url += string.Format("status={0}&", status);
                url += string.Format("sce_platform={0}&", platform);
                if (titlePreset)
                {
                    url += "sce_title_preset=true&";
                }
                url += string.Format("sort={0}", sort);
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var nico = JsonConvert.DeserializeObject<NicoNicoEntity>(result.ResultJson);
                return nico;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get Nico Nico Douga feed", ex);
            }
        }

        public async Task<TwitchEntity> GetTwitchFeed(int offset, int limit, string platform,
            bool titlePreset, string query, UserAccountEntity userAccountEntity)
        {
            try
            {
                var url = EndPoints.TwitchBaseUrl;
                // Sony's app hardcodes this value to 0. 
                // This app could, in theory, allow for more polling of data, so these options are left open to new values and limits.
                url += string.Format("sce_platform=PS4&offset={0}&", offset);
                url += string.Format("limit={0}&", limit);
                if (!string.IsNullOrEmpty(query))
                {
                    url += string.Format("q={0}&", query);
                }
                if (titlePreset)
                {
                    url += "sce_title_preset=true";
                }
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v1+json"));
                        httpClient.DefaultRequestHeaders.Add("Platform", "54bd6377db3b48cba9ecc44bff5a410b");
                        var user = userAccountEntity.GetUserEntity();
                        if (user != null)
                        {
                            var language = userAccountEntity.GetUserEntity().Language;
                            httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                        }

                        var response = await httpClient.GetAsync(new Uri(url));
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return string.IsNullOrEmpty(responseContent)
                            ? new TwitchEntity()
                            : JsonConvert.DeserializeObject<TwitchEntity>(responseContent);
                    }
                    catch
                    {
                        // TODO: Add detail error result to json object.
                        return new TwitchEntity();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get Twitch feed", ex);
            }
        }

        public async Task<UstreamEntity> GetUstreamFeed(int pageNumber, int pageSize, string detailLevel,
            Dictionary<string, string> filterList, string sortBy, string query, UserAccountEntity userAccountEntity)
        {
            try
            {
                var url = EndPoints.UstreamBaseUrl;
                url += string.Format("p={0}&", pageNumber);
                url += string.Format("pagesize={0}&", pageSize);

                url += string.Format("detail_level={0}&", detailLevel);
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
                url += string.Format("sort={0}", sortBy);
                if (!string.IsNullOrEmpty(query))
                {
                    url += string.Format("&q={0}", query);
                }
                url += "&r=" + Guid.NewGuid();
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var ustream = JsonConvert.DeserializeObject<UstreamEntity>(result.ResultJson);
                return ustream;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get Ustream feed", ex);
            }
        }
    }
}
