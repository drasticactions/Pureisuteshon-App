using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Friends
{
    public class LoadFriendsPageWithDetail : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            App.RootFrame.Navigate(typeof(FriendsPage));
            App.RootFrame.Navigate(typeof (FriendPage));
            new LoadFriendDetail().Execute(parameter);
        }
    }
}
