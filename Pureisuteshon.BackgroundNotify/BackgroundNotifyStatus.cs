using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Newtonsoft.Json;
using PlayStation.Entities.User;
using PlayStation.Entities.Web;
using PlayStation.Managers;
using PlayStation_App.Database;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.User;
using Pureisuteshon.Notifications;
using SQLite.Net.Platform.WinRT;


namespace Pureisuteshon.BackgroundNotify
{
    public sealed class BackgroundNotifyStatus : IBackgroundTask
    {
        private WebManager _webManager;
        readonly Template10.Services.SettingsService.ISettingsHelper _helper;
        private readonly AuthenticationManager _authManager = new AuthenticationManager();
        private readonly UserAccountDatabase AccountDatabase = new UserAccountDatabase(new SQLitePlatformWinRT(), Path.Combine(ApplicationData.Current.LocalFolder.Path, "UserDatabase"));
        private AccountUser _accountUser;
        public BackgroundNotifyStatus()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();


            try
            {
                if (_helper.Read<bool>("BackgroundEnable", false))
                {
                    if (NotifyStatusTile.IsInternet())
                    {
                        var result = await LoginUser();
                        if(result)
                            await Update(taskInstance);
                    }
                }

            }
            catch (Exception)
            {
            }
            deferral.Complete();
        }

        private async Task<bool> LoginUser()
        {
            try
            {
                var defaultUsers = await AccountDatabase.GetDefaultUserAccounts();
                if (!defaultUsers.Any()) return false;
                var defaultUser = defaultUsers.First();
                var loginResult = await LoginTest(defaultUser);
                if (loginResult)
                {
                    _accountUser = defaultUser;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> LoginTest(AccountUser user)
        {
            Result result = new Result();
            try
            {
                result = await _authManager.RefreshAccessToken(user.RefreshToken);
                var tokenResult = JsonConvert.DeserializeObject<Tokens>(result.Tokens);
                user.AccessToken = tokenResult.AccessToken;
                user.RefreshToken = tokenResult.RefreshToken;
                user.RefreshDate = GetUnixTime(DateTime.Now) + (300);
                result.IsSuccess = true;
                await AccountDatabase.UpdateAccountUser(user);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error = ex.Message;
            }
            return result.IsSuccess;
        }

        private static long GetUnixTime(DateTime time)
        {
            time = time.ToUniversalTime();
            TimeSpan timeSpam = time - (new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local));
            return (long)timeSpam.TotalSeconds;
        }

        private async Task Update(IBackgroundTaskInstance taskInstance)
        {
            NotifyStatusTile.ClearCurrentTiles();
            if (_helper.Read<bool>("RecentActivityBackground", false))
            {
                var recentActivityManager = new RecentActivityManager();
                var feedResultEntity =
                await recentActivityManager.GetActivityFeed(_accountUser.Username, 0, 
                true, true, new UserAuthenticationEntity(_accountUser.AccessToken, 
                _accountUser.RefreshToken, _accountUser.RefreshDate), 
                _accountUser.Region, _accountUser.Language);
                if (string.IsNullOrEmpty(feedResultEntity?.ResultJson))
                {
                    // No Items, return false.
                    return;
                }
                var feedEntity = JsonConvert.DeserializeObject<RecentActivityResponse>(feedResultEntity.ResultJson);
                if (!feedEntity.Feed.Any())
                {
                    return;
                }
                var feeds = feedEntity.Feed.Take(5);
                foreach (var feed in feeds)
                {
                    NotifyStatusTile.CreateRecentActvityLiveTile(feed);
                }
            }

            //var newbookmarkthreads = new List<Thread>();
            //try
            //{
            //    var pageNumber = 1;
            //    var hasItems = false;
            //    while (!hasItems)
            //    {
            //        var bookmarkResult = await _threadManager.GetBookmarksAsync(pageNumber);
            //        var bookmarks = JsonConvert.DeserializeObject<List<Thread>>(bookmarkResult.ResultJson);
            //        if (!bookmarks.Any())
            //        {
            //            hasItems = true;
            //        }
            //        else
            //        {
            //            pageNumber++;
            //        }
            //        newbookmarkthreads.AddRange(bookmarks);
            //    }
            //    _helper.Read<DateTime>("LastRefresh", DateTime.UtcNow);
            //    await _bdb.RefreshBookmarkedThreads(newbookmarkthreads);
            //    newbookmarkthreads = await _bdb.GetBookmarkedThreadsFromDb();
            //}
            //catch (Exception ex)
            //{
            //    //AwfulDebugger.SendMessageDialogAsync("Failed to get Bookmarks", ex);
            //}

            //if (!newbookmarkthreads.Any())
            //{
            //    return;
            //}

            //if (_helper.Read<bool>("BookmarkBackground", false))
            //{
            //    CreateBookmarkLiveTiles(newbookmarkthreads);
            //}

            //if (_helper.Read<bool>("BookmarkNotifications", false))
            //{
            //    var notifyList = newbookmarkthreads.Where(node => node.IsNotified);
            //    CreateToastNotifications(notifyList);
            //}
        }

        private void CreateToastNotifications()
        {
            //foreach (var thread in forumThreads.Where(thread => thread.RepliesSinceLastOpened > 0))
            //{
            //    NotifyStatusTile.CreateToastNotification(thread);
            //}
        }
    }
}
