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
using PlayStation_App.Models.MessageGroups;
using PlayStation_App.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlayStation_App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessageListPage : Page
    {
        private MessageGroupItem _lastSelectedItem;
        public MessageListPage()
        {
            this.InitializeComponent();
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Locator.ViewModels.MessagesVm.MessageCollection.CollectionChanged += (s, args) => ScrollToBottom();

            // Assure we are displaying the correct item. This is necessary in certain adaptive cases.
            MessageList.SelectedItem = _lastSelectedItem;
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateForVisualState(e.NewState, e.OldState);
        }

        private void UpdateForVisualState(VisualState newState, VisualState oldState = null)
        {
            var isNarrow = newState == NarrowState;

            if (isNarrow && oldState == DefaultState && _lastSelectedItem != null)
            {
                // Resize down to the detail item. Don't play a transition.
                App.RootFrame.Navigate(typeof(MessageDetailPage), null, new SuppressNavigationTransitionInfo());
            }

            EntranceNavigationTransitionInfo.SetIsTargetElement(MasterListView, isNarrow);
            //if (DetailContentPresenter != null)
            //{
            //    EntranceNavigationTransitionInfo.SetIsTargetElement(DetailContentPresenter, !isNarrow);
            //}
        }

        private async void MessageGroup_OnClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (MessageGroupItem)e.ClickedItem;
            _lastSelectedItem = clickedItem;
            await Locator.ViewModels.MessagesVm.GetMessages(clickedItem.MessageGroup);
            if (AdaptiveStates.CurrentState == NarrowState)
            {
                // Use "drill in" transition for navigating from master list to detail view
                App.RootFrame.Navigate(typeof(MessageDetailPage), null, new DrillInNavigationTransitionInfo());
            }
            else
            {
                // Play a refresh animation when the user switches detail items.
                //EnableContentTransitions();
            }
        }

        private async void DownloadImage_OnClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (MessageGroupItem) e.ClickedItem;
            if (clickedItem.ImageAvailable)
            {
                
            }
        }

        private async void PullToRefreshBox_OnRefreshInvoked(DependencyObject sender, object args)
        {
            await Locator.ViewModels.MessagesVm.GetMessages(Locator.ViewModels.MessagesVm.SelectedMessageGroup);
        }

        private async void PullToRefreshBoxMessageList_OnRefreshInvoked(DependencyObject sender, object args)
        {
            await Locator.ViewModels.MessagesVm.GetMessageGroups(Locator.ViewModels.MessagesVm.Username);
        }

        private void MessagesList_OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void ScrollToBottom()
        {
            if (MessagesList.Items != null)
            {
                var selectedIndex = MessagesList.Items.Count - 1;
                if (selectedIndex < 0)
                    return;

                MessagesList.SelectedIndex = selectedIndex;
            }
            MessagesList.UpdateLayout();

            MessagesList.ScrollIntoView(MessagesList.SelectedItem);
        }
    }
}
