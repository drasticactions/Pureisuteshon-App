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
using Template10.Mvvm;

namespace PlayStation_App.ViewModels
{
    public class MessageGroupItem : ViewModelBase
    {
        private string _avatarUrl;
        private bool _imageAvailable;
        public string AvatarUrl
        {
            get { return _avatarUrl; }
            set
            {
                Set(ref _avatarUrl, value);
            }
        }

        public bool ImageAvailable
        {
            get { return _imageAvailable; }
            set
            {
                Set(ref _imageAvailable, value);
            }
        }

        private BitmapImage _image;

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                Set(ref _image, value);
            }
        }

        public Message Message { get; set; }

        public MessageGroup MessageGroup { get; set; }
    }

    public class UserViewModel : ViewModelBase
    {
        public User User { get; set; }

        public string CurrentUserOnlineId { get; set; }

        public string Language { get; set; }

        public bool IsNotCurrentUser { get; set; }
    }
}
