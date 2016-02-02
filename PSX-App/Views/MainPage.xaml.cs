using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PlayStation_App.Commands.SelectAccount;
using PlayStation_App.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlayStation_App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            App.RootFrame = MainFrame;
            App.RootFrame.Navigated += RootFrameOnNavigated;
            Locator.ViewModels.MainPageVm.SwipeableSplitView = Splitter;
            var test3 = new CheckAndNavigateToSelectAccountCommand();
            test3.Execute(null);
        }

        private async void MenuClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as MenuItem;
            menuItem?.Command.Execute(null);
            if (Splitter.IsSwipeablePaneOpen)
            {
                Splitter.IsSwipeablePaneOpen = false;
            }
        }

        private void RootFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = App.RootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Splitter.DisplayMode = (Splitter.DisplayMode == SplitViewDisplayMode.Inline) ? SplitViewDisplayMode.CompactInline : SplitViewDisplayMode.Inline;
            Splitter.IsSwipeablePaneOpen = (Splitter.IsSwipeablePaneOpen != true);
        }

        private void MenuSelection_Click(object sender, RoutedEventArgs e)
        {
            var menuListView = (ListView)sender;
            if (menuListView.SelectedItem == null)
            {
                return;
            }

            var menuItem = menuListView.SelectedItem as MenuItem;
            menuItem?.Command.Execute(null);
            if (Splitter.IsSwipeablePaneOpen)
            {
                Splitter.IsSwipeablePaneOpen = false;
            }
            menuListView.SelectedItem = null;
        }
    }
}
