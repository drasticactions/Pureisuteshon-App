using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PlayStation_App.Models.Friends;
using PlayStation_Gui.ViewModels;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PlayStation_Gui.Controls
{
    public sealed partial class FriendView : UserControl
    {
        public FriendViewModel ViewModel => this.DataContext as FriendViewModel;

        public FriendView()
        {
            this.InitializeComponent();
        }

        public async Task LoadFriend(string onlineId)
        {
            await ViewModel.SetUser(onlineId);
            ViewModel.SetTrophyList(onlineId);
            ViewModel.SetFriendsList(onlineId, false, false, false, false, true, false, false);
            ViewModel.SetRecentActivityFeed(onlineId);
        }
    }
}
