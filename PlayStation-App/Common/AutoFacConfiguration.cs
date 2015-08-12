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
            builder.RegisterType<MainPageViewModel>().SingleInstance();
            //builder.RegisterType<MainPage>();
            return builder.Build();
        }
    }
}
