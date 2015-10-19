using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using PlayStation_App.Common;
using PlayStation_App.Models.MessageGroups;
using PlayStation_App.Models.Messages;
using PlayStation_App.Models.User;

namespace PlayStation_App.ViewModels
{
    public class MessageGroupItem : NotifierBase
    {
        private string _avatarUrl;
        private bool _imageAvailable;
        public string AvatarUrl
        {
            get { return _avatarUrl; }
            set
            {
                SetProperty(ref _avatarUrl, value);
                OnPropertyChanged();
            }
        }

        public bool ImageAvailable
        {
            get { return _imageAvailable; }
            set
            {
                SetProperty(ref _imageAvailable, value);
                OnPropertyChanged();
            }
        }

        public BitmapImage Image { get; set; }

        public Message Message { get; set; }

        public MessageGroup MessageGroup { get; set; }
    }

    public class UserViewModel : NotifierBase
    {
        public User User { get; set; }

        public string CurrentUserOnlineId { get; set; }

        public string Language { get; set; }

        public bool IsNotCurrentUser { get; set; }
    }
}
