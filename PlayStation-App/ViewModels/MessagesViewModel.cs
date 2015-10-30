using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Commands.Messages;
using PlayStation_App.Common;
using PlayStation_App.Models.MessageGroups;
using PlayStation_App.Models.Response;
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
        private bool _isImageAttached;
        private string _message;
        private MessageResponse _messageResponse;
        public AttachImageCommand AttachImageCommand { get; set; } = new AttachImageCommand();
        public RemoveImageCommand RemoveImageCommand { get; set; } = new RemoveImageCommand();
        public SendMessageCommand SendMessageCommand { get; set; } = new SendMessageCommand();
        public DownloadImage DownloadImage { get; set; } = new DownloadImage();

        public ViewImage ViewImage { get; set; } = new ViewImage();

        private MessageGroupResponse _messageGroupEntity;
        public byte[] AttachedImage { get; set; }
        public string ImagePath { get; set; } = "";
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
            MessageCollection = new ObservableCollection<MessageGroupItem>();
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
                foreach (
    var newMessage in
        _messageResponse.Messages.Reverse().Select(message => new MessageGroupItem { Message = message }))
                {
                    newMessage.AvatarUrl = await GetAvatarUrl(newMessage.Message.SenderOnlineId);
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
                foreach (
    var newMessage in
        _messageGroupEntity.MessageGroups.Select(message => new MessageGroupItem { MessageGroup = message }))
                {
                    newMessage.AvatarUrl = await GetAvatarUrl(newMessage.MessageGroup.LatestMessage.SenderOnlineId);
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
        }

        private async Task SendMessageWithMedia()
        {
           var result = await _messageManager.CreatePostWithMedia(_selectedMessageGroup.MessageGroupId, Message, ImagePath,
                                AttachedImage,
                                Locator.ViewModels.MainPageVm.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, result);
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (resultCheck)
            {
                await GetMessages(_selectedMessageGroup);
            }
        }

        private async Task SendMessageWithoutMedia()
        {
            var result = await
                            _messageManager.CreatePost(_selectedMessageGroup.MessageGroupId, Message,
                                Locator.ViewModels.MainPageVm.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, result);
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (resultCheck)
            {
                await GetMessages(_selectedMessageGroup);
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
