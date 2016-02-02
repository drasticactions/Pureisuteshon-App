using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Messages
{
    public class NavigateToStickersListView : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            App.RootFrame.Navigate(typeof (StickerSelectionPage));
            await Locator.ViewModels.StickerListVm.GetStickerPacks();
        }
    }
}
