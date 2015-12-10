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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlayStation_App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessageDetailPage : Page
    {
        public MessageDetailPage()
        {
            this.InitializeComponent();

        }

        private void PageRoot_Loaded(object sender, RoutedEventArgs e)
        {
            Locator.ViewModels.MessagesVm.MessageCollection.CollectionChanged += (s, args) => ScrollToBottom();

            if (ShouldGoToWideState())
            {
                // We shouldn't see this page since we are in "wide master-detail" mode.
                // Play a transition as we are navigating from a separate page.
                NavigateBackForWideState(useTransition: true);
            }
            else
            {
                // Realize the main page content.
                FindName("RootPanel");
            }

            Window.Current.SizeChanged += Window_SizeChanged;
        }

        void NavigateBackForWideState(bool useTransition)
        {
            // Evict this page from the cache as we may not need it again.
            NavigationCacheMode = NavigationCacheMode.Disabled;

            if (useTransition)
            {
                Frame.GoBack(new EntranceNavigationTransitionInfo());
            }
            else
            {
                Frame.GoBack(new SuppressNavigationTransitionInfo());
            }
        }

        private void PageRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (ShouldGoToWideState())
            {
                // Make sure we are no longer listening to window change events.
                Window.Current.SizeChanged -= Window_SizeChanged;

                // We shouldn't see this page since we are in "wide master-detail" mode.
                NavigateBackForWideState(useTransition: false);
            }
        }

        private bool ShouldGoToWideState()
        {
            return Window.Current.Bounds.Width >= 800;
        }

        private async void PullToRefreshBox_OnRefreshInvoked(DependencyObject sender, object args)
        {
            await Locator.ViewModels.MessagesVm.GetMessages(Locator.ViewModels.MessagesVm.SelectedMessageGroup);
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
