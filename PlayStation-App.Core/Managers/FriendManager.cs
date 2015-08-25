using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.Friend;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Interfaces;
using PlayStation_App.Core.Tools;

namespace PlayStation_App.Core.Managers
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

        public async Task<FriendsEntity> GetFriendsList(string username, int? offset, bool blockedPlayer,
            bool playedRecently, bool personalDetailSharing, bool friendStatus, bool requesting, bool requested,
            bool onlineFilter, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.FriendList, user.Region, username, offset);
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
                if (blockedPlayer) url = string.Format("https://{0}-prof.np.community.playstation.net/userProfile/v1/users/{1}/blockList?fields=@default,@profile&offset={2}", user.Region, username, offset);
                // TODO: Fix this cheap hack to get around caching issue. For some reason, no-cache is not working...
                url += "&r=" + Guid.NewGuid();
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var friend = JsonConvert.DeserializeObject<FriendsEntity>(result.ResultJson);
                return friend;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> AddFriend(string username, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.DenyAddFriend, user.Region, user.OnlineId, username);
                var result = await _webManager.PutData(new Uri(url), null, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding friend", ex);
            }
        }

        public async Task<bool> IgnoreFriendRequest(string username, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.DenyAddFriend, user.Region, user.OnlineId, username);
                var result = await _webManager.DeleteData(new Uri(url), null, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing friend", ex);
            }
        }

        public async Task<bool> DeleteFriend(string username, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.DenyAddFriend, user.Region, user.OnlineId, username);
                var result = await _webManager.DeleteData(new Uri(url), null, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing friend", ex);
            }
        }

        public async Task<string> GetFriendRequestMessage(string username, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var url = string.Format(EndPoints.RequestMessage, user.Region, user.OnlineId, username);
                var result = await _webManager.GetData(new Uri(url), userAccountEntity);
                var o = JObject.Parse(result.ResultJson);
                return o["requestMessage"] != null ? (string)o["requestMessage"] : string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting friend request message", ex);
            }
        }

        public async Task<bool> SendFriendRequest(string username, string requestMessage,
            UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var param = new Dictionary<String, String>();
                if (!string.IsNullOrEmpty(requestMessage)) param.Add("requestMessage", requestMessage);
                var jsonObject = JsonConvert.SerializeObject(param);
                var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var url = string.Format(EndPoints.SendFriendRequest, user.Region, user.OnlineId, username, requestMessage);
                var result = await _webManager.PostData(new Uri(url), stringContent, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending friend request", ex);
            }
        }

        public async Task<bool> SendNameRequest(string username, UserAccountEntity userAccountEntity)
        {
            try
            {
                var user = userAccountEntity.GetUserEntity();
                var param = new Dictionary<String, String>();
                var jsonObject = JsonConvert.SerializeObject(param);
                var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var url = string.Format(EndPoints.SendNameRequest, user.Region, user.OnlineId, username);
                var result = await _webManager.PostData(new Uri(url), stringContent, userAccountEntity);
                return result.IsSuccess;
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending name request", ex);
            }
        }

        public async Task<FriendTokenEntity> GetFriendLink(UserAccountEntity userAccountEntity)
        {
            try
            {
                var param = new Dictionary<string, string> { { "type", "ONE" } };
                var jsonObject = JsonConvert.SerializeObject(param);
                var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                var result = await _webManager.PostData(new Uri(EndPoints.FriendMeUrl), stringContent, userAccountEntity);
                var friend = JsonConvert.DeserializeObject<FriendTokenEntity>(result.ResultJson);
                return friend;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting friends link", ex);
            }
        }
    }
}
