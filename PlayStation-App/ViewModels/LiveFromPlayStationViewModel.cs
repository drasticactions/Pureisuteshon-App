using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Commands.Search;
using PlayStation_App.Common;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Managers;

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

        public void BuildList()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            SetUstreamElements();
            SetTwitchElements();
            SetNicoDougaElements();
        }

        public void BuildListSearch()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            SetUstreamElements(false, SearchString);
            SetTwitchElements(false, SearchString);
            SetNicoDougaElements(false, SearchString);
        }

        public void BuildListInteractive()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            SetUstreamElements(true);
            SetTwitchElements(true);
            SetNicoDougaElements(true);
        }

        public void BuildNicoList()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            SetNicoDougaElements(false);
        }

        public void BuildTwitch()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            SetTwitchElements(false);
        }


        public void BuildUstreamList()
        {
            LiveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            SetUstreamElements(false);
        }

        private async void SetUstreamElements(bool interactive = false, string query = "")
        {
            IsLoading = true;
            var filterList = new Dictionary<string, string>
            {
                {"platform", "PS4"},
                {"type", "live"},
                {"interactive", interactive ? "true" : "false"}
            };
            UstreamEntity ustreamList =
                await
                    _liveStreamManager.GetUstreamFeed(0, 80, "compact", filterList, "views", query,
                        Locator.ViewModels.MainPageVm.CurrentUser);
            if (ustreamList?.items == null) return;
            foreach (UstreamEntity.Item ustream in ustreamList.items)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromUstream(ustream);
                LiveBroadcastCollection.Add(entity);
            }
            IsLoading = false;
        }

        private async void SetTwitchElements(bool interactive = false, string query = "")
        {
            IsLoading = true;
            TwitchEntity twitchList =
                await _liveStreamManager.GetTwitchFeed(0, 80, "PS4", interactive, query, Locator.ViewModels.MainPageVm.CurrentUser);
            if (twitchList?.streams == null) return;
            foreach (TwitchEntity.Stream twitch in twitchList.streams)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromTwitch(twitch);
                LiveBroadcastCollection.Add(entity);
            }
            IsLoading = false;
        }

        private async void SetNicoDougaElements(bool interactive = false, string query = "")
        {
            IsLoading = true;
            NicoNicoEntity nicoNicoEntity =
                await _liveStreamManager.GetNicoFeed("onair", "PS4", interactive, 0, 80, "view", query, Locator.ViewModels.MainPageVm.CurrentUser);
            if (nicoNicoEntity?.programs == null) return;
            foreach (NicoNicoEntity.Program program in nicoNicoEntity.programs)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromNicoNico(program);
                LiveBroadcastCollection.Add(entity);
            }

            IsLoading = false;
        }
    }
}
