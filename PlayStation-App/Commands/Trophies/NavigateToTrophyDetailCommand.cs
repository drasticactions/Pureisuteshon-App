using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using PlayStation_App.Common;
using PlayStation_App.Models.Trophies;
using PlayStation_App.Models.TrophyDetail;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Trophies
{
    public class NavigateToTrophyDetailCommand : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            var args = (ItemClickEventArgs)parameter;
            var trophy = args.ClickedItem as Trophy;
            if (trophy == null)
                return;
            Locator.ViewModels.TrophiesVm.SelectedTrophy = trophy;
            Locator.ViewModels.TrophiesVm.IsTrophySelected = true;
        }
    }
}
