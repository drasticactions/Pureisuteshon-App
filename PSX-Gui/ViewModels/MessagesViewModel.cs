using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Kimono.Controls;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Models.FriendFinder;
using PlayStation_App.Models.MessageGroups;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.User;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_App.ViewModels;
using PlayStation_Gui.Views;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class MessagesViewModel : ViewModelBase
    {
        public override async void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Template10.Common.BootStrapper.Current.NavigationService.FrameFacade.BackRequested += MasterDetailViewControl.NavigationManager_BackRequested;
            MasterDetailViewControl.LoadLayout();
            if (MessageGroupCollection != null || !MessageGroupCollection.Any())
            {
                await GetMessageGroups(Shell.Instance.ViewModel.CurrentUser.Username);
            }
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            Template10.Common.BootStrapper.Current.NavigationService.FrameFacade.BackRequested -= MasterDetailViewControl.NavigationManager_BackRequested;
            if (Selected != null)
            {
                state[nameof(Selected)] = JsonConvert.SerializeObject(Selected);
            }
            return Task.CompletedTask;
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

        public async Task SelectMessageGroup(object sender, ItemClickEventArgs e)
        {
            var selectedMessageGroup = e.ClickedItem as MessageGroupItem;
            string error;
            try
            {
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
            MessageCollection.Clear();
            IsLoading = true;
            try
            {
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
                            //var imageBytes =
                            //    await
                            //        _messageManager.GetMessageContent(SelectedMessageGroup.MessageGroupId,
                            //            newMessage.Message.SentMessageId,
                            //            Locator.ViewModels.MainPageVm.CurrentTokens,
                            //            Locator.ViewModels.MainPageVm.CurrentUser.Region,
                            //            Locator.ViewModels.MainPageVm.CurrentUser.Language);
                            //newMessage.Image = await DecodeImage(imageBytes);
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
            var newMessages = _messageResponse?.Messages.Where(node => !node.SeenFlag);
            if (newMessages != null)
            {
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
