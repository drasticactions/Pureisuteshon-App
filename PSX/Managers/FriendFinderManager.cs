using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
{
    public class FriendFinderManager
    {
        private readonly IWebManager _webManager;

        public FriendFinderManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public FriendFinderManager()
            : this(new WebManager())
        {
        }

        public async Task<Result> SearchForFriends(string query, UserAuthenticationEntity userAuthenticationEntity,
            string region = "jp", string language = "ja")
        {
            var url = String.Format(EndPoints.FriendFinder, query);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity, language);
        }
    }
}
