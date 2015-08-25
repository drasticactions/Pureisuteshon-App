using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.Friend;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Managers;

namespace PlayStation_App.Tools.ScrollingCollection
{
    public class FriendScrollingCollection : ObservableCollection<FriendsEntity.Friend>, ISupportIncrementalLoading
    {
        public bool BlockedPlayer;
        public bool FriendStatus;
        public int Offset;

        public bool OnlineFilter;

        public bool PersonalDetailSharing;

        public bool RecentlyPlayed;
        public bool Requested;
        public bool Requesting;
        private bool _isEmpty;
        public UserAccountEntity UserAccountEntity;

        private bool _isLoading;

        public FriendScrollingCollection()
        {
            HasMoreItems = true;
            IsLoading = false;
        }

        public string Username { get; set; }

        public bool IsLoading
        {
            get { return _isLoading; }

            private set
            {
                _isLoading = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsLoading"));
            }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }

            private set
            {
                _isEmpty = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsEmpty"));
            }
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return LoadDataAsync(count).AsAsyncOperation();
        }

        public bool HasMoreItems { get; protected set; }
        private async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
            IsLoading = true;
            try
            {
                var friendManager = new FriendManager();
                var friendEntity =
                    await
                        friendManager.GetFriendsList(Username, Offset, BlockedPlayer, RecentlyPlayed, PersonalDetailSharing,
                            FriendStatus, Requesting, Requested, OnlineFilter, UserAccountEntity);
                if (friendEntity == null)
                {
                    HasMoreItems = false;
                    if (Count <= 0)
                    {
                        IsEmpty = true;
                    }
                    return new LoadMoreItemsResult { Count = count };
                }
                if (friendEntity.FriendList == null)
                {
                    HasMoreItems = false;
                    if (Count <= 0)
                    {
                        IsEmpty = true;
                    }
                    return new LoadMoreItemsResult { Count = count };
                }
                foreach (var friend in friendEntity.FriendList)
                {
                    Add(friend);
                }
                if (friendEntity.FriendList.Any())
                {
                    HasMoreItems = true;
                    Offset = Offset += 32;
                }
                else
                {
                    HasMoreItems = false;
                    if (Count <= 0)
                    {
                        IsEmpty = true;
                    }
                }
            }
            catch (Exception ex)
            {
                HasMoreItems = false;
            }
            IsLoading = false;
            return new LoadMoreItemsResult { Count = count };
        }

    }
}
