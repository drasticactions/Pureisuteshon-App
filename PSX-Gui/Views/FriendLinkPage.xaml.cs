using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PlayStation_Gui.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlayStation_Gui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendLinkPage : Page, IDisposable
    {
        public FriendLinkPage()
        {
            this.InitializeComponent();
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        private string _link = string.Empty;

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            args.Request.Data.SetText(
                $"{loader.GetString("SendMeFriendRequest/Text")} {_link} {Environment.NewLine} {loader.GetString("SentFromPlayStationApp/Text")}");
            args.Request.Data.Properties.Title = loader.GetString("InviteFriendsToPsn/Text");
        }

        public FriendLinkViewModel ViewModel => this.DataContext as FriendLinkViewModel;

        private async void ShareInvite_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.IsLoading = true;
            var _link = await ViewModel.CreateFriendLink();
            ViewModel.IsLoading = false;
            if (string.IsNullOrEmpty(_link))
            {
                return;
            }
            DataTransferManager.ShowShareUI();
        }

        public void Dispose()
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }
    }
}
