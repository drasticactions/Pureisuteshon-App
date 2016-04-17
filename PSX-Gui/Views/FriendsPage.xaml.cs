using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AmazingPullToRefresh.Controls;
using PlayStation_App.Models.Friends;
using PlayStation_Gui.ViewModels;


namespace PlayStation_Gui.Views
{
    public sealed partial class FriendsPage : Page
    {
        public FriendsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ViewModel.FilterComboBox = FilterComboBox;
            ViewModel.FriendView = FriendPageView;
            ViewModel.MasterDetailViewControl = previewControl;
        }

        public FriendsViewModel ViewModel => this.DataContext as FriendsViewModel;

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ResetPageCache();
            }
        }

        private void ResetPageCache()
        {
            var cacheSize = ((Frame)Parent).CacheSize;
            ((Frame)Parent).CacheSize = 0;
            ((Frame)Parent).CacheSize = cacheSize;
        }

        private void RefreshList(object sender, RoutedEventArgs e)
        {
            ViewModel.SetFriendList();
        }

        private async void FriendList_OnItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem == null) return;
            var friend = e.ClickedItem as Friend;
            await FriendPageView.LoadFriend(friend.OnlineId);
            ViewModel.FriendLoaded = true;
        }
    }
}
