using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Managers;
using PlayStation_App.Models.Friends;
using PlayStation_App.Models.Response;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;


namespace PlayStation_App.Tools.ScrollingCollection
{
    public class FriendScrollingCollection : ObservableCollection<Friend>, ISupportIncrementalLoading
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

        private async Task<LoadMoreItemsResult> LoadFriends(uint count)
        {
            IsLoading = true;
            try
            {
                var friendManager = new FriendManager();
                var friendResultEntity =
                    await
                        friendManager.GetFriendsList(Username, Offset, BlockedPlayer, RecentlyPlayed, PersonalDetailSharing,
                            FriendStatus, Requesting, Requested, OnlineFilter, Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
                var result = await ResultChecker.CheckSuccess(friendResultEntity, false);
                if (!result)
                {
                    HasMoreItems = false;
                    if (Count <= 0)
                    {
                        IsEmpty = true;
                    }
                    IsLoading = false;
                    return new LoadMoreItemsResult { Count = 0 };
                }
                await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, friendResultEntity);
                var friendEntity = JsonConvert.DeserializeObject<FriendListResponse>(friendResultEntity.ResultJson);
                if (friendEntity == null)
                {
                    HasMoreItems = false;
                    if (Count <= 0)
                    {
                        IsEmpty = true;
                    }
                    IsLoading = false;
                    return new LoadMoreItemsResult { Count = count };
                }
                if (friendEntity.Friend == null)
                {
                    HasMoreItems = false;
                    if (Count <= 0)
                    {
                        IsEmpty = true;
                    }
                    IsLoading = false;
                    return new LoadMoreItemsResult { Count = count };
                }
                foreach (var friend in friendEntity.Friend)
                {
                    Add(friend);
                }
                if (friendEntity.Friend.Any())
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

        private async Task<LoadMoreItemsResult> LoadDataAsync(uint count)
        {
            return await LoadFriends(count);
        }

    }
}
