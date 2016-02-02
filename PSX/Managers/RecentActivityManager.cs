using System;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
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

        public async Task<Result> GetActivityFeed(string userName, int? pageNumber, bool storePromo,
            bool isNews, UserAuthenticationEntity userAuthenticationEntity, string region = "jp", string language = "ja")
        {
            var feedNews = isNews ? "news" : "feed";
            var url = string.Format(EndPoints.RecentActivity, userName, feedNews, pageNumber);
            if (storePromo)
                url += "&filters=STORE_PROMO";
            url += "&r=" + Guid.NewGuid();
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity, language);
        }
    }
}
