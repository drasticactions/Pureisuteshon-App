using PlayStation_App.Common;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Live
{
    public class NavigateToLiveFromPlaystationPage : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            App.RootFrame.Navigate(typeof(LiveFromPlaystationPage));
        }
    }
}
