using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Views;

namespace PlayStation_App.Commands.Messages
{
    public class NavigateToMessagesViewCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            App.RootFrame.Navigate(typeof (MessageListPage));
            await Locator.ViewModels.MessagesVm.GetMessageGroups(Locator.ViewModels.MainPageVm.CurrentUser.Username);
        }
    }
}
