using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Common;
using PlayStation_App.Models.FriendFinder;
using PlayStation_App.Models.Response;
using PlayStation_App.Tools.Debug;

namespace PlayStation_App.Commands.Messages
{
    public class FriendFilterSelection : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            var itemClick = parameter as ItemClickEventArgs;
            var searchResult = itemClick?.ClickedItem as SearchResult;
            if (searchResult == null)
            {
                return;
            }

            Locator.ViewModels.MessagesVm.GroupMembers = new List<string>
            {
                searchResult.OnlineId
            };
            Locator.ViewModels.MessagesVm.GroupMemberSeperatedList = searchResult.OnlineId;
            Locator.ViewModels.MessagesVm.SelectedMessageGroup = null;
            Locator.ViewModels.MessagesVm.IsNewMessage = true;
            Locator.ViewModels.MessagesVm.IsSelected = true;
            Locator.ViewModels.MessagesVm.CloseCommand.Execute(null);
        }
    }

    public class FriendFilterButton : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            if (string.IsNullOrEmpty(Locator.ViewModels.MessagesVm.FriendSearch))
            {
                return;
            }
            var friendFinderManager = new FriendFinderManager();
            var friendResult =
                await
                    friendFinderManager.SearchForFriends(Locator.ViewModels.MessagesVm.FriendSearch, Locator.ViewModels.MainPageVm.CurrentTokens,
                        Locator.ViewModels.MainPageVm.CurrentUser.Region,
                        Locator.ViewModels.MainPageVm.CurrentUser.Language);
            var result = await ResultChecker.CheckSuccess(friendResult);
            if (!result)
            {
                return;
            }
            var friendFinderResult = JsonConvert.DeserializeObject<FriendFinderResponse>(friendResult.ResultJson);
            if (friendFinderResult.SearchResults != null)
            {
                Locator.ViewModels.MessagesVm.FriendSearchResults = friendFinderResult.SearchResults.ToList();
            }
        }
    }
    public class FriendMessageFilterOnSuggestedQuery : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            var args = parameter as SearchBoxSuggestionsRequestedEventArgs;
            var deferral = args.Request.GetDeferral();
            string queryText = args?.QueryText;
            if (string.IsNullOrEmpty(queryText)) return;

            var friendFinderManager = new FriendFinderManager();
            var friendResult =
                await
                    friendFinderManager.SearchForFriends(queryText, Locator.ViewModels.MainPageVm.CurrentTokens,
                        Locator.ViewModels.MainPageVm.CurrentUser.Region,
                        Locator.ViewModels.MainPageVm.CurrentUser.Language);
            var result = await ResultChecker.CheckSuccess(friendResult);
            if (!result)
            {
                return;
            }

            var friendFinderResult = JsonConvert.DeserializeObject<FriendFinderResponse>(friendResult.ResultJson);

            var suggestionCollection = args.Request.SearchSuggestionCollection;
            if (friendFinderResult.SearchResults != null)
            {
                Locator.ViewModels.MessagesVm.FriendSearchResults = friendFinderResult.SearchResults.ToList();

                foreach (var friend in friendFinderResult.SearchResults.Take(5))
                {
                    suggestionCollection.AppendQuerySuggestion(friend.OnlineId);
                }
            }
            deferral.Complete();
        }
    }

    public class FriendMessageFilterOnSubmittedQuery : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            var args = parameter as SearchBoxQuerySubmittedEventArgs;
            string queryText = args?.QueryText;
            if (string.IsNullOrEmpty(queryText)) return;

            var friendFinderManager = new FriendFinderManager();
            var friendResult =
                await
                    friendFinderManager.SearchForFriends(queryText, Locator.ViewModels.MainPageVm.CurrentTokens,
                        Locator.ViewModels.MainPageVm.CurrentUser.Region,
                        Locator.ViewModels.MainPageVm.CurrentUser.Language);
            var result = await ResultChecker.CheckSuccess(friendResult);
            if (!result)
            {
                return;
            }
            var friendFinderResult = JsonConvert.DeserializeObject<FriendFinderResponse>(friendResult.ResultJson);
            if (!friendFinderResult.SearchResults.Any())
            {
                return;
            }

            Locator.ViewModels.MessagesVm.GroupMembers = new List<string>
            {
                friendFinderResult.SearchResults.First().OnlineId
            };
            Locator.ViewModels.MessagesVm.IsNewMessage = true;

        }
    }
    public class FriendFilterOnChangedQuery : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            var args = parameter as SearchBoxQueryChangedEventArgs;
            if (args == null)
            {
                return;
            }
            //var vm = Locator.ViewModels.SmiliesPageVm;
            //string queryText = args.QueryText;
            //if (string.IsNullOrEmpty(queryText))
            //{
            //    vm.SmileCategoryList = vm.FullSmileCategoryEntities;
            //    return;
            //}
            //var result = vm.SmileCategoryList.SelectMany(
            //    smileCategory => smileCategory.SmileList.Where(smile => smile.Title.Equals(queryText))).FirstOrDefault();
            //if (result != null) return;
            //var searchList = vm.FullSmileCategoryEntities.SelectMany(
            //    smileCategory => smileCategory.SmileList.Where(smile => smile.Title.Contains(queryText)));
            //List<SmileEntity> smileListEntities = searchList.ToList();
            //var searchSmileCategory = new SmileCategoryEntity()
            //{
            //    Name = "Search",
            //    SmileList = smileListEntities
            //};
            //var fakeSmileCategoryList = new List<SmileCategoryEntity> { searchSmileCategory };
            //vm.SmileCategoryList = fakeSmileCategoryList.ToObservableCollection();
        }
    }
}
