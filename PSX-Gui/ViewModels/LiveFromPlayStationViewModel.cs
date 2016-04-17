using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AmazingPullToRefresh.Controls;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Models.Live;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_Gui.Views;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class LiveFromPlayStationViewModel : ViewModelBase
    {
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                Set(ref _isLoading, value);
            }
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var error = string.Empty;
            try
            {
                await BuildList();
                return;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            await ResultChecker.SendMessageDialogAsync(error, false);
        }

        public async void PullToRefresh_ListView(object sender, RefreshRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();
            await BuildList();
            deferral.Complete();
        }

        public async void LiveGrid_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (LiveBroadcastEntity)e.ClickedItem;
            await Launcher.LaunchUriAsync(new Uri(item.Url));
        }

        private string _searchString;

        private readonly LiveStreamManager _liveStreamManager = new LiveStreamManager();
        private ObservableCollection<LiveBroadcastEntity> _liveBroadcastCollection;
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                Set(ref _searchString, value);
            }
        }

        public ObservableCollection<LiveBroadcastEntity> LiveBroadcastCollection
        {
            get { return _liveBroadcastCollection; }
            set
            {
                Set(ref _liveBroadcastCollection, value);
            }
        }

        public async Task BuildList()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            IsLoading = true;
            await SetUstreamElements();
            await SetTwitchElements();
            await SetNicoDougaElements();
            IsLoading = false;
        }

        public async Task BuildListSearch()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            IsLoading = true;
            await SetUstreamElements(false, SearchString);
            await SetTwitchElements(false, SearchString);
            await SetNicoDougaElements(false, SearchString);
            IsLoading = false;
        }

        public async Task BuildListInteractive()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            IsLoading = true;
            await Shell.Instance.ViewModel.UpdateTokens();
            await SetUstreamElements(true);
            await SetTwitchElements(true);
            await SetNicoDougaElements(true);
            IsLoading = false;
        }

        public async Task BuildNicoList()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            await SetNicoDougaElements(false);
        }

        public async Task BuildTwitch()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            await SetTwitchElements(false);
        }


        public async Task BuildUstreamList()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            await SetUstreamElements(false);
        }

        private async Task SetUstreamElements(bool interactive = false, string query = "")
        {
            var filterList = new Dictionary<string, string>
            {
                {"platform", "PS4"},
                {"type", "live"},
                {"interactive", interactive ? "true" : "false"}
            };

            var ustreamResultList =
                await
                    _liveStreamManager.GetUstreamFeed(0, 80, "compact", filterList, "views", query,
                        Shell.Instance.ViewModel.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, ustreamResultList);
            var result = await ResultChecker.CheckSuccess(ustreamResultList);
            if (!result)
            {
                return;
            }
            var ustreamList = JsonConvert.DeserializeObject<UstreamEntity>(ustreamResultList.ResultJson);
            if (ustreamList?.items == null) return;
            foreach (UstreamEntity.Item ustream in ustreamList.items)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromUstream(ustream);
                LiveBroadcastCollection.Add(entity);
            }

        }

        private async Task SetTwitchElements(bool interactive = false, string query = "")
        {
            var twitchResult =
                await _liveStreamManager.GetTwitchFeed(0, 80, "PS4", interactive, query, Shell.Instance.ViewModel.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, twitchResult);
            var result = await ResultChecker.CheckSuccess(twitchResult);
            if (!result)
            {
                return;
            }
            var twitchList = JsonConvert.DeserializeObject<TwitchEntity>(twitchResult.ResultJson);
            if (twitchList?.streams == null) return;
            foreach (TwitchEntity.Stream twitch in twitchList.streams)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromTwitch(twitch);
                LiveBroadcastCollection.Add(entity);
            }
        }

        private async Task SetNicoDougaElements(bool interactive = false, string query = "")
        {
            var nicoNicoResultEntity =
                await _liveStreamManager.GetNicoFeed("onair", "PS4", interactive, 0, 80, "view", query, Shell.Instance.ViewModel.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, nicoNicoResultEntity);
            var result = await ResultChecker.CheckSuccess(nicoNicoResultEntity);
            if (!result)
            {
                return;
            }
            var nicoNicoEntity = JsonConvert.DeserializeObject<NicoNicoEntity>(nicoNicoResultEntity.ResultJson);
            if (nicoNicoEntity?.programs == null) return;
            foreach (NicoNicoEntity.Program program in nicoNicoEntity.programs)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromNicoNico(program);
                LiveBroadcastCollection.Add(entity);
            }

        }
    }
}
