using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Common;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.User;
using PlayStation_App.Tools.Helpers;
using Xamarin;

namespace PlayStation_App.ViewModels
{
    public class MessagesViewModel : NotifierBase
    {
        private ObservableCollection<MessageGroupItem> _messageGroupCollection =
            new ObservableCollection<MessageGroupItem>();

        private ObservableCollection<MessageGroupItem> _messageCollection =
            new ObservableCollection<MessageGroupItem>();

        private bool _messageGroupEmpty;
        private bool _isSelected;
        private bool _isImageAttached;
        private MessageResponse _messageResponse;

        private MessageGroupResponse _messageGroupEntity;

        public MessageResponse MessageResponse
        {
            get { return _messageResponse; }
            set
            {
                SetProperty(ref _messageResponse, value);
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

        public async Task GetMessages(string id)
        {
            IsSelected = false;
            MessageCollection = new ObservableCollection<MessageGroupItem>();
            IsLoading = true;
            try
            {
                var messageResult =
               await _messageManager.GetGroupConversation(id, Locator.ViewModels.MainPageVm.CurrentTokens);
                await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, messageResult);
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
                                    _messageManager.GetMessageContent(id, newMessage.Message.SentMessageId,
                                        Locator.ViewModels.MainPageVm.CurrentTokens);
                            newMessage.Image = await DecodeImage(imageBytes);
                        }
                    }
                    catch (Exception ex)
                    {
                        Insights.Report(ex);
                    }
                    MessageCollection.Add(newMessage);
                }
            }
            catch (Exception ex)
            {
                Insights.Report(ex);
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

        public async Task GetMessageGroups(string userName)
        {
            IsLoading = true;
            MessageGroupCollection = new ObservableCollection<MessageGroupItem>();
            
            try
            {
                var messageResult = await _messageManager.GetMessageGroup(userName, Locator.ViewModels.MainPageVm.CurrentTokens);
                await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, messageResult);
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
                Insights.Report(ex);
            }
            if (MessageGroupCollection.Count <= 0)
            {
                MessageGroupEmpty = true;
            }
            IsLoading = false;
        }

        private async Task<string> GetAvatarUrl(string username)
        {
            var userResult = await _userManager.GetUserAvatar(username, Locator.ViewModels.MainPageVm.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, userResult);
            var user = JsonConvert.DeserializeObject<User>(userResult.ResultJson);
            if (user?.AvatarUrls != null)
            {
                return user.AvatarUrls.First().AvatarUrlLink;
            }

            return string.Empty;
        }

    }
}
