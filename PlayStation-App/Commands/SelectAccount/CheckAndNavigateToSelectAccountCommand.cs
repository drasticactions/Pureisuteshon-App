using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Views.Account;

namespace PlayStation_App.Commands.SelectAccount
{
    public class CheckAndNavigateToSelectAccountCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            var areUsers = await Locator.ViewModels.AccountSelectVm.Initialize();
            if (areUsers)
            {
                App.RootFrame.Navigate(typeof (SelectAccountPage));
            }
            else
            {
                App.RootFrame.Navigate(typeof (LoginPage));
            }
        }
    }
}
