using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using PlayStation_App.Models.Trophies;
using PlayStation_App.Models.TrophyDetail;
using PlayStation_App.Tools.ScrollingCollection;
using PlayStation_Gui.Models;
using PlayStation_Gui.Views;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class TrophyListViewModel : ViewModelBase
    {
        private TrophyScrollingCollection _trophyScrollingCollection;

        public TrophyScrollingCollection TrophyScrollingCollection
        {
            get { return _trophyScrollingCollection; }
            set
            {
                Set(ref _trophyScrollingCollection, value);
            }
        }

        public string Username { get; set; }

        public void SetTrophyList()
        {
            TrophyScrollingCollection = new TrophyScrollingCollection
            {
                CompareUsername = Shell.Instance.ViewModel.CurrentUser.Username,
                Username = Username,
                Offset = 0
            };
        }

        public void SelectTrophy(object sender, ItemClickEventArgs e)
        {
            var trophy = e.ClickedItem as TrophyTitle;
            var trophyNav = new TrophyNavProperties
            {
                NpCommunicationId = trophy.NpCommunicationId,
                Username = Username,
                TrophyName = trophy.TrophyTitleName
            };
            NavigationService.Navigate(typeof (TrophyDetailListPage), JsonConvert.SerializeObject(trophyNav));
        }

        public override void OnNavigatedTo(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            if (TrophyScrollingCollection == null || !TrophyScrollingCollection.Any())
            {
                if (!string.IsNullOrEmpty(parameter as string))
                {
                    Username = parameter as string;
                }
                else
                {
                    Username = Shell.Instance.ViewModel.CurrentUser.Username;
                }
                SetTrophyList();
            }
        }
    }
}
