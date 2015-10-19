using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Commands.Live;
using PlayStation_App.Common;
using PlayStation_App.Models.Live;
using PlayStation_App.Models.Response;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;

namespace PlayStation_App.ViewModels
{
    public class LiveFromPlayStationViewModel : NotifierBase
    {
        private string _searchString;
        private readonly LiveStreamManager _liveStreamManager = new LiveStreamManager();
        private ObservableCollection<LiveBroadcastEntity> _liveBroadcastCollection;
        public SearchLiveFromPlaystation SearchLiveList { get; set; } = new SearchLiveFromPlaystation();
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                SetProperty(ref _searchString, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LiveBroadcastEntity> LiveBroadcastCollection
        {
            get { return _liveBroadcastCollection; }
            set
            {
                SetProperty(ref _liveBroadcastCollection, value);
                OnPropertyChanged();
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
                        Locator.ViewModels.MainPageVm.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, ustreamResultList);
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
                    await _liveStreamManager.GetTwitchFeed(0, 80, "PS4", interactive, query, Locator.ViewModels.MainPageVm.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, twitchResult);
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
                await _liveStreamManager.GetNicoFeed("onair", "PS4", interactive, 0, 80, "view", query, Locator.ViewModels.MainPageVm.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, nicoNicoResultEntity);
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
