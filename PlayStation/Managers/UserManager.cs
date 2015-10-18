using System;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
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

        public async Task<Result> GetUser(string userName, UserAuthenticationEntity userAuthenticationEntity,
            string region = "jp", string language = "ja")
        {
            try
            {
                var url = string.Format(EndPoints.User, region, userName);
                return await _webManager.GetData(new Uri(url), userAuthenticationEntity, language);
            }
            catch (Exception exception)
            {
                throw new Exception("Error getting user", exception);
            }
        }
    }
}
