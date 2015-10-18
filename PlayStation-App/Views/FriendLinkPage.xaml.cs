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
using PlayStation_App.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlayStation_App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendLinkPage : Page, IDisposable
    {
        private readonly FriendLinkViewModel _vm;
        private string _link = string.Empty;
        public FriendLinkPage()
        {
            this.InitializeComponent();
            _vm = (FriendLinkViewModel)DataContext;
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            args.Request.Data.SetText(
                $"{loader.GetString("SendMeFriendRequest/Text")} {_link} {Environment.NewLine} {loader.GetString("SentFromPlayStationApp/Text")}");
            args.Request.Data.Properties.Title = loader.GetString("InviteFriendsToPsn/Text");
        }

        private async void SendEmail_OnClick(object sender, RoutedEventArgs e)
        {
            await _vm.SendFriendLinkViaEmail();
        }

        private async void SendSms_OnClick(object sender, RoutedEventArgs e)
        {
            await _vm.SendFriendLinkViaSms();
        }

        private async void ShareInvite_OnClick(object sender, RoutedEventArgs e)
        {
            _vm.IsLoading = true;
            _link = await _vm.CreateFriendLink();
            _vm.IsLoading = false;
            DataTransferManager.ShowShareUI();
        }

        public void Dispose()
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;
        }
    }
}
