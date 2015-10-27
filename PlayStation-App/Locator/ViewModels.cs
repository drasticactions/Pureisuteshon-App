using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Autofac;
using PlayStation_App.Common;
using PlayStation_App.ViewModels;

namespace PlayStation_App.Locator
{
    public class ViewModels
    {
        public ViewModels()
        {
            if (DesignMode.DesignModeEnabled)
            {
                App.Container = AutoFacConfiguration.Configure();
            }
        }

        public static AccountSelectScreenViewModel AccountSelectVm => App.Container.Resolve<AccountSelectScreenViewModel>();

        public static MainPageViewModel MainPageVm => App.Container.Resolve<MainPageViewModel>();

        public static WhatsNewViewModel WhatsNewVm => App.Container.Resolve<WhatsNewViewModel>();

        public static FriendsPageViewModel FriendsPageVm => App.Container.Resolve<FriendsPageViewModel>();

        public static FriendPageViewModel FriendPageVm => App.Container.Resolve<FriendPageViewModel>();

        public static TrophiesViewModel TrophiesVm => App.Container.Resolve<TrophiesViewModel>();

        public static LiveFromPlayStationViewModel LiveFromPlayStationVm => App.Container.Resolve<LiveFromPlayStationViewModel>();

        public static MessagesViewModel MessagesVm => App.Container.Resolve<MessagesViewModel>();

        public static EventsViewModel EventsVm => App.Container.Resolve<EventsViewModel>();

    }
}
