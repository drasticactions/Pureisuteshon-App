using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayStation_App.Common;
using PlayStation_App.Models.Message;
using PlayStation_App.Models.User;

namespace PlayStation_App.ViewModels
{
    public class MessageGroupItem : NotifierBase
    {
        private string _avatarUrl;

        public string AvatarUrl
        {
            get { return _avatarUrl; }
            set
            {
                SetProperty(ref _avatarUrl, value);
                OnPropertyChanged();
            }
        }

        public MessageEntity.Message Message { get; set; }

        public MessageGroupEntity.MessageGroup MessageGroup { get; set; }
    }

    public class UserViewModel : NotifierBase
    {
        public User User { get; set; }

        public string CurrentUserOnlineId { get; set; }

        public string Language { get; set; }

        public bool IsNotCurrentUser { get; set; }
    }
}
