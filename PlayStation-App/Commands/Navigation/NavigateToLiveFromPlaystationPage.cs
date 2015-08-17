using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Pages;

namespace PlayStation_App.Commands.Navigation
{
    public class NavigateToLiveFromPlaystationPage : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            App.RootFrame.Navigate(typeof(LiveFromPlaystationPage));
        }
    }
}
