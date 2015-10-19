using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Newtonsoft.Json;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation_App.Commands.WhatsNew;
using PlayStation_App.Common;
using PlayStation_App.Models.RecentActivity;
using PlayStation_App.Models.Response;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_App.Tools.ScrollingCollection;

namespace PlayStation_App.ViewModels
{
    public class WhatsNewViewModel : NotifierBase
    {
        public async Task Initialize()
        {
            if (DesignMode.DesignModeEnabled)
            {
                SetupSampleData();
            }
            else
            {
                //SetupSampleData();
                _page = 0;
                await LoadNextPages();
            }
        }

        public async Task LoadPreviousPages()
        {
            // TODO: Fix this crap. This sort of paging is weird and makes no sense.
            _page = _page - 4;
            if (_page < 0) _page = 0;
            await LoadPage();
        }

        public async Task LoadNextPages()
        {
            IsLoading = true;
            var result = new Result();
            try
            {
                await LoadPage();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error = ex.Message;
            }
            await ResultChecker.CheckSuccess(result);
            IsLoading = false;
        }

        private async Task LoadPage()
        {
            var testFeed = new RecentActivityScrollingCollection
            {
                _page == 0
                    ? new Feed() {IsPreviousButton = true, IsReloadButton = true}
                    : new Feed() {IsPreviousButton = true}
            };
            var result = await LoadFeed(testFeed);
            if (result)
            {
                result = await LoadFeed(testFeed);
            }
            if (result)
            {
                testFeed.Add(new Feed() { IsNextButton = true });
            }
            RecentActivityScrollingCollection = testFeed;
        }

        private async Task<bool> LoadFeed(ObservableCollection<Feed> testFeed)
        {
            var feedResultEntity =
                await _recentActivityManager.GetActivityFeed(Locator.ViewModels.MainPageVm.CurrentUser.Username, _page, true, true, Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
            var result = await ResultChecker.CheckSuccess(feedResultEntity);
            if (!result)
            {
                return false;
            }   
            if (string.IsNullOrEmpty(feedResultEntity.ResultJson))
            {
                // No Items, return false.
                return false;
            }
            var feedEntity = JsonConvert.DeserializeObject<RecentActivityResponse>(feedResultEntity.ResultJson);
            foreach (var feed in feedEntity.Feed)
            {
                testFeed.Add(feed);
            }
            _page++;
            return true;
        }

        private async void SetupSampleData()
        {
            RecentActivityScrollingCollection = new RecentActivityScrollingCollection();
            var items = await SampleData.GetSampleRecentActivityFeed();
            foreach (var item in items)
            {
                RecentActivityScrollingCollection.Add(item);
            }
            RecentActivityScrollingCollection.Add(new Feed() { IsNextButton = true });
        }

        private readonly RecentActivityManager _recentActivityManager = new RecentActivityManager();
        private int _page;

        private ObservableCollection<Feed> _recentActivityScrollingCollection;

        public ObservableCollection<Feed> RecentActivityScrollingCollection
        {
            get { return _recentActivityScrollingCollection; }
            set
            {
                SetProperty(ref _recentActivityScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public SelectRecentItemCommand SelectRecentItemCommand { get; set; } = new SelectRecentItemCommand();
    }
}
