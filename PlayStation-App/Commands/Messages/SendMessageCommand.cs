using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;

namespace PlayStation_App.Commands.Messages
{
    public class SendMessageCommand : AlwaysExecutableCommand
    {
        public async override void Execute(object parameter)
        {
           await Locator.ViewModels.MessagesVm.SendMessage();
        }
    }
}
