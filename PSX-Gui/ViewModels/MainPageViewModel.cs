using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation_App.Models.RecentActivity;
using PlayStation_App.Models.Response;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_App.Tools.ScrollingCollection;
using PlayStation_Gui.Views;
using Template10.Mvvm;
using Template10.Utils;

namespace PlayStation_Gui.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                Set(ref _isLoading, value);
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await Initialize();
        }

        public async Task SelectRecentItem(object sender, ItemClickEventArgs e)
        {
            var args = e;
            var feed = args.ClickedItem as Feed;
            if (feed == null)
                return;
            if (feed.IsPreviousButton)
                await LoadPreviousPages();
            if (feed.IsNextButton)
                await LoadNextPages();
        }

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

        public async Task LoadPage()
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
            await Shell.Instance.ViewModel.UpdateTokens();
            var feedResultEntity =
                await _recentActivityManager.GetActivityFeed(Shell.Instance.ViewModel.CurrentUser.Username, _page, true, true, Shell.Instance.ViewModel.CurrentTokens, Shell.Instance.ViewModel.CurrentUser.Region, Shell.Instance.ViewModel.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, feedResultEntity);
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
                Set(ref _recentActivityScrollingCollection, value);
            }
        }
    }
}

