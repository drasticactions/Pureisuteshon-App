using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.Trophy;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Interfaces;
using PlayStation_App.Core.Tools;

namespace PlayStation_App.Core.Managers
{
    public class TrophyDetailManager
    {
        private readonly IWebManager _webManager;

        public TrophyDetailManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public TrophyDetailManager()
            : this(new WebManager())
        {
        }

        public async Task<TrophyDetailEntity> GetTrophyDetailList(string gameId, string comparedUser, bool includeHidden,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.TrophyDetailList, user.Region, gameId, user.Language, comparedUser, user.OnlineId);
                url += "&r=" + Guid.NewGuid();
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var trophyDetailEntity = JsonConvert.DeserializeObject<TrophyDetailEntity>(result.ResultJson);
                return trophyDetailEntity;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get trophy detail list", ex);
            }
        }
    }
}
