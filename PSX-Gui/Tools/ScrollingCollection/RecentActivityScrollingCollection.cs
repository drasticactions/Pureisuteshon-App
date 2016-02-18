using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Models.RecentActivity;
using PlayStation_App.Models.Response;
using PlayStation_App.Properties;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_Gui.Views;


namespace PlayStation_App.Tools.ScrollingCollection
{
    public class RecentActivityScrollingCollection : ObservableCollection<Feed>,
            ISupportIncrementalLoading, INotifyPropertyChanged
    {
        public bool IsNews;
        public bool StorePromo;
        private bool _isEmpty;
        private bool _isLoading;

        public RecentActivityScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
        }

        public string Username { get; set; }

        public int PageCount { get; set; }

        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                NotifyPropertyChanged("IsLoading");
            }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }

            private set
            {
                _isEmpty = value;
                NotifyPropertyChanged("IsEmpty");
            }
        }

        private RecentActivityManager _recentActivityManager = new RecentActivityManager();

        public new event PropertyChangedEventHandler PropertyChanged;

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        public bool HasMoreItems { get; set; }

        private async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
                if (!IsLoading)
           {
                LoadFeedList(Username);
            }
            var ret = new LoadMoreItemsResult { Count = IsEmpty ? 0 : count };
            return ret;
        }

        public async void LoadFeedList(string username)
        {
            IsLoading = true;
            await Shell.Instance.ViewModel.UpdateTokens();
            var feedResultEntity =
                await _recentActivityManager.GetActivityFeed(username, PageCount, StorePromo, IsNews, Shell.Instance.ViewModel.CurrentTokens, Shell.Instance.ViewModel.CurrentUser.Region, Shell.Instance.ViewModel.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, feedResultEntity);
            var result = await ResultChecker.CheckSuccess(feedResultEntity, false);
            if (!result)
            {
                HasMoreItems = false;
                if (Count <= 0)
                {
                    IsEmpty = true;
                }
                IsLoading = false;
                return;
            }
            var feedEntity = JsonConvert.DeserializeObject<RecentActivityResponse>(feedResultEntity.ResultJson);
            if (feedEntity == null)
            {
                if (Count <= 0)
                {
                    IsEmpty = true;
                }
                HasMoreItems = false;
                IsLoading = false;
                return;
            }
            if (feedEntity.Feed == null)
            {
                if (Count <= 0)
                {
                    IsEmpty = true;
                }
                HasMoreItems = false;
                IsLoading = false;
                return;
            }
            foreach (var feed in feedEntity.Feed)
            {
                Add(feed);
            }
            if (feedEntity.Feed.Any())
            {
                HasMoreItems = true;
                PageCount++;
            }
            else
            {
                if (Count <= 0)
                {
                    IsEmpty = true;
                }
                HasMoreItems = false;
            }
            IsLoading = false;
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
