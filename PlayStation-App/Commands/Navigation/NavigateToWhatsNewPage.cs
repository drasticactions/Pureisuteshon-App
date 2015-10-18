using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Models.RecentActivity;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Navigation
{
    public class NavigateToWhatsNewPage : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            Locator.ViewModels.WhatsNewVm.RecentActivityScrollingCollection = new ObservableCollection<Feed>();
            App.RootFrame.Navigate(typeof (WhatsNewPage));
            await Locator.ViewModels.WhatsNewVm.Initialize();
        }
    }
}
