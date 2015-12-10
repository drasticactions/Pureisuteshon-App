using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using PlayStation_App.Common;
using PlayStation_App.Models.Sticker;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Messages
{
    public class SelectStickerCommand : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            var itemClick = parameter as ItemClickEventArgs;
            var sticker = itemClick?.ClickedItem as StickerSelection;
            if (sticker == null)
            {
                return;
            }
            Locator.ViewModels.StickerListVm.IsLoading = true;
            try
            {
                await Locator.ViewModels.MessagesVm.SendSticker(sticker);
                App.RootFrame.Navigate(typeof (MessageListPage));
            }
            catch (Exception ex)
            {
                // TODO: Throw message
            }
            Locator.ViewModels.StickerListVm.IsLoading = false;
        }
    }
}
