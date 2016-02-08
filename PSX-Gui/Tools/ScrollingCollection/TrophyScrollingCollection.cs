using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.TrophyDetail;
using PlayStation_App.Properties;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_Gui.Views;


namespace PlayStation_App.Tools.ScrollingCollection
{
    public class TrophyScrollingCollection : ObservableCollection<TrophyTitle>, ISupportIncrementalLoading,
        INotifyPropertyChanged
    {
        public int Offset;
        private bool _isEmpty;
        private bool _isLoading;

        public TrophyScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
        }

        public string CompareUsername { get; set; }
        public string Username { get; set; }
        public int MaxCount { get; set; }

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
            set
            {
                _isEmpty = value;
                OnPropertyChanged();
            }
        }

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
                await LoadTrophies(Username);
            }
            var ret = new LoadMoreItemsResult {Count = count};
            return ret;
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

        public async Task<bool> LoadTrophies(string username)
        {
            Offset = Offset + MaxCount;
            IsLoading = true;
            var trophyManager = new TrophyManager();
            var trophyResultList = await trophyManager.GetTrophyList(username, CompareUsername, Offset, Shell.Instance.ViewModel.CurrentTokens, Shell.Instance.ViewModel.CurrentUser.Region, Shell.Instance.ViewModel.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, trophyResultList);
            var result = await ResultChecker.CheckSuccess(trophyResultList, false);
            if (!result)
            {
                HasMoreItems = false;
                if (Count <= 0)
                {
                    IsEmpty = true;
                }
                IsLoading = false;
                return false;
            }
            var trophyList = JsonConvert.DeserializeObject<TrophyDetailResponse>(trophyResultList.ResultJson);
            if (trophyList == null)
            {
                //HasMoreItems = false;
                IsEmpty = true;
                return false;
            }
            foreach (var trophy in trophyList.TrophyTitles)
            {
                Add(trophy);
            }
            if (trophyList.TrophyTitles.Any())
            {
                HasMoreItems = true;
                MaxCount += 64;
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
            return true;
        }
    }
}