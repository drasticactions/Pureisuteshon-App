using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Core.Entities;
using PlayStation_App.Core.Managers;

namespace PlayStation_App.ViewModels
{
    public class LiveFromPlayStationViewModel : NotifierBase
    {
        private readonly LiveStreamManager _liveStreamManager = new LiveStreamManager();
        private ObservableCollection<LiveBroadcastEntity> _liveBroadcastCollection;

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
            _liveBroadcastCollection = new ObservableCollection<LiveBroadcastEntity>();
            SetUstreamElements();
            SetTwitchElements();
            SetNicoDougaElements();
        }

        private async void SetUstreamElements()
        {
            IsLoading = true;
            var filterList = new Dictionary<string, string>
            {
                {"platform", "PS4"},
                {"type", "live"},
                {"interactive", "true"}
            };
            UstreamEntity ustreamList =
                await
                    _liveStreamManager.GetUstreamFeed(0, 80, "compact", filterList, "views", string.Empty,
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

        private async void SetTwitchElements()
        {
            IsLoading = true;
            TwitchEntity twitchList =
                await _liveStreamManager.GetTwitchFeed(0, 80, "PS4", "true", string.Empty, Locator.ViewModels.MainPageVm.CurrentUser);
            if (twitchList?.streams == null) return;
            foreach (TwitchEntity.Stream twitch in twitchList.streams)
            {
                var entity = new LiveBroadcastEntity();
                entity.ParseFromTwitch(twitch);
                LiveBroadcastCollection.Add(entity);
            }
            IsLoading = false;
        }

        private async void SetNicoDougaElements()
        {
            IsLoading = true;
            NicoNicoEntity nicoNicoEntity =
                await _liveStreamManager.GetNicoFeed("onair", "PS4", 0, 80, "view", Locator.ViewModels.MainPageVm.CurrentUser);
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
