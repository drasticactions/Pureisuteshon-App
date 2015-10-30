using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Commands.Trophies;
using PlayStation_App.Common;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.Trophies;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_App.Tools.ScrollingCollection;

namespace PlayStation_App.ViewModels
{
    public class TrophiesViewModel : NotifierBase
    {
        public TrophiesViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                TrophyScrollingCollection = new TrophyScrollingCollection()
                {
                    HasMoreItems = false
                };

            }
        }

        public string Username { get; set; }

        public string NpcommunicationId { get; set; }

        public async void SetupSampleData()
        {
            var items = await SampleData.GetSampleTrophyFeed();
            foreach (var item in items)
            {
                TrophyScrollingCollection.Add(item);
            }
        }

        public void SetTrophyList(string userName)
        {
            Username = userName;
            TrophyScrollingCollection = new TrophyScrollingCollection
            {
                CompareUsername = Locator.ViewModels.MainPageVm.CurrentUser.Username,
                Username = userName,
                Offset = 0
            };
        }

        public async Task SetTrophyDetailList(string npCommunicationId)
        {
            NpcommunicationId = npCommunicationId;
            IsLoading = true;
            TrophyDetailList = new ObservableCollection<Trophy>();
            var trophyResult =
                await
                    _trophyManager.GetTrophyDetailList(npCommunicationId,
                        TrophyScrollingCollection.CompareUsername, true,
                        Locator.ViewModels.MainPageVm.CurrentTokens, TrophyScrollingCollection.Username, Locator.ViewModels.MainPageVm.CurrentUser.Region, Locator.ViewModels.MainPageVm.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, trophyResult);
            var trophies = JsonConvert.DeserializeObject<TrophyResponse>(trophyResult.ResultJson);
            if (trophies == null)
            {
                IsLoading = false;
                return;
            }
            if (trophies.Trophies == null)
            {
                IsLoading = false;
                return;
            }
            foreach (var trophy in trophies.Trophies)
            {
                TrophyDetailList.Add(trophy);
            }
            if (!trophies.Trophies.Any())
            {
                IsTrophyDetailListEmpty = true;
            }
            IsLoading = false;
        }

        private string _selectedTrophyTitleName;
        private bool _isTrophySelected;
        private bool _isTrophyDetailListEmpty;
        private readonly TrophyManager _trophyManager = new TrophyManager();
        private Trophy _selectedTrophy;
        private ObservableCollection<Trophy> _trophyDetailList; 
        private TrophyScrollingCollection _trophyScrollingCollection;

        public Trophy SelectedTrophy
        {
            get { return _selectedTrophy; }
            set
            {
                SetProperty(ref _selectedTrophy, value);
                OnPropertyChanged();
            }
        }

        public string SelectedTrophyTitleName
        {
            get { return _selectedTrophyTitleName; }
            set
            {
                SetProperty(ref _selectedTrophyTitleName, value);
                OnPropertyChanged();
            }
        }

        public bool IsTrophySelected
        {
            get { return _isTrophySelected; }
            set
            {
                SetProperty(ref _isTrophySelected, value);
                OnPropertyChanged();
            }
        }

        public bool IsTrophyDetailListEmpty
        {
            get { return _isTrophyDetailListEmpty; }
            set
            {
                SetProperty(ref _isTrophyDetailListEmpty, value);
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Trophy> TrophyDetailList
        {
            get { return _trophyDetailList; }
            set
            {
                SetProperty(ref _trophyDetailList, value);
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
        public NavigateToTrophyDetailCommand NavigateToTrophyDetailCommand { get; set; } = new NavigateToTrophyDetailCommand();
        public NavigateToTrophyDetailListCommand NavigateToTrophyDetailList { get; set; } = new NavigateToTrophyDetailListCommand();
    }
}
