using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.ViewModels;

namespace PlayStation_App.Commands.Messages
{
    public class LeaveMessageGroup : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
            var message = parameter as MessageGroupItem;
            if (message == null)
                return;
            await Locator.ViewModels.MessagesVm.RemoveMessageGroup(message);
        }
    }
}
