using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Interfaces;
using PlayStation_App.Core.Tools;

namespace PlayStation_App.Core.Managers
{
    public class UserManager
    {
        private readonly IWebManager _webManager;

        public UserManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public UserManager()
            : this(new WebManager())
        {
        }

        public async Task<UserEntity> GetUser(string userName, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.User, user.Region, userName);
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var userEntity = JsonConvert.DeserializeObject<UserEntity>(result.ResultJson);
                if (userEntity?.trophySummary == null) return userEntity;
                var list = userEntity.trophySummary.EarnedTrophies;
                userEntity.trophySummary.TotalTrophies = list.Bronze + list.Gold + list.Platinum + list.Silver;
                return userEntity;
            }
            catch (Exception exception)
            {
                throw new Exception("Error getting user", exception);
            }
        }

        public async Task<UserEntity> GetUserAvatar(string userName, UserAccountEntity userAccountEntity)
        {
            try
            {
                var authenticationManager = new AuthenticationManager();
                var user = userAccountEntity.GetUserEntity();
                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    await authenticationManager.RefreshAccessToken(userAccountEntity);
                }
                string url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/profile?fields=avatarUrl", user.Region, userName);
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var userEntity = JsonConvert.DeserializeObject<UserEntity>(result.ResultJson);
                return userEntity;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}
