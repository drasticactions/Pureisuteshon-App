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

        public static MainPageViewModel MainPageVm => App.Container.Resolve<MainPageViewModel>();

        public static HomeViewModel HomeVm => App.Container.Resolve<HomeViewModel>();
    }
}
