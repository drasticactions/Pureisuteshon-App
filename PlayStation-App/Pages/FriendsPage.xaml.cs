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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using PlayStation_App.Commands.DetailLoader;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.Friend;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace PlayStation_App.Pages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class FriendsPage : Page
    {
        private FriendsEntity.Friend _lastSelectedItem;

        public FriendsPage()
        {
            this.InitializeComponent();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // Assure we are displaying the correct item. This is necessary in certain adaptive cases.
            FriendList.SelectedItem = _lastSelectedItem;
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateForVisualState(e.NewState, e.OldState);
        }

        private void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFriendList();
        }

        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == NarrowState;

            if (isNarrow && oldState == DefaultState && _lastSelectedItem != null)
            {
                // Resize down to the detail item. Don't play a transition.
               App.RootFrame.Navigate(typeof(FriendPage), null, new SuppressNavigationTransitionInfo());
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(MasterListView, isNarrow);
            if (DetailContentPresenter != null)
            {
                EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
            }
        }

        private void Friend_OnClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (FriendsEntity.Friend)e.ClickedItem;
            _lastSelectedItem = clickedItem;
            var command = new LoadFriendDetail();
            command.Execute(clickedItem.OnlineId);
            if (AdaptiveStates.CurrentState == NarrowState)
            {
                // Use "drill in" transition for navigating from master list to detail view
                App.RootFrame.Navigate(typeof(FriendPage), null, new DrillInNavigationTransitionInfo());
            }
            else
            {
                // Play a refresh animation when the user switches detail items.
                //EnableContentTransitions();
            }
        }

        private void SetFriendList()
        {
            if (FilterComboBox == null) return;
            switch (FilterComboBox.SelectedIndex)
            {
                case 0:
                    // Friends - Online
                    Locator.ViewModels.FriendsPageVm.SetFriendsList(Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId, true, false, false, false, true, false, false);
                    break;
                case 1:
                    // All
                    Locator.ViewModels.FriendsPageVm.SetFriendsList(Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId, false, false, false, false, true, false, false);
                    break;
                case 2:
                    // Friend Request Received
                    Locator.ViewModels.FriendsPageVm.SetFriendsList(Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId, false, false, false, false, true, false, true);
                    break;
                case 3:
                    // Friend Requests Sent
                    Locator.ViewModels.FriendsPageVm.SetFriendsList(Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId, false, false, false, false, true, true, false);
                    break;
                case 4:
                    // Name Requests Received
                    Locator.ViewModels.FriendsPageVm.SetFriendsList(Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId, true, false, false, true, true, false, false);
                    break;
                case 5:
                    // Name Requests Sent
                    Locator.ViewModels.FriendsPageVm.SetFriendsList(Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId, false, false, false, true, true, true, false);
                    break;
            }
        }
    }
}
