using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using PlayStation_App.Common;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.TrophyDetail;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Trophies
{
    public class NavigateToTrophyDetailListCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            var args = (ItemClickEventArgs)parameter;
            var trophy = args.ClickedItem as TrophyTitle;
            if (trophy == null)
                return;
            Locator.ViewModels.TrophiesVm.SelectedTrophyTitleName = trophy.TrophyTitleName;
            App.RootFrame.Navigate(typeof (TrophyDetailListPage));
            await Locator.ViewModels.TrophiesVm.SetTrophyDetailList(trophy.NpCommunicationId);
        }
    }
}
