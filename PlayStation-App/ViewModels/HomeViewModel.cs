using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Managers;

namespace PlayStation_App.ViewModels
{
    public class HomeViewModel : NotifierBase
    {
        private UserAccountEntity.User _currentUserEntity;
        public UserAccountEntity.User CurrentUserEntity
        {
            get { return _currentUserEntity; }
            set
            {
                SetProperty(ref _currentUserEntity, value);
                OnPropertyChanged();
            }
        }

        private ObservableCollection<LiveBroadcastEntity> _broadcastEntities = new ObservableCollection<LiveBroadcastEntity>();

        public ObservableCollection<LiveBroadcastEntity> BroadcastEntities
        {
            get { return _broadcastEntities; }
            set
            {
                SetProperty(ref _broadcastEntities, value);
                OnPropertyChanged();
            }
        }


        private ObservableCollection<FriendsEntity.Friend> _friendList;

        public ObservableCollection<FriendsEntity.Friend> FriendList
        {
            get { return _friendList; }
            set
            {
                SetProperty(ref _friendList, value);
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RecentActivityEntity.Feed> _whatsNewList = new ObservableCollection<RecentActivityEntity.Feed>();

        public ObservableCollection<RecentActivityEntity.Feed> WhatsNew
        {
            get { return _whatsNewList; }
            set
            {
                SetProperty(ref _whatsNewList, value);
                OnPropertyChanged();
            }
        }

        public async Task Initialize()
        {
            IsLoading = true;
            try
            {
                CurrentUserEntity = Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity();
                var friendManager = new FriendManager();

                var friendEntity = 
                    await friendManager.GetFriendsList(CurrentUserEntity.OnlineId, 0, false, false, false, true, false, false,
                   false, Locator.ViewModels.MainPageVm.CurrentUser);
                FriendList = new ObservableCollection<FriendsEntity.Friend>
                {
                    new FriendsEntity.Friend()
                    {
                        IsMenuItem = true,
                        FriendCount = friendEntity.TotalResults
                    }
                };

                WhatsNew = new ObservableCollection<RecentActivityEntity.Feed>()
                {
                    new RecentActivityEntity.Feed()
                    {
                        IsMenuItem = true
                    }
                };

                BroadcastEntities.Add(new LiveBroadcastEntity()
                {
                    IsMenuItem = true
                });
                foreach (var friend in friendEntity.FriendList)
                {
                    FriendList.Add(friend);
                }

                var recentActivityManager = new RecentActivityManager();
                var recentActivityList =
                    await
                        recentActivityManager.GetActivityFeed(CurrentUserEntity.OnlineId, 0, true, true,
                            Locator.ViewModels.MainPageVm.CurrentUser);
                foreach (var item in recentActivityList.feed)
                {
                    WhatsNew.Add(item);
                }

                var liveStreamManager = new LiveStreamManager();
                NicoNicoEntity nicoNicoEntity =
                    await liveStreamManager.GetNicoFeed("onair", "PS4", 0, 80, "view", Locator.ViewModels.MainPageVm.CurrentUser);
                if (nicoNicoEntity?.programs == null)
                {
                    IsLoading = false;
                    return;
                }
                foreach (NicoNicoEntity.Program program in nicoNicoEntity.programs)
                {
                    var entity = new LiveBroadcastEntity();
                    entity.ParseFromNicoNico(program);
                    BroadcastEntities.Add(entity);
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            IsLoading = false;
        }
    }
}
