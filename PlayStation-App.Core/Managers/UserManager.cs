using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation_App.Core.Entities;
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
                return userEntity;
            }
            catch (Exception exception)
            {
                throw new Exception("Error getting user", exception);
            }
        }
    }
}
