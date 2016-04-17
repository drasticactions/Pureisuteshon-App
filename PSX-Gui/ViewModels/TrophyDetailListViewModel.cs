using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using AmazingPullToRefresh.Controls;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Models.Friends;
using PlayStation_App.Models.Response;
using PlayStation_App.Models.Trophies;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;
using PlayStation_App.Tools.ScrollingCollection;
using PlayStation_Gui.Controls;
using PlayStation_Gui.Models;
using PlayStation_Gui.Views;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class TrophyDetailListViewModel : ViewModelBase
    {
        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (state.ContainsKey(nameof(Username)) && state.ContainsKey(nameof(NpcommunicationId)) && state.ContainsKey(nameof(TrophyName)))
            {
                Username = state[nameof(Username)]?.ToString();
                NpcommunicationId = state[nameof(NpcommunicationId)]?.ToString();
                TrophyName = state[nameof(TrophyName)]?.ToString();
            }

            string error;
            try
            {
                if (TrophyDetailList == null || !TrophyDetailList.Any())
                {
                    if (string.IsNullOrEmpty(NpcommunicationId) || string.IsNullOrEmpty(Username))
                    {
                        var trophyNavString = parameter as string;
                        if (string.IsNullOrEmpty(trophyNavString))
                        {
                            return;
                        }
                        var trophyNav = JsonConvert.DeserializeObject<TrophyNavProperties>(trophyNavString);
                        NpcommunicationId = trophyNav.NpCommunicationId;
                        Username = trophyNav.Username;
                        TrophyName = trophyNav.TrophyName;
                    }
                    await SetTrophyDetailList();
                }
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
            try
            {
                await SetTrophyDetailList();
            }
            catch (Exception)
            {
                // TODO: Add error checker here too?
            }
            deferral.Complete();
        }


        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            try
            {
                state[nameof(Username)] = Username;
                state[nameof(NpcommunicationId)] = NpcommunicationId;
                state[nameof(TrophyName)] = TrophyName;
            }
            catch (Exception ex)
            {
                ResultChecker.LogEvent("SerializeError", new Dictionary<string, string>() { { "serialization", ex.Message } });
            }
            return Task.CompletedTask;
        }

        public void SelectTrophyDetail(object sender, ItemClickEventArgs e)
        {
            var trophyDetail = e.ClickedItem as Trophy;
            if (trophyDetail == null) return;
            TrophyViewModel.SelectedTrophy = trophyDetail;
            TrophyViewModel.IsOpen = true;
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private readonly TrophyManager _trophyManager = new TrophyManager();
        private ObservableCollection<Trophy> _trophyDetailList;

        public TrophyViewModel TrophyViewModel { get; set; }

        public ObservableCollection<Trophy> TrophyDetailList
        {
            get { return _trophyDetailList; }
            set
            {
                Set(ref _trophyDetailList, value);
            }
        }
        private bool _isTrophyDetailListEmpty;

        public bool IsTrophyDetailListEmpty
        {
            get { return _isTrophyDetailListEmpty; }
            set
            {
                Set(ref _isTrophyDetailListEmpty, value);
            }
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

        public string Username { get; set; }

        public string NpcommunicationId { get; set; }

        public string TrophyName { get; set; }


        public async Task SetTrophyDetailList()
        {
            IsLoading = true;
            await Shell.Instance.ViewModel.UpdateTokens();
            TrophyDetailList = new ObservableCollection<Trophy>();
            var trophyResult =
                await
                    _trophyManager.GetTrophyDetailList(NpcommunicationId,
                        Username, true,
                        Shell.Instance.ViewModel.CurrentTokens, Username, Shell.Instance.ViewModel.CurrentUser.Region, Shell.Instance.ViewModel.CurrentUser.Language);
            await AccountAuthHelpers.UpdateTokens(Shell.Instance.ViewModel.CurrentUser, trophyResult);
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
    }
}
