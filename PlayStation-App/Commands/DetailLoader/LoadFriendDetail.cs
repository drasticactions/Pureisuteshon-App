using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;

namespace PlayStation_App.Commands.DetailLoader
{
    public class LoadFriendDetail : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            Locator.ViewModels.FriendsPageVm.IsLoading = true;
            await Locator.ViewModels.FriendPageVm.SetUser((string) parameter);
            Locator.ViewModels.FriendsPageVm.IsLoading = false;
        }
    }
}
