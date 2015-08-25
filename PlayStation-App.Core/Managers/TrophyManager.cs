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

        public async Task<TrophyEntity> GetTrophyList(string user, int offset, UserAccountEntity userAccountEntity)
        {
            try
            {
                var userAccount = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.TrophyList, userAccount.Region, userAccount.Language, offset, user,
                    userAccountEntity.GetUserEntity().OnlineId);
                url += "&r=" + Guid.NewGuid();
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var trophy = JsonConvert.DeserializeObject<TrophyEntity>(result.ResultJson);
                return trophy;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
