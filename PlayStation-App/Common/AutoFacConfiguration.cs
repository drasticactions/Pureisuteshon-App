using Autofac;
using PlayStation_App.ViewModels;

namespace PlayStation_App.Common
{
    public class AutoFacConfiguration
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            // Register View Models
            builder.RegisterType<AccountSelectScreenViewModel>().SingleInstance();
            builder.RegisterType<MainPageViewModel>().SingleInstance();
            builder.RegisterType<WhatsNewViewModel>().SingleInstance();
            builder.RegisterType<FriendsPageViewModel>().SingleInstance();
            builder.RegisterType<FriendPageViewModel>().SingleInstance();
            builder.RegisterType<TrophiesViewModel>().SingleInstance();
            builder.RegisterType<LiveFromPlayStationViewModel>().SingleInstance();
            builder.RegisterType<MessagesViewModel>().SingleInstance();
            builder.RegisterType<EventsViewModel>().SingleInstance();
            //builder.RegisterType<MainPage>();
            return builder.Build();
        }
    }
}
