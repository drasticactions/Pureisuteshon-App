using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.User;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_App.Tools.ScrollingCollection;
using PlayStation_App.ViewModels;
using PlayStation_Gui.Views;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class FriendViewModel : ViewModelBase
    {
        public FriendViewModel()
        {
            FriendScrollingCollection = new FriendScrollingCollection();
            RecentActivityScrollingCollection = new RecentActivityScrollingCollection();
        }
        private FriendScrollingCollection _friendScrollingCollection;
        private MessageResponse _messageEntity;

        private ObservableCollection<MessageGroupItem> _messageGroupCollection =
            new ObservableCollection<MessageGroupItem>();

        private RecentActivityScrollingCollection _recentActivityScrollingCollection;
        private TrophyScrollingCollection _trophyScrollingCollection;
        private UserViewModel _userViewModel;

        public bool IsSelected => UserModel != null;

        public UserViewModel UserModel
        {
            get { return _userViewModel; }
            set
            {
                Set(ref _userViewModel, value);
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

        public FriendScrollingCollection FriendScrollingCollection
        {
            get { return _friendScrollingCollection; }
            set
            {
                Set(ref _friendScrollingCollection, value);
            }
        }

        public RecentActivityScrollingCollection RecentActivityScrollingCollection
        {
            get { return _recentActivityScrollingCollection; }
            set
            {
                Set(ref _recentActivityScrollingCollection, value);
            }
        }

        public TrophyScrollingCollection TrophyScrollingCollection
        {
            get { return _trophyScrollingCollection; }
            set
            {
                Set(ref _trophyScrollingCollection, value);
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

        public void SetRecentActivityFeed(string userName)
        {
            RecentActivityScrollingCollection = new RecentActivityScrollingCollection
            {
                IsNews = false,
                StorePromo = false,
                Username = userName,
                PageCount = 0
            };
        }

        public void SetTrophyList(string userName)
        {
            TrophyScrollingCollection = new TrophyScrollingCollection
            {
                CompareUsername = Shell.Instance.ViewModel.CurrentUser.Username,
                Username = userName,
                Offset = 0
            };
        }

        public void SelectFriend(object sender, ItemClickEventArgs e)
        {
            Template10.Common.BootStrapper.Current.NavigationService.Navigate(typeof(FriendPage), JsonConvert.SerializeObject(e.ClickedItem));
        }

        public async Task SetUser(string userName)
        {
            var isCurrentUser = Shell.Instance.ViewModel.CurrentUser.Username.Equals(userName);
            var userManager = new UserManager();
            var userResult = await userManager.GetUser(userName, Shell.Instance.ViewModel.CurrentTokens, Shell.Instance.ViewModel.CurrentUser.Region, Shell.Instance.ViewModel.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, userResult);
            var result = await ResultChecker.CheckSuccess(userResult);
            if (!result)
            {
                return;
            }
            if (string.IsNullOrEmpty(userResult.ResultJson))
            {
                return;
            }
            var user = JsonConvert.DeserializeObject<User>(userResult.ResultJson);
            if (user == null) return;
            var list = user.TrophySummary.EarnedTrophies;
            user.TrophySummary.TotalTrophies = list.Bronze + list.Gold + list.Platinum + list.Silver;
            List<string> languageList = user.LanguagesUsed.Select(ParseLanguageVariable).ToList();
            string language = string.Join("," + Environment.NewLine, languageList);
            UserModel = new UserViewModel
            {
                Language = language,
                User = user,
                IsNotCurrentUser = !isCurrentUser,
                CurrentUserOnlineId = Shell.Instance.ViewModel.CurrentUser.Username
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
    }
}
