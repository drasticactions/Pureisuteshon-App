using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation_App.Commands.Messages;
using PlayStation_App.Common;
using PlayStation_App.Models.FriendFinder;
using PlayStation_App.Models.MessageGroups;
using PlayStation_App.Models.Messages;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.Sticker;
using PlayStation_App.Models.User;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;


namespace PlayStation_App.ViewModels
{
    public class MessagesViewModel : NotifierBase
    {
        private ObservableCollection<MessageGroupItem> _messageGroupCollection =
            new ObservableCollection<MessageGroupItem>();

        private ObservableCollection<MessageGroupItem> _messageCollection =
            new ObservableCollection<MessageGroupItem>();

        private MessageGroup _selectedMessageGroup;
        private bool _messageGroupEmpty;
        private bool _isSelected;
        private bool _isNewMessage;
        private bool _isImageAttached;
        private string _message;
        private string _friendSearch;
        private List<SearchResult> _friendSearchResult; 
        public List<string> NewGroupMembers { get; set; } = new List<string>();
        public NavigateToStickersListView NavigateToStickersListCommand { get; set; } = new NavigateToStickersListView();
        private MessageResponse _messageResponse;

        public FriendFilterOnChangedQuery FriendFilterOnChangedQuery { get; set; } = new FriendFilterOnChangedQuery();

        public FriendMessageFilterOnSubmittedQuery FriendMessageFilterOnSubmittedQuery { get; set; } = new FriendMessageFilterOnSubmittedQuery();

        public FriendMessageFilterOnSuggestedQuery FriendMessageFilterOnSuggestedQuery { get; set; } = new FriendMessageFilterOnSuggestedQuery();
        public FriendFilterButton FriendFilterButton { get; set; } = new FriendFilterButton();
        public AttachImageCommand AttachImageCommand { get; set; } = new AttachImageCommand();
        public RemoveImageCommand RemoveImageCommand { get; set; } = new RemoveImageCommand();
        public SendMessageCommand SendMessageCommand { get; set; } = new SendMessageCommand();
        public DownloadImage DownloadImage { get; set; } = new DownloadImage();

        public ViewImage ViewImage { get; set; } = new ViewImage();

        private MessageGroupResponse _messageGroupEntity;
        public IRandomAccessStream AttachedImage { get; set; }
        public string ImagePath { get; set; } = "";
        public string FriendSearch
        {
            get { return _friendSearch; }
            set
            {
                SetProperty(ref _friendSearch, value);
                OnPropertyChanged();
            }
        }
        public List<SearchResult> FriendSearchResults
        {
            get { return _friendSearchResult; }
            set
            {
                SetProperty(ref _friendSearchResult, value);
                OnPropertyChanged();
            }
        }
        public MessageResponse MessageResponse
        {
            get { return _messageResponse; }
            set
            {
                SetProperty(ref _messageResponse, value);
                OnPropertyChanged();
            }
        }
        public string Message
        {
            get { return _message; }
            set
            {
                SetProperty(ref _message, value);
                OnPropertyChanged();
            }
        }
        public bool IsImageAttached
        {
            get { return _isImageAttached; }
            set
            {
                SetProperty(ref _isImageAttached, value);
                OnPropertyChanged();
            }
        }
        public bool IsNewMessage
        {
            get { return _isNewMessage; }
            set
            {
                SetProperty(ref _isNewMessage, value);
                OnPropertyChanged();
            }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
                OnPropertyChanged();
            }
        }
        public bool MessageGroupEmpty
        {
            get { return _messageGroupEmpty; }
            set
            {
                SetProperty(ref _messageGroupEmpty, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MessageGroupItem> MessageCollection
        {
            get { return _messageCollection; }
            set
            {
                SetProperty(ref _messageCollection, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MessageGroupItem> MessageGroupCollection
        {
            get { return _messageGroupCollection; }
            set
            {
                SetProperty(ref _messageGroupCollection, value);
                OnPropertyChanged();
            }
        }
        private readonly MessageManager _messageManager = new MessageManager();
        private readonly UserManager _userManager = new UserManager();

        public MessageGroup SelectedMessageGroup { get; set; }

        public async Task GetMessages(MessageGroup messageGroup)
        {
            SelectedMessageGroup = messageGroup;
            IsSelected = false;
            MessageCollection.Clear();
            IsLoading = true;
            try
            {
                _selectedMessageGroup = messageGroup;
                var messageResult =
               await _messageManager.GetGroupConversation(messageGroup.MessageGroupId, Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
                await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, messageResult);
                var result = await ResultChecker.CheckSuccess(messageResult);
                if (!result)
                {
                    return;
                }
                _messageResponse = JsonConvert.DeserializeObject<MessageResponse>(messageResult.ResultJson);
                var avatarOnlineId = new Dictionary<string, string>();
                foreach(var member in messageGroup.MessageGroupDetail.Members)
                {
                    var avatar = await GetAvatarUrl(member.OnlineId);
                    if(avatarOnlineId.All(node => node.Key != member.OnlineId))
                        avatarOnlineId.Add(member.OnlineId, avatar);
                }
                foreach (
    var newMessage in
        _messageResponse.Messages.Reverse().Select(message => new MessageGroupItem { Message = message }))
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
                            var imageBytes =
                                await
                                    _messageManager.GetMessageContent(_selectedMessageGroup.MessageGroupId, newMessage.Message.SentMessageId,
                                        Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
                            newMessage.Image = await DecodeImage(imageBytes);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Insights.Report(ex);
                    }
                    MessageCollection.Add(newMessage);
                }
            }
            catch (Exception ex)
            {
                //Insights.Report(ex);
            }
            IsLoading = false;
            IsSelected = true;
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

        public string Username { get; set; }

        public async Task GetMessageGroups(string userName)
        {
            Username = userName;
            IsLoading = true;
            MessageGroupCollection = new ObservableCollection<MessageGroupItem>();
            
            try
            {
                var messageResult = await _messageManager.GetMessageGroup(userName, Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
                await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, messageResult);
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
                        avatarOnlineId.Add(member.SenderOnlineId, avatar);
                }
                foreach (
    var newMessage in
        _messageGroupEntity.MessageGroups.Select(message => new MessageGroupItem { MessageGroup = message }))
                {
                    newMessage.AvatarUrl = avatarOnlineId.FirstOrDefault(node => node.Key == newMessage.MessageGroup.LatestMessage.SenderOnlineId).Value;
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

        private async Task<string> GetAvatarUrl(string username)
        {
            var userResult = await _userManager.GetUserAvatar(username, Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, userResult);
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
            if (AttachedImage != null && IsImageAttached)
            {
                await SendMessageWithMedia();
            }
            else
            {
                await SendMessageWithoutMedia();
            }

            Message = string.Empty;
        }

        private async Task SendMessageWithMedia()
        {
           var realImage = await ConvertToJpeg(AttachedImage);
            var result = new Result();
            if (NewGroupMembers.Any() && IsNewMessage)
            {
                result = await _messageManager.CreateNewGroupMessageWithMedia(NewGroupMembers.ToArray(), Message, ImagePath,
                     realImage,
                     Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region);
            }
            else
            {
                if (_selectedMessageGroup == null)
                {
                    await ResultChecker.SendMessageDialogAsync("Selected Message Group Null!", false);
                }
                else
                {
                    result = await _messageManager.CreatePostWithMedia(_selectedMessageGroup.MessageGroupId, Message, ImagePath,
                     realImage,
                     Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region);
                }
            }
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, result);
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (resultCheck)
            {
                Locator.ViewModels.MessagesVm.IsImageAttached = false;
                Locator.ViewModels.MessagesVm.AttachedImage = null;
                _selectedMessageGroup = JsonConvert.DeserializeObject<MessageGroup>(result.ResultJson);
                await GetMessages(_selectedMessageGroup);
                IsNewMessage = false;
            }
        }

        private async Task<Stream> ConvertToJpeg(IRandomAccessStream stream)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var pixels = await decoder.GetPixelDataAsync();

            var outStream = new InMemoryRandomAccessStream();
            // create encoder for saving the tile image
            BitmapPropertySet propertySet = new BitmapPropertySet();
            // create class representing target jpeg quality - a bit obscure, but it works
            BitmapTypedValue qualityValue = new BitmapTypedValue(.7, PropertyType.Single);
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
            if (NewGroupMembers.Any() && IsNewMessage)
            {
                result = await _messageManager.CreateNewGroupMessage(NewGroupMembers.ToArray(), Message,
                  Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region);
            }
            else
            {
                if (_selectedMessageGroup == null)
                {
                    await ResultChecker.SendMessageDialogAsync("Selected Message Group Null!", false);
                }
                else
                {
                    result = await
               _messageManager.CreatePost(_selectedMessageGroup.MessageGroupId, Message,
                   Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region);
                }
            }
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, result);
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (resultCheck)
            {
                _selectedMessageGroup = JsonConvert.DeserializeObject<MessageGroup>(result.ResultJson);
                await GetMessages(_selectedMessageGroup);
                IsNewMessage = false;
            }
        }

        public async Task SendSticker(StickerSelection stickerSelection)
        {
            var result = new Result();
            if (NewGroupMembers.Any() && IsNewMessage)
            {
                result =
                await
                    _messageManager.CreateStickerPostWithNewGroupMessage(NewGroupMembers.ToArray(), stickerSelection.ManifestUrl,
                        stickerSelection.Number.ToString(), stickerSelection.ImageUrl, stickerSelection.PackageId,
                        stickerSelection.Type, Locator.ViewModels.MainPageVm.CurrentTokens,
                        Locator.ViewModels.MainPageVm.CurrentUser.Region);
            }
            else
            {
                result =
                await
                    _messageManager.CreateStickerPost(_selectedMessageGroup.MessageGroupId, stickerSelection.ManifestUrl,
                        stickerSelection.Number.ToString(), stickerSelection.ImageUrl, stickerSelection.PackageId,
                        stickerSelection.Type, Locator.ViewModels.MainPageVm.CurrentTokens,
                        Locator.ViewModels.MainPageVm.CurrentUser.Region);
            }
                
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, result);
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (resultCheck)
            {
                _selectedMessageGroup = JsonConvert.DeserializeObject<MessageGroup>(result.ResultJson);
                await GetMessages(_selectedMessageGroup);
                IsNewMessage = false;
            }
        }



        public async Task DownloadImageAsync(MessageGroupItem item)
        {
            IsLoading = true;
            var saved = true;
            try
            {
                var imageBytes =
                                await
                                    _messageManager.GetMessageContent(_selectedMessageGroup.MessageGroupId, item.Message.SentMessageId,
                                        Locator.ViewModels.MainPageVm.CurrentTokens, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
                await FileAccessCommands.SaveStreamToCameraRoll(imageBytes, item.Message.SentMessageId + ".png");
            }
            catch (Exception ex)
            {
                //Insights.Report(ex);
                saved = false;
            }

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

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
    }
}
