using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Commands.Navigation;
using PlayStation_App.Common;
using PlayStation_App.Core.Entities;
using PlayStation_App.Models;

namespace PlayStation_App.ViewModels
{
    public class MainPageViewModel : NotifierBase
    {
        public MainPageViewModel()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            MenuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Icon = "/Assets/Icons/Home.png",
                    Name = loader.GetString("Home/Text"),
                    Command = new NavigateToHomePage()
                },
                new MenuItem()
                {
                    Icon = "/Assets/Icons/WhatsNew.png",
                    Name = loader.GetString("WhatsNew/Text"),
                    //Command = new NavigateToMainForumsPage()
                },
                new MenuItem()
                {
                    Icon = "/Assets/Icons/Friends.png",
                    Name = loader.GetString("Friends/Text"),
                    //Command = new NavigateToMainForumsPage()
                },
                new MenuItem()
                {
                    Icon = "/Assets/Icons/Trophy.png",
                    Name = loader.GetString("Trophy/Text"),
                    //Command = new NavigateToMainForumsPage()
                },
                new MenuItem()
                {
                    Icon = "/Assets/Icons/Messenger.png",
                    Name = loader.GetString("Messenger/Text"),
                    //Command = new NavigateToMainForumsPage()
                },
                new MenuItem()
                {
                    Icon = "/Assets/Icons/Live.png",
                    Name = loader.GetString("LiveFromPlayStation/Text"),
                    //Command = new NavigateToMainForumsPage()
                },
                new MenuItem()
                {
                    Icon = "/Assets/Icons/Settings.png",
                    Name = loader.GetString("Settings/Text"),
                    //Command = new NavigateToMainForumsPage()
                }
            };
        }
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
