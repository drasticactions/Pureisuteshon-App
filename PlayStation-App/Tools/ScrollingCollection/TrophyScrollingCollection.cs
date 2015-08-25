using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.Trophy;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Managers;
using PlayStation_App.Properties;

namespace PlayStation_App.Tools.ScrollingCollection
{
    public class TrophyScrollingCollection : ObservableCollection<TrophyEntity.TrophyTitle>, ISupportIncrementalLoading,
        INotifyPropertyChanged
    {
        public int Offset;
        public UserAccountEntity UserAccountEntity;
        private bool _isEmpty;
        private bool _isLoading;

        public TrophyScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
        }

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

        public bool HasMoreItems { get; private set; }

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
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<bool> LoadTrophies(string username)
        {
            Offset = Offset + MaxCount;
            IsLoading = true;
            var trophyManager = new TrophyManager();
            TrophyEntity trophyList = await trophyManager.GetTrophyList(username, Offset, UserAccountEntity);
            if (trophyList == null)
            {
                //HasMoreItems = false;
                IsEmpty = true;
                return false;
            }
            foreach (TrophyEntity.TrophyTitle trophy in trophyList.TrophyTitles)
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