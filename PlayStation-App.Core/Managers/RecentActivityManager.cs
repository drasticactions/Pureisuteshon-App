using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Interfaces;
using PlayStation_App.Core.Tools;

namespace PlayStation_App.Core.Managers
{
    public class RecentActivityManager
    {
        private readonly IWebManager _webManager;

        public RecentActivityManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public RecentActivityManager()
            : this(new WebManager())
        {
        }

        public async Task<RecentActivityEntity> GetActivityFeed(string userName, int? pageNumber, bool storePromo,
            bool isNews, UserAccountEntity userAccountEntity)
        {
            try
            {
                var feedNews = isNews ? "news" : "feed";
                var url = string.Format(EndPoints.RecentActivity, userName, feedNews, pageNumber);
                if (storePromo)
                    url += "&filters=STORE_PROMO";
                url += "&r=" + Guid.NewGuid();
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var recentActivityEntity = JsonConvert.DeserializeObject<RecentActivityEntity>(result.ResultJson);
                return recentActivityEntity;
            }
            catch (Exception exception)
            {
                
                throw new Exception("Error getting activity feed", exception);
            }
        }
    }
}
