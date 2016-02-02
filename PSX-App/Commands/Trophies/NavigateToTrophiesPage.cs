using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Trophies
{
    public class NavigateToTrophiesPage : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            string username = parameter as string;
            if (string.IsNullOrEmpty(username))
            {
                username = Locator.ViewModels.MainPageVm.CurrentUser.Username;
            }
            Locator.ViewModels.TrophiesVm.SetTrophyList(username);
            App.RootFrame.Navigate(typeof (TrophyListPage));
        }
    }
}
