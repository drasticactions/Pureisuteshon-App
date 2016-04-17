using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AmazingPullToRefresh.Controls;
using Kimono.Controls;
using Newtonsoft.Json;
using PlayStation_App.Models.Friends;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.ScrollingCollection;
using PlayStation_Gui.Controls;
using PlayStation_Gui.Views;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class FriendsViewModel : ViewModelBase
    {
        public MasterDetailViewControl MasterDetailViewControl { get; set; }
        private FriendScrollingCollection _friendScrollingCollection;
        private ComboBox _filterComboBox;

        private bool _friendLoaded = default(bool);
        public bool FriendLoaded
        {
            get { return _friendLoaded; }
            set
            {
                Set(ref _friendLoaded, value);
            }
        }

        private Friend _selected;

        public Friend Selected
        {
            get { return _selected; }
            set
            {
                Set(ref _selected, value);
            }
        }

        public FriendView FriendView { get; set; }

        public FriendScrollingCollection FriendScrollingCollection
        {
            get { return _friendScrollingCollection; }
            set
            {
                Set(ref _friendScrollingCollection, value);
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Template10.Common.BootStrapper.Current.NavigationService.FrameFacade.BackRequested += MasterDetailViewControl.NavigationManager_BackRequested;
            MasterDetailViewControl.LoadLayout();

            if (FriendScrollingCollection == null || !FriendScrollingCollection.Any())
            {
                SetFriendList();
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            Template10.Common.BootStrapper.Current.NavigationService.FrameFacade.BackRequested -= MasterDetailViewControl.NavigationManager_BackRequested;
            try
            {
                if (Selected != null)
                {
                    state["Friend"] = JsonConvert.SerializeObject(Selected);
                }
            }
            catch (Exception ex)
            {
                ResultChecker.LogEvent("SerializeError", new Dictionary<string, string>() { { "serialization", ex.Message } });
            }
            return Task.CompletedTask;
        }

        public ComboBox FilterComboBox
        {
            get { return _filterComboBox; }
            set
            {
                Set(ref _filterComboBox, value);
            }
        }

        public void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFriendList();
        }

        public void PullToRefresh_ListView(object sender, RefreshRequestedEventArgs e)
        {
            //var deferral = e.GetDeferral();
            SetFriendList();
            //deferral.Complete();
        }

        public void SetFriendList()
        {
            if (FilterComboBox == null) return;
            switch (FilterComboBox.SelectedIndex)
            {
                case 0:
                    // Friends - Online
                    SetFriendsList(Shell.Instance.ViewModel.CurrentUser.Username, true, false, false, false, true, false, false);
                    break;
                case 1:
                    // All
                    SetFriendsList(Shell.Instance.ViewModel.CurrentUser.Username, false, false, false, false, true, false, false);
                    break;
                case 2:
                    // Friend Request Received
                    SetFriendsList(Shell.Instance.ViewModel.CurrentUser.Username, false, false, false, false, true, false, true);
                    break;
                case 3:
                    // Friend Requests Sent
                    SetFriendsList(Shell.Instance.ViewModel.CurrentUser.Username, false, false, false, false, true, true, false);
                    break;
            }
        }

        public void SetFriendsList(string userName, bool onlineFilter, bool blockedPlayer, bool recentlyPlayed,
            bool personalDetailSharing, bool friendStatus, bool requesting, bool requested)
        {
            FriendScrollingCollection = new FriendScrollingCollection
            {
                Offset = 0,
                OnlineFilter = onlineFilter,
                Requested = requested,
                Requesting = requesting,
                PersonalDetailSharing = personalDetailSharing,
                FriendStatus = friendStatus,
                Username = userName
            };
        }
    }
}
