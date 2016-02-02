using PlayStation_App.Common;

namespace PlayStation_App.Commands.Live
{
    public class SearchLiveFromPlaystation : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            Locator.ViewModels.LiveFromPlayStationVm.BuildListSearch();
        }
    }
}
