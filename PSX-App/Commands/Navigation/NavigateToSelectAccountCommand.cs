using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Models;
using PlayStation_App.Models.Authentication;
using PlayStation_App.Views.Account;

namespace PlayStation_App.Commands.Navigation
{
    public class NavigateToSelectAccountCommand : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            App.RootFrame.Navigate(typeof (SelectAccountPage));
        }
    }

    public class DeleteAccountCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            var user = (AccountUser) parameter;
            await Locator.ViewModels.AccountSelectVm.DeleteUserAccount(user);
        }
    }
}
