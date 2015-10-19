using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Newtonsoft.Json;
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
            await LoadPage();
        }

        private async Task LoadPage()
        {
            IsLoading = true;
            var testFeed = new RecentActivityScrollingCollection();
            testFeed.Add(_page == 0
                ? new Feed() {IsPreviousButton = true, IsReloadButton = true}
                : new Feed() {IsPreviousButton = true});
            await LoadFeed(testFeed);
            await LoadFeed(testFeed);
            testFeed.Add(new Feed() { IsNextButton = true });
            RecentActivityScrollingCollection = testFeed;
            IsLoading = false;
        }

        private async Task LoadFeed(ObservableCollection<Feed> testFeed)
        {
            var feedResultEntity =
                await _recentActivityManager.GetActivityFeed(Locator.ViewModels.MainPageVm.CurrentUser.Username, _page, true, true, Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, feedResultEntity);
            var feedEntity = JsonConvert.DeserializeObject<RecentActivityResponse>(feedResultEntity.ResultJson);
            foreach (var feed in feedEntity.Feed)
            {
                testFeed.Add(feed);
            }
            _page++;
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
