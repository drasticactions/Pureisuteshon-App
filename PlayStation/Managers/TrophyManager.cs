using System;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
{
    public class TrophyManager
    {
        private readonly IWebManager _webManager;

        public TrophyManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public TrophyManager()
            : this(new WebManager())
        {
        }

        public async Task<Result> GetTrophyList(string comparedUser, string fromUser, int offset, UserAuthenticationEntity userAuthenticationEntity,
            string region = "jp", string language = "ja")
        {
            var url = string.Format(EndPoints.TrophyList, region, language, offset, comparedUser, fromUser);
            url += "&r=" + Guid.NewGuid();
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetTrophyDetailList(string gameId, string comparedUser, bool includeHidden,
            UserAuthenticationEntity userAuthenticationEntity, string fromUser, string region = "jp", string language = "ja")
        {
            var url = string.Format(EndPoints.TrophyDetailList, region, gameId, language, comparedUser, fromUser);
            url += "&r=" + Guid.NewGuid();
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }
    }
}
