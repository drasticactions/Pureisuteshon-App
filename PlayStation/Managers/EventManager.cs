using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Interfaces;
using PlayStation.Tools;

namespace PlayStation.Managers
{
    public class EventManager
    {
        private readonly IWebManager _webManager;

        public EventManager(IWebManager webManager)
        {
            _webManager = webManager;
        }

        public EventManager()
            : this(new WebManager())
        {
        }

        public async Task<Result> GetMyEvents(UserAuthenticationEntity userAuthenticationEntity, string targetEvents = "inSessionAndUpcoming", string region = "ja")
        {
            var url = string.Format(EndPoints.MyEvents, region, targetEvents);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetFeaturedEvents(UserAuthenticationEntity userAuthenticationEntity, string sort = "recommendInGame", string region = "ja")
        {
            var url = string.Format(EndPoints.FeaturedEvents, region, sort);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetEvents(UserAuthenticationEntity userAuthenticationEntity, string sort = "eventStartDate", string region = "ja")
        {
            var url = string.Format(EndPoints.Events, region, sort);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetEventTitles(UserAuthenticationEntity userAuthenticationEntity, string sort = "eventStartDate", string region = "ja")
        {
            var url = string.Format(EndPoints.Events, region, sort);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetEventTitleDetails(string id, UserAuthenticationEntity userAuthenticationEntity, string sort = "eventStartDate", string region = "ja")
        {
            var url = string.Format(EndPoints.EventTitleDetails, region, id, sort);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetEventDetails(string id, UserAuthenticationEntity userAuthenticationEntity, string region = "ja")
        {
            var url = string.Format(EndPoints.EventDetail, region, id);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> AddEvent(string id, string username, UserAuthenticationEntity userAuthenticationEntity, string region = "ja")
        {
            var url = string.Format(EndPoints.AddRemoveEvent, region, id, username);
            var json = new StringContent("{\"autoBootPreference\":{\"autoBootFlag\":false,\"key\":\"\"}}", Encoding.UTF8, "application/json");
            return await _webManager.PutData(new Uri(url), json, userAuthenticationEntity);
        }

        public async Task<Result> RemoveEvent(string id, string username, UserAuthenticationEntity userAuthenticationEntity, string region = "ja")
        {
            var url = string.Format(EndPoints.AddRemoveEvent, region, id, username);
            return await _webManager.DeleteData(new Uri(url), null, userAuthenticationEntity);
        }

        public async Task<Result> GetEventDetailFriends(string id, int offset, int limit, UserAuthenticationEntity userAuthenticationEntity,
            string region = "ja")
        {
            var url = string.Format(EndPoints.EventDetailFriends, region, id, offset, limit);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }

        public async Task<Result> GetEventLiveBroadcast(string id, UserAuthenticationEntity userAuthenticationEntity)
        {
            var url = string.Format(EndPoints.EventLiveBroadcast, id);
            return await _webManager.GetData(new Uri(url), userAuthenticationEntity);
        }
    }
}
