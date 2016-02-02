using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using PlayStation_App.Common;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Models.RecentActivity;

namespace PlayStation_App.Commands.WhatsNew
{
    public class SelectRecentItemCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            var args = (ItemClickEventArgs)parameter;
            var feed = args.ClickedItem as Feed;
            if (feed == null)
                return;
            if (feed.IsPreviousButton)
                await Locator.ViewModels.WhatsNewVm.LoadPreviousPages();
            if (feed.IsNextButton)
                await Locator.ViewModels.WhatsNewVm.LoadNextPages();
        }
    }
}
