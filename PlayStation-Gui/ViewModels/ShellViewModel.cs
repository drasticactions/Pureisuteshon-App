using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Mvvm;

namespace PlayStation_Gui.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private bool _isLoggedIn = default(bool);

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                Set(ref _isLoggedIn, value);
            }
        }


    }
}
