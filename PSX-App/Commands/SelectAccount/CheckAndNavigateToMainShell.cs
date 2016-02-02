using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using PlayStation_App.Common;
using PlayStation_App.Models.Authentication;

namespace PlayStation_App.Commands.SelectAccount
{
    public class CheckAndNavigateToMainShellCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            var args = (ItemClickEventArgs)parameter;
            var user = args.ClickedItem as AccountUser;
            if (user == null)
                return;
            await Locator.ViewModels.AccountSelectVm.CheckAndNavigateToMainShell(user);
        }
    }
}
