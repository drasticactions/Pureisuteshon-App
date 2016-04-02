using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PlayStation_App.Models.Friends;
using PlayStation_App.Models.MessageGroups;
using PlayStation_App.Tools.Debug;
using PlayStation_App.ViewModels;
using PlayStation_Gui.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlayStation_Gui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MessagesPage : Page
    {
        public MessagesViewModel ViewModel => this.DataContext as MessagesViewModel;

        public MessagesPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ViewModel.MasterDetailViewControl = previewControl;
            ViewModel.ListView = MessagesList;
            ViewModel.StickerFlyout = StickerFlyout;
        }

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

        private async void DownloadImage(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as MenuFlyoutItem;
            var message = imageSource?.CommandParameter as MessageGroupItem;
            if (message == null)
                return;
            await ViewModel.DownloadImageAsync(message);
        }

        private void RemoveImage(object sender, RoutedEventArgs e)
        {
            ViewModel.IsImageAttached = false;
            ViewModel.AttachedImage = null;
        }

        private async void LoadImage(object sender, RoutedEventArgs e)
        {
            var imageSource = sender as Image;
            var message = imageSource?.DataContext as MessageGroupItem;
            if (message == null)
                return;
            await ViewModel.LoadMessageImage(message);
        }

        private async void AttachImage(object sender, RoutedEventArgs e)
        {
            string error;
            try
            {
                var openPicker = new FileOpenPicker
                {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                openPicker.FileTypeFilter.Add(".jpg");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".gif");
                var file = await openPicker.PickSingleFileAsync();
                if (file == null) return;
                var stream = await file.OpenAsync(FileAccessMode.Read);
                ViewModel.AttachedImage = stream;
                ViewModel.IsImageAttached = true;
                ViewModel.ImagePath = file.Path;
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            await ResultChecker.SendMessageDialogAsync(error, false);
        }

        private async void RefreshGroupList(object sender, RoutedEventArgs e)
        {
            await ViewModel.GetMessageGroups(Shell.Instance.ViewModel.CurrentUser.Username);
        }

        private async void RefreshList(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedMessageGroup != null)
            {
                await ViewModel.GetMessages(ViewModel.SelectedMessageGroup);
            }
        }

        private async void NewMessage(object sender, RoutedEventArgs e)
        {
            string error;
            if (!MessageFriendList.SelectedItems.Any()) return;
            try
            {
                ViewModel.MessageCollection?.Clear();
                var usernameObjectList = MessageFriendList.SelectedItems.ToList();
                var friendList = usernameObjectList.Cast<Friend>().ToList();
                var usernameList = friendList.Select(node => node.OnlineId).ToList();
                ViewModel.IsNewMessage = true;
                ViewModel.GroupMembers = usernameList;
                var selectedGroupMessage = new MessageGroup()
                {
                    MessageGroupDetail = new MessageGroupDetail() { MessageGroupName = string.Join(",", usernameList) }
                };
                ViewModel.SelectedMessageGroup = selectedGroupMessage;
                ViewModel.Selected = new MessageGroupItem()
                {
                    MessageGroup = selectedGroupMessage
                };
                FriendMessageFlyout.Hide();
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            await ResultChecker.SendMessageDialogAsync(error, false);
        }
    }
}
