using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Core.Entities;
using PlayStation_App.Models;

namespace PlayStation_App.ViewModels
{
    public class MainPageViewModel : NotifierBase
    {
        private UserAccountEntity _currentUser;

        public UserAccountEntity CurrentUser
        {
            get { return _currentUser; }
            set
            {
                SetProperty(ref _currentUser, value);
                OnPropertyChanged();
            }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set
            {
                SetProperty(ref _isLoggedIn, value);
                OnPropertyChanged();
            }
        }

        public List<MenuItem> MenuItems { get; set; }
    }
}
