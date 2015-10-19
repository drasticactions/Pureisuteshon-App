using PlayStation_App.Common;

namespace PlayStation_App.Commands.Friends
{
    public class LoadFriendDetail : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            Locator.ViewModels.FriendsPageVm.IsLoading = true;
            Locator.ViewModels.FriendPageVm.SetFriendsList((string)parameter, false, false, false, false, true,
        false, false);
            Locator.ViewModels.FriendPageVm.SetRecentActivityFeed((string)parameter);
            Locator.ViewModels.FriendPageVm.SetTrophyList((string)parameter);
            //Locator.ViewModels.FriendPageVm.SetMessages((string)parameter);
            await Locator.ViewModels.FriendPageVm.SetUser((string) parameter);
            Locator.ViewModels.FriendsPageVm.IsLoading = false;
        }
    }
}
