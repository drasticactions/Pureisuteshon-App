using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AmazingPullToRefresh.Controls;
using Kimono.Controls;
using Newtonsoft.Json;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation_App.Common;
using PlayStation_App.Models.FriendFinder;
using PlayStation_App.Models.MessageGroups;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.Sticker;
using PlayStation_App.Models.User;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_App.Tools.ScrollingCollection;
using PlayStation_App.ViewModels;
using PlayStation_Gui.Views;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class MessagesViewModel : ViewModelBase
    {
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Template10.Common.BootStrapper.Current.NavigationService.FrameFacade.BackRequested += MasterDetailViewControl.NavigationManager_BackRequested;
            MasterDetailViewControl.LoadLayout();
            if (FriendScrollingCollection == null || !FriendScrollingCollection.Any())
            {
                SetFriendsList(Shell.Instance.ViewModel.CurrentUser.Username, false, false, false, false, true, false, false);
            }
            if (StickerListViewModel.StickerList == null || !StickerListViewModel.StickerList.Any())
            {
                await StickerListViewModel.GetStickerPacks();
            }
            if (MessageGroupCollection == null || !MessageGroupCollection.Any())
            {
                await GetMessageGroups(Shell.Instance.ViewModel.CurrentUser.Username);
            }
            try
            {
                if (state.ContainsKey("Thread"))
                {
                    if (Selected == null)
                    {
                        Selected = JsonConvert.DeserializeObject<MessageGroupItem>(state["Thread"]?.ToString());
                    }
                }
                if (state.ContainsKey("SelectedMessageGroup"))
                {
                    if (SelectedMessageGroup == null)
                    {
                        SelectedMessageGroup = JsonConvert.DeserializeObject<MessageGroup>(state["SelectedMessageGroup"]?.ToString());
                        await GetMessages(SelectedMessageGroup);
                    }
                }
                state.Clear();
            }
            catch (Exception)
            {
                // State didn't save
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

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            Template10.Common.BootStrapper.Current.NavigationService.FrameFacade.BackRequested -= MasterDetailViewControl.NavigationManager_BackRequested;
            try
            {
                if (Selected != null)
                {
                    state["Thread"] = JsonConvert.SerializeObject(Selected);
                    state["SelectedMessageGroup"] = JsonConvert.SerializeObject(SelectedMessageGroup);
                }
            }
            catch (Exception ex)
            {
                ResultChecker.LogEvent("SerializeError", new Dictionary<string, string>() { {"serialization", ex.Message}});
            }
            return Task.CompletedTask;
        }

        public async void PullToRefresh_ListView(object sender, RefreshRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();
            await GetMessageGroups(Shell.Instance.ViewModel.CurrentUser.Username);
            deferral.Complete();
        }

        public async void PullToRefresh_MessageView(object sender, RefreshRequestedEventArgs e)
        {
            if (SelectedMessageGroup != null)
            {
                await GetMessages(SelectedMessageGroup);
            }
        }

        public async void SelectSticker(object sender, ItemClickEventArgs e)
        {
            var sticker = e.ClickedItem as StickerSelection;
            if (sticker == null)
            {
                return;
            }
            StickerFlyout?.Hide();
            IsLoading = true;
            await SendSticker(sticker);
            IsLoading = false;
        }

        public Flyout StickerFlyout { get; set; }

        public async void SelectStickerGroup(object sender, ItemClickEventArgs e)
        {
            try
            {
                var stickerResponse = e.ClickedItem as StickerResponse;
                if (stickerResponse == null) return;
                await StickerListViewModel.GetStickers(stickerResponse);
                FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
            }
            catch (Exception)
            {
                
                //throw;
            }
        }

        public StickersListViewModel StickerListViewModel { get; set; } = new StickersListViewModel();

        public void RemoveImage()
        {
            IsImageAttached = false;
            AttachedImage = null;
        }


        public async Task LoadMessageImage(MessageGroupItem item)
        {
            IsLoading = true;
            await Shell.Instance.ViewModel.UpdateTokens();
            var imageBytes =
                await
                    _messageManager.GetMessageContent(SelectedMessageGroup.MessageGroupId,
                        item.Message.SentMessageId,
                         Shell.Instance.ViewModel.CurrentTokens,
                         Shell.Instance.ViewModel.CurrentUser.Region,
                         Shell.Instance.ViewModel.CurrentUser.Language);
            item.Image = await DecodeImage(imageBytes);
            IsLoading = false;
        }

        private async Task<BitmapImage> DecodeImage(Stream stream)
        {
            var memStream = new MemoryStream();
            await stream.CopyToAsync(memStream);
            memStream.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(memStream.AsRandomAccessStream());
            return bitmapImage;
        }

        public async Task DownloadImageAsync(MessageGroupItem item)
        {
            IsLoading = true;
            var saved = true;
            try
            {
                var imageBytes =
                    await
                        _messageManager.GetMessageContent(SelectedMessageGroup.MessageGroupId,
                            item.Message.SentMessageId,
                            Shell.Instance.ViewModel.CurrentTokens,
                            Shell.Instance.ViewModel.CurrentUser.Region,
                            Shell.Instance.ViewModel.CurrentUser.Language);
                await FileAccessCommands.SaveStreamToCameraRoll(imageBytes, item.Message.SentMessageId + ".png");
            }
            catch (Exception ex)
            {
                //Insights.Report(ex);
                saved = false;
            }

            var loader = new ResourceLoader();

            if (!saved)
            {
                await ResultChecker.SendMessageDialogAsync(loader.GetString("ErrorSaveImage/Text"), false);
            }
            else
            {
                await ResultChecker.SendMessageDialogAsync(loader.GetString("ImageSaved/Text"), true);
            }
            IsLoading = false;
        }

        private MessageGroupItem _selected;

        public MessageGroupItem Selected
        {
            get { return _selected; }
            set
            {
                Set(ref _selected, value);
            }
        }

        public MasterDetailViewControl MasterDetailViewControl { get; set; }

        public MessageGroup SelectedMessageGroup { get; set; }

        private FriendScrollingCollection _friendScrollingCollection;

        public FriendScrollingCollection FriendScrollingCollection
        {
            get { return _friendScrollingCollection; }
            set
            {
                Set(ref _friendScrollingCollection, value);
            }
        }

        public async Task SelectMessageGroup(object sender, ItemClickEventArgs e)
        {
            var selectedMessageGroup = e.ClickedItem as MessageGroupItem;
            string error;
            try
            {
                MessageCollection.Clear();
                await GetMessages(selectedMessageGroup.MessageGroup);
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            await ResultChecker.SendMessageDialogAsync(error, false);
        }

        public async Task GetMessages(MessageGroup messageGroup)
        {
            IsSelected = false;
            //MessageCollection.Clear();
            IsLoading = true;
            try
            {
                SelectedMessageGroup = messageGroup;
                await Shell.Instance.ViewModel.UpdateTokens();
                var messageResult =
                    await
                        _messageManager.GetGroupConversation(messageGroup.MessageGroupId,
                            Shell.Instance.ViewModel.CurrentTokens,
                            Shell.Instance.ViewModel.CurrentUser.Region,
                            Shell.Instance.ViewModel.CurrentUser.Language);
                await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, messageResult);
                var result = await ResultChecker.CheckSuccess(messageResult);
                if (!result)
                {
                    return;
                }
                _messageResponse = JsonConvert.DeserializeObject<MessageResponse>(messageResult.ResultJson);
                var avatarOnlineId = new Dictionary<string, string>();
                foreach (var member in _messageResponse.MessageGroup.MessageGroupDetail.Members)
                {
                    var avatar = await GetAvatarUrl(member.OnlineId);
                    if (avatarOnlineId.All(node => node.Key != member.OnlineId))
                    {
                        avatarOnlineId.Add(member.OnlineId, avatar);
                    }
                }
                var newList =
                    avatarOnlineId.Where(node => node.Key != Shell.Instance.ViewModel.CurrentUser.Username)
                        .Select(node => node.Key);
                GroupMemberSeperatedList = string.Join(", ", newList);
                foreach (
                    var newMessage in
                        _messageResponse.Messages.Where(node => !MessageCollection.Select(test => test.Message.MessageUid).Contains(node.MessageUid)).Reverse().Select(message => new MessageGroupItem { Message = message }))
                {
                    newMessage.AvatarUrl =
                        avatarOnlineId.First(node => node.Key == newMessage.Message.SenderOnlineId).Value;
                    if (newMessage.Message.StickerDetail != null)
                    {
                        newMessage.Message.Body = string.Empty;
                    }
                    newMessage.ImageAvailable = newMessage.Message.ContentKeys.Contains("image-data-0");
                    try
                    {
                        if (newMessage.ImageAvailable)
                        {
                            //var imageBytes =
                            //    await
                            //        _messageManager.GetMessageContent(SelectedMessageGroup.MessageGroupId,
                            //            newMessage.Message.SentMessageId,
                            //            Shell.Instance.ViewModel.CurrentTokens,
                            //            Shell.Instance.ViewModel.CurrentUser.Region,
                            //            Shell.Instance.ViewModel.CurrentUser.Language);
                            //newMessage.Image = await DecodeImage(imageBytes);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Insights.Report(ex);
                    }
                    MessageCollection.Add(newMessage);
                }
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                //Insights.Report(ex);
            }
            var newMessages = _messageResponse?.Messages.Where(node => !node.SeenFlag);
            if (newMessages != null)
            {
                await Shell.Instance.ViewModel.UpdateTokens();
                var messageUids = newMessages.Select(node => node.MessageUid).ToList();
                await
                    _messageManager.ClearMessages(_messageResponse.MessageGroup.MessageGroupId, messageUids,
                        Shell.Instance.ViewModel.CurrentTokens,
                        Shell.Instance.ViewModel.CurrentUser.Region);
            }

            IsLoading = false;
            IsSelected = true;
        }

        public async Task GetMessageGroups(string userName)
        {
            Username = userName;
            IsLoading = true;
            MessageGroupCollection = new ObservableCollection<MessageGroupItem>();
            try
            {
                await Shell.Instance.ViewModel.UpdateTokens();
                var messageResult =
                    await
                        _messageManager.GetMessageGroup(userName, Shell.Instance.ViewModel.CurrentTokens,
                            Shell.Instance.ViewModel.CurrentUser.Region,
                            Shell.Instance.ViewModel.CurrentUser.Language);
                await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, messageResult);
                var result = await ResultChecker.CheckSuccess(messageResult);
                if (!result)
                {
                    return;
                }
                _messageGroupEntity = JsonConvert.DeserializeObject<MessageGroupResponse>(messageResult.ResultJson);
                var avatarOnlineId = new Dictionary<string, string>();
                var avatarOnlinelist = _messageGroupEntity.MessageGroups.Select(node => node.LatestMessage);
                foreach (var member in avatarOnlinelist)
                {
                    var avatar = await GetAvatarUrl(member.SenderOnlineId);
                    if (avatarOnlineId.All(node => node.Key != member.SenderOnlineId))
                    {
                        avatarOnlineId.Add(member.SenderOnlineId, avatar);
                    }
                }
                foreach (
                    var newMessage in
                        _messageGroupEntity.MessageGroups.Select(
                            message => new MessageGroupItem { MessageGroup = message }))
                {
                    newMessage.AvatarUrl =
                        avatarOnlineId.FirstOrDefault(
                            node => node.Key == newMessage.MessageGroup.LatestMessage.SenderOnlineId).Value;
                    MessageGroupCollection.Add(newMessage);
                }
            }
            catch (Exception ex)
            {
                //Insights.Report(ex);
            }
            if (MessageGroupCollection.Count <= 0)
            {
                MessageGroupEmpty = true;
            }
            IsLoading = false;
        }

        private void ScrollToBottom()
        {
            if (ListView.Items != null && ListView.Items.Any())
            {
                var selectedIndex = ListView.Items.Count - 1;
                if (selectedIndex < 0)
                    return;

                ListView.SelectedIndex = selectedIndex;
                ListView.UpdateLayout();

                ListView.ScrollIntoView(ListView.SelectedItem);
            }
        }

        private async Task<string> GetAvatarUrl(string username)
        {
            var userResult =
                await
                    _userManager.GetUserAvatar(username, Shell.Instance.ViewModel.CurrentTokens,
                        Shell.Instance.ViewModel.CurrentUser.Region,
                       Shell.Instance.ViewModel.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, userResult);
            var result = await ResultChecker.CheckSuccess(userResult);
            if (!result)
            {
                return string.Empty;
            }
            if (string.IsNullOrEmpty(userResult.ResultJson))
            {
                return string.Empty;
            }
            var user = JsonConvert.DeserializeObject<User>(userResult.ResultJson);
            if (user?.AvatarUrls != null)
            {
                return user.AvatarUrls.First().AvatarUrlLink;
            }

            return string.Empty;
        }

        public async Task SendMessage()
        {
            string error;
            try
            {
                if (AttachedImage != null && IsImageAttached)
                {
                    await SendMessageWithMedia();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Message))
                        await SendMessageWithoutMedia();
                }

                Message = string.Empty;
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            await ResultChecker.SendMessageDialogAsync(error, false);
        }

        public async void SendMessageCommand(object sender, object e)
        {
            var message = e as string;
            if (string.IsNullOrEmpty(message)) return;
            Message = message;
            await SendMessage();
        }

        private async Task SendMessageWithMedia()
        {
            var realImage = await ConvertToJpeg(AttachedImage);
            var result = new Result();
            if (GroupMembers.Any() && IsNewMessage)
            {
                result =
                    await _messageManager.CreateNewGroupMessageWithMedia(GroupMembers.ToArray(), Message, ImagePath,
                        realImage,
                        Shell.Instance.ViewModel.CurrentTokens, Shell.Instance.ViewModel.CurrentUser.Region);
            }
            else
            {
                if (SelectedMessageGroup == null)
                {
                    await ResultChecker.SendMessageDialogAsync("Selected Message Group Null!", false);
                }
                else
                {
                    result =
                        await
                            _messageManager.CreatePostWithMedia(SelectedMessageGroup.MessageGroupId, Message, ImagePath,
                                realImage,
                                Shell.Instance.ViewModel.CurrentTokens,
                                Shell.Instance.ViewModel.CurrentUser.Region);
                }
            }
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, result);
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (resultCheck)
            {
                IsImageAttached = false;
                AttachedImage = null;
                SelectedMessageGroup = JsonConvert.DeserializeObject<MessageGroup>(result.ResultJson);

                await GetMessages(SelectedMessageGroup);
                //await GetMessageGroups(Shell.Instance.ViewModel.CurrentUser.Username);
                IsNewMessage = false;
            }
        }

        private async Task<Stream> ConvertToJpeg(IRandomAccessStream stream)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var pixels = await decoder.GetPixelDataAsync();

            var outStream = new InMemoryRandomAccessStream();
            // create encoder for saving the tile image
            var propertySet = new BitmapPropertySet();
            // create class representing target jpeg quality - a bit obscure, but it works
            var qualityValue = new BitmapTypedValue(.7, PropertyType.Single);
            propertySet.Add("ImageQuality", qualityValue);

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, outStream);
            encoder.SetPixelData(decoder.BitmapPixelFormat, BitmapAlphaMode.Ignore,
                decoder.OrientedPixelWidth, decoder.OrientedPixelHeight,
                decoder.DpiX, decoder.DpiY,
                pixels.DetachPixelData());
            await encoder.FlushAsync();
            return outStream.AsStream();
        }

        private async Task SendMessageWithoutMedia()
        {
            var result = new Result();
            if (GroupMembers.Any() && IsNewMessage)
            {
                result = await _messageManager.CreateNewGroupMessage(GroupMembers.ToArray(), Message,
                    Shell.Instance.ViewModel.CurrentTokens, Shell.Instance.ViewModel.CurrentUser.Region);
            }
            else
            {
                if (SelectedMessageGroup == null)
                {
                    await ResultChecker.SendMessageDialogAsync("Selected Message Group Null!", false);
                }
                else
                {
                    result = await
                        _messageManager.CreatePost(SelectedMessageGroup.MessageGroupId, Message,
                            Shell.Instance.ViewModel.CurrentTokens,
                            Shell.Instance.ViewModel.CurrentUser.Region);
                }
            }
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, result);
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (resultCheck)
            {
                SelectedMessageGroup = JsonConvert.DeserializeObject<MessageGroup>(result.ResultJson);
                await GetMessages(SelectedMessageGroup);
                //await GetMessageGroups(Shell.Instance.ViewModel.CurrentUser.Username);
                IsNewMessage = false;
            }
        }

        public async Task SendSticker(StickerSelection stickerSelection)
        {
            string error;
            try
            {
                var result = new Result();
                if (GroupMembers.Any() && IsNewMessage)
                {
                    result =
                        await
                            _messageManager.CreateStickerPostWithNewGroupMessage(GroupMembers.ToArray(),
                                stickerSelection.ManifestUrl,
                                stickerSelection.Number.ToString(), stickerSelection.ImageUrl, stickerSelection.PackageId,
                                stickerSelection.Type, Shell.Instance.ViewModel.CurrentTokens,
                                Shell.Instance.ViewModel.CurrentUser.Region);
                }
                else
                {
                    result =
                        await
                            _messageManager.CreateStickerPost(SelectedMessageGroup.MessageGroupId,
                                stickerSelection.ManifestUrl,
                                stickerSelection.Number.ToString(), stickerSelection.ImageUrl, stickerSelection.PackageId,
                                stickerSelection.Type, Shell.Instance.ViewModel.CurrentTokens,
                                Shell.Instance.ViewModel.CurrentUser.Region);
                }

                await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, result);
                var resultCheck = await ResultChecker.CheckSuccess(result);
                if (resultCheck)
                {
                    SelectedMessageGroup = JsonConvert.DeserializeObject<MessageGroup>(result.ResultJson);
                    await GetMessages(SelectedMessageGroup);
                    //await GetMessageGroups(Shell.Instance.ViewModel.CurrentUser.Username);
                    IsNewMessage = false;
                }
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            await ResultChecker.SendMessageDialogAsync(error, false);
        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                Set(ref _isLoading, value);
            }
        }


        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                Set(ref _username, value);
            }
        }

        public ListView ListView { get; set; }
        private readonly MessageManager _messageManager = new MessageManager();
        private readonly UserManager _userManager = new UserManager();
        private string _friendSearch;
        private List<SearchResult> _friendSearchResult;
        private string _groupMemberSeperatedList;
        private bool _isImageAttached;
        private bool _isNewMessage;

        private bool _isOpen;
        private bool _isSelected;
        private string _message;

        private ObservableCollection<MessageGroupItem> _messageCollection =
           new ObservableCollection<MessageGroupItem>();

        private ObservableCollection<MessageGroupItem> _messageGroupCollection =
            new ObservableCollection<MessageGroupItem>();

        private bool _messageGroupEmpty;

        private MessageGroupResponse _messageGroupEntity;
        private MessageResponse _messageResponse;

        private MessageGroup _selectedMessageGroup;

        public List<string> GroupMembers { get; set; } = new List<string>();

        public IRandomAccessStream AttachedImage { get; set; }
        public string ImagePath { get; set; } = "";

        public string GroupMemberSeperatedList
        {
            get { return _groupMemberSeperatedList; }
            set
            {
                Set(ref _groupMemberSeperatedList, value);
            }
        }

        public string FriendSearch
        {
            get { return _friendSearch; }
            set
            {
                Set(ref _friendSearch, value);
            }
        }

        public List<SearchResult> FriendSearchResults
        {
            get { return _friendSearchResult; }
            set
            {
                Set(ref _friendSearchResult, value);
            }
        }

        public MessageResponse MessageResponse
        {
            get { return _messageResponse; }
            set
            {
                Set(ref _messageResponse, value);
            }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                Set(ref _message, value);
            }
        }

        public bool IsImageAttached
        {
            get { return _isImageAttached; }
            set
            {
                Set(ref _isImageAttached, value);
            }
        }

        public bool IsNewMessage
        {
            get { return _isNewMessage; }
            set
            {
                Set(ref _isNewMessage, value);
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                Set(ref _isSelected, value);
            }
        }

        public bool MessageGroupEmpty
        {
            get { return _messageGroupEmpty; }
            set
            {
                Set(ref _messageGroupEmpty, value);
            }
        }

        public ObservableCollection<MessageGroupItem> MessageCollection
        {
            get { return _messageCollection; }
            set
            {
                Set(ref _messageCollection, value);
            }
        }

        public ObservableCollection<MessageGroupItem> MessageGroupCollection
        {
            get { return _messageGroupCollection; }
            set
            {
                Set(ref _messageGroupCollection, value);
            }
        }
    }
}
