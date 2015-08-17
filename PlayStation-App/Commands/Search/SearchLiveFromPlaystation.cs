using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;

namespace PlayStation_App.Commands.Search
{
    public class SearchLiveFromPlaystation : AlwaysExecutableCommand
    {
        public override void Execute(object parameter)
        {
            Locator.ViewModels.LiveFromPlayStationVm.BuildListSearch();
        }
    }
}
