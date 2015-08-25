using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using PlayStation_App.Common;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Entities.Message;
using PlayStation_App.Core.Entities.User;
using PlayStation_App.Core.Managers;
using PlayStation_App.Tools.ScrollingCollection;

namespace PlayStation_App.ViewModels
{
    public class FriendPageViewModel : NotifierBase
    {
        private FriendScrollingCollection _friendScrollingCollection;
        private MessageEntity _messageEntity;

        private ObservableCollection<MessageGroupItem> _messageGroupCollection =
            new ObservableCollection<MessageGroupItem>();

        private RecentActivityScrollingCollection _recentActivityScrollingCollection;
        private TrophyScrollingCollection _trophyScrollingCollection;
        private UserViewModel _userViewModel;

        public UserViewModel UserModel
        {
            get { return _userViewModel; }
            set
            {
                SetProperty(ref _userViewModel, value);
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

        public FriendScrollingCollection FriendScrollingCollection
        {
            get { return _friendScrollingCollection; }
            set
            {
                SetProperty(ref _friendScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public RecentActivityScrollingCollection RecentActivityScrollingCollection
        {
            get { return _recentActivityScrollingCollection; }
            set
            {
                SetProperty(ref _recentActivityScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public TrophyScrollingCollection TrophyScrollingCollection
        {
            get { return _trophyScrollingCollection; }
            set
            {
                SetProperty(ref _trophyScrollingCollection, value);
                OnPropertyChanged();
            }
        }

        public Visibility SetFriendRequestVisibility()
        {
            if (UserModel.User == null) return Visibility.Collapsed;
            if (string.IsNullOrEmpty(UserModel.User.Relation))
                return Visibility.Collapsed;
            if (UserModel.User.Relation.Equals("friend of friends") || UserModel.User.Relation.Equals("no relationship"))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public Visibility SetAddFriendVisibility()
        {
            if (UserModel.User == null) return Visibility.Collapsed;
            if (string.IsNullOrEmpty(UserModel.User.Relation))
                return Visibility.Collapsed;
            return UserModel.User.Relation.Equals("requested friend") ? Visibility.Visible : Visibility.Collapsed;
        }


        public async void SetMessages(string userName, UserAccountEntity userAccountEntity)
        {
            MessageGroupCollection = new ObservableCollection<MessageGroupItem>();
            var messageManager = new MessageManager();
            _messageEntity =
                await
                    messageManager.GetGroupConversation(
                        string.Format("~{0},{1}", userName, Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId),
                         Locator.ViewModels.MainPageVm.CurrentUser);
            if (_messageEntity?.messages == null)
                return;
            foreach (
                MessageGroupItem newMessage in
                    _messageEntity.messages.Select(message => new MessageGroupItem { Message = message }))
            {
                //GetAvatar(newMessage, userAccountEntity);
                MessageGroupCollection.Add(newMessage);
            }
        }

        private async void GetAvatar(MessageGroupItem message, UserAccountEntity userAccountEntity)
        {
            var userManager = new UserManager();
            UserEntity user = await userManager.GetUserAvatar(message.Message.senderOnlineId, userAccountEntity);
            if (user == null) return;
            message.AvatarUrl = user.DefaultAvatarUrl;
            OnPropertyChanged("MessageGroupCollection");
        }

        public void SetFriendsList(string userName, bool onlineFilter, bool blockedPlayer, bool recentlyPlayed,
            bool personalDetailSharing, bool friendStatus, bool requesting, bool requested)
        {
            FriendScrollingCollection = new FriendScrollingCollection
            {
                UserAccountEntity = Locator.ViewModels.MainPageVm.CurrentUser,
                Offset = 0,
                OnlineFilter = onlineFilter,
                Requested = requested,
                Requesting = requesting,
                PersonalDetailSharing = personalDetailSharing,
                FriendStatus = friendStatus,
                Username = userName
            };
        }

        public void SetRecentActivityFeed(string userName)
        {
            RecentActivityScrollingCollection = new RecentActivityScrollingCollection
            {
                IsNews = false,
                StorePromo = false,
                UserAccountEntity = Locator.ViewModels.MainPageVm.CurrentUser,
                Username = userName,
                PageCount = 0
            };
        }

        public void SetTrophyList(string userName)
        {
            TrophyScrollingCollection = new TrophyScrollingCollection
            {
                UserAccountEntity = Locator.ViewModels.MainPageVm.CurrentUser,
                Username = userName,
                Offset = 0
            };
        }

        public async Task SetUser(string userName)
        {
            bool isCurrentUser = Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId.Equals(userName);
            var userManager = new UserManager();
            UserEntity user = await userManager.GetUser(userName, Locator.ViewModels.MainPageVm.CurrentUser);
            if (user == null) return;
            List<string> languageList = user.LanguagesUsed.Select(ParseLanguageVariable).ToList();
            string language = string.Join("," + Environment.NewLine, languageList);
            UserModel = new UserViewModel
            {
                Language = language,
                User = user,
                IsNotCurrentUser = !isCurrentUser,
                CurrentUserOnlineId = Locator.ViewModels.MainPageVm.CurrentUser.GetUserEntity().OnlineId
            };
        }

        private static string ParseLanguageVariable(string language)
        {
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            switch (language)
            {
                case "ja":
                    return resourceLoader.GetString("LangJapanese/Text").Trim();
                case "dk":
                    return resourceLoader.GetString("LangDanish/Text").Trim();
                case "de":
                    return resourceLoader.GetString("LangGerman/Text").Trim();
                case "en":
                    return resourceLoader.GetString("LangEnglishUS/Text").Trim();
                case "en-GB":
                    return resourceLoader.GetString("LangEnglishUK/Text").Trim();
                case "fi":
                    return resourceLoader.GetString("LangFinnish/Text").Trim();
                case "fr":
                    return resourceLoader.GetString("LangFrench/Text").Trim();
                case "es":
                    return resourceLoader.GetString("LangSpanishSpain/Text").Trim();
                case "es-MX":
                    return resourceLoader.GetString("LangSpanishLA/Text").Trim();
                case "it":
                    return resourceLoader.GetString("LangItalian/Text").Trim();
                case "nl":
                    return resourceLoader.GetString("LangDutch/Text").Trim();
                case "pt":
                    return resourceLoader.GetString("LangPortuguesePortugal/Text").Trim();
                case "pt-BR":
                    return resourceLoader.GetString("LangPortugueseBrazil/Text").Trim();
                case "ru":
                    return resourceLoader.GetString("LangRussian/Text").Trim();
                case "pl":
                    return resourceLoader.GetString("LangPolish/Text").Trim();
                case "no":
                    return resourceLoader.GetString("LangNorwegian/Text").Trim();
                case "sv":
                    return resourceLoader.GetString("LangSwedish/Text").Trim();
                case "tr":
                    return resourceLoader.GetString("LangTurkish/Text").Trim();
                case "ko":
                    return resourceLoader.GetString("LangKorean/Text").Trim();
                case "zh-CN":
                    return resourceLoader.GetString("LangChineseSimplified/Text").Trim();
                case "zh-TW":
                    return resourceLoader.GetString("LangChineseTraditional/Text").Trim();
                default:
                    return null;
            }
        }

        /// <summary>
        ///     TODO: Seperate to new class, use ISupportIncrementalLoading
        /// </summary>
        public class MessageGroupItem : NotifierBase
        {
            private string _avatarUrl;

            public string AvatarUrl
            {
                get { return _avatarUrl; }
                set
                {
                    SetProperty(ref _avatarUrl, value);
                    OnPropertyChanged();
                }
            }

            public MessageEntity.Message Message { get; set; }

            public MessageEntity.MessageGroup MessageGroup { get; set; }
        }

        public class UserViewModel : NotifierBase
        {
            public UserEntity User { get; set; }

            public string CurrentUserOnlineId { get; set; }

            public string Language { get; set; }

            public bool IsNotCurrentUser { get; set; }
        }
    }
}
