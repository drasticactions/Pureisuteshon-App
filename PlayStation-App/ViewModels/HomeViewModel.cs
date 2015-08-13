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
                foreach (var friend in friendEntity.FriendList)
                {
                    FriendList.Add(friend);
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
