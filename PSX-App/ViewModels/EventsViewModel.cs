using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Common;
using PlayStation_App.Models.Events;
using PlayStation_App.Models.Response;
using PlayStation_App.Tools.Debug;

namespace PlayStation_App.ViewModels
{
    public class EventsViewModel : NotifierBase
    {
        public async Task Initialize()
        {
            //SetupSampleData();
            IsLoading = true;
            await GetFeatureEvents();
            await GetAllEvents();
            await GetMyEvents();
            await GetGameList();
            IsLoading = false;
        }

        private async void SetupSampleData()
        {
            var items = await SampleData.GetEventFeed();
            foreach (var item in items)
            {
                FeatureEvents.Add(item);
                MyEvents.Add(item);
                AllEvents.Add(item);
            }
        }

        public async Task GetGameList()
        {

        }

        public async Task GetMyEvents()
        {
            var feedResultEntity =
                await
                    _eventManager.GetMyEvents(Locator.ViewModels.MainPageVm.CurrentTokens, "inSessionAndUpcoming",
                        Locator.ViewModels.MainPageVm.CurrentUser.Region);
            var result = await ResultChecker.CheckSuccess(feedResultEntity);
            if (!result)
            {
                return;
            }
            var feedEntity = JsonConvert.DeserializeObject<EventsResponse>(feedResultEntity.ResultJson);
            foreach (var feed in feedEntity.Events)
            {
                MyEvents.Add(feed);
            }
        }

        public async Task GetAllEvents()
        {
            var feedResultEntity =
                await
                    _eventManager.GetEvents(Locator.ViewModels.MainPageVm.CurrentTokens, "eventStartDate",
                        Locator.ViewModels.MainPageVm.CurrentUser.Region);
            var result = await ResultChecker.CheckSuccess(feedResultEntity);
            if (!result)
            {
                return;
            }
            var feedEntity = JsonConvert.DeserializeObject<EventsResponse>(feedResultEntity.ResultJson);
            foreach (var feed in feedEntity.Events)
            {
                AllEvents.Add(feed);
            }
        }

        public async Task GetFeatureEvents()
        {
            var feedResultEntity =
                await
                    _eventManager.GetFeaturedEvents(Locator.ViewModels.MainPageVm.CurrentTokens, "recommendInGame",
                        Locator.ViewModels.MainPageVm.CurrentUser.Region);
            var result = await ResultChecker.CheckSuccess(feedResultEntity);
            if (!result)
            {
                return;
            }
            var feedEntity = JsonConvert.DeserializeObject<EventsResponse>(feedResultEntity.ResultJson);
            foreach (var feed in feedEntity.Events)
            {
                FeatureEvents.Add(feed);
            }
        }

        private readonly EventManager _eventManager = new EventManager();

       public ObservableCollection<Event> FeatureEvents { get; set; } = new ObservableCollection<Event>();

        public ObservableCollection<Event> MyEvents { get; set; } = new ObservableCollection<Event>();

        public ObservableCollection<Event> AllEvents { get; set; } = new ObservableCollection<Event>();
    }
}
