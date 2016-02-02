using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
{
    public class FriendManager
    {
        private readonly IWebManager _webManager;

        public FriendManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public FriendManager()
            : this(new WebManager())
        {
        }

        public async Task<Result> GetFriendsList(string username, int? offset, bool blockedPlayer,
            bool playedRecently, bool personalDetailSharing, bool friendStatus, bool requesting, bool requested,
            bool onlineFilter, UserAuthenticationEntity userAuthenticationEntity, string region = "jp", string language = "ja")
        {
            var url = string.Format(EndPoints.FriendList, region, username, offset);
            if (onlineFilter) url += "&filter=online";
            if (friendStatus && !requesting && !requested) url += "&friendStatus=friend&presenceType=primary";
            if (friendStatus && requesting && !requested) url += "&friendStatus=requesting";
            if (friendStatus && !requesting && requested) url += "&friendStatus=requested";
            if (personalDetailSharing && requested) url += "&friendStatus=friend&personalDetailSharing=requested&presenceType=primary";
            if (personalDetailSharing && requesting) url += "&friendStatus=friend&personalDetailSharing=requesting&presenceType=primary";
            if (playedRecently)
                url =
                    string.Format(
                        EndPoints.RecentlyPlayed, username);
            if (blockedPlayer) url =
                $"https://{region}-prof.np.community.playstation.net/userProfile/v1/users/{username}/blockList?fields=@default,@profile&offset={offset}";
            url += "&r=" + Guid.NewGuid();
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity, language);
        }

        public async Task<Result> AddFriend(string username, string currentUserOnlineId, UserAuthenticationEntity userAuthenticationEntity, string region = "jp", string language = "ja")
        {
            var url = string.Format(EndPoints.DenyAddFriend, region, currentUserOnlineId, username);
            var result = await _webManager.PutData(new Uri(url), null, userAuthenticationEntity, language);
            return new Result(result.IsSuccess, string.Empty);
        }

        public async Task<Result> IgnoreFriendREquest(string username, string currentUserOnlineId, UserAuthenticationEntity userAuthenticationEntity, string region = "jp", string language = "ja")
        {
            var url = string.Format(EndPoints.DenyAddFriend, region, currentUserOnlineId, username);
            var result = await _webManager.DeleteData(new Uri(url), null, userAuthenticationEntity, language);
            return new Result(result.IsSuccess, string.Empty);
        }

        public async Task<Result> DeleteFriend(string username, string currentUserOnlineId, UserAuthenticationEntity userAuthenticationEntity, string region = "jp", string language = "ja")
        {
            var url = string.Format(EndPoints.DenyAddFriend, region, currentUserOnlineId, username);
            var result = await _webManager.DeleteData(new Uri(url), null, userAuthenticationEntity, language);
            return new Result(result.IsSuccess, string.Empty);
        }

        public async Task<Result> GetFriendRequestMessage(string username, string currentUserOnlineId, UserAuthenticationEntity userAuthenticationEntity, string region = "jp")
        {
            var url = string.Format(EndPoints.RequestMessage, region, currentUserOnlineId, username);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> SendFriendRequest(string username, string requestMessage, string currentUserOnlineId, UserAuthenticationEntity userAuthenticationEntity, string region = "jp")
        {
            var param = new Dictionary<String, String>();
            if (!string.IsNullOrEmpty(requestMessage)) param.Add("requestMessage", requestMessage);
            var jsonObject = JsonConvert.SerializeObject(param);
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var url = string.Format(EndPoints.SendFriendRequest, region, currentUserOnlineId, username, requestMessage);
            return await _webManager.PostData(new Uri(url), stringContent, userAuthenticationEntity);
        }

        public async Task<Result> SendNameRequest(string username, string currentUserOnlineId, UserAuthenticationEntity userAuthenticationEntity, string region = "jp")
        {
            var param = new Dictionary<String, String>();
            var jsonObject = JsonConvert.SerializeObject(param);
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            var url = string.Format(EndPoints.SendNameRequest, region, currentUserOnlineId, username);
            return await _webManager.PostData(new Uri(url), stringContent, userAuthenticationEntity);
        }

        public async Task<Result> GetFriendLink(UserAuthenticationEntity userAuthenticationEntity)
        {
            var param = new Dictionary<string, string> { { "type", "ONE" } };
            var jsonObject = JsonConvert.SerializeObject(param);
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            return await _webManager.PostData(new Uri(EndPoints.FriendMeUrl), stringContent, userAuthenticationEntity);
        }
    }
}
