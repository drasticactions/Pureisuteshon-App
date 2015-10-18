using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Common;
using PlayStation_App.Models.Message;
using PlayStation_App.Tools.Helpers;

namespace PlayStation_App.ViewModels
{
    public class MessagesViewModel : NotifierBase
    {
        private ObservableCollection<MessageGroupItem> _messageGroupCollection =
            new ObservableCollection<MessageGroupItem>();

        private bool _messageGroupEmpty;

        private MessageGroupEntity _messageGroupEntity;
        private bool _messageGroupLoading;

        public bool MessageGroupEmpty
        {
            get { return _messageGroupEmpty; }
            set
            {
                SetProperty(ref _messageGroupEmpty, value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MessageGroupItem> MessageGroupCollection
        {
            get { return _messageGroupCollection; }
            set
            {
                SetProperty(ref _messageGroupCollection, value);
                OnPropertyChanged();
            }
        }

        public async void SetMessages(string userName)
        {
            IsLoading = true;
            MessageGroupCollection = new ObservableCollection<MessageGroupItem>();
            var messageManager = new MessageManager();
            var messageResult = await messageManager.GetMessageGroup(userName, Locator.ViewModels.MainPageVm.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, messageResult);
            _messageGroupEntity = JsonConvert.DeserializeObject<MessageGroupEntity>(messageResult.ResultJson);
            foreach (
                var newMessage in
                    _messageGroupEntity.MessageGroups.Select(message => new MessageGroupItem { MessageGroup = message }))
            {
                MessageGroupCollection.Add(newMessage);
            }
            if (MessageGroupCollection.Count <= 0)
            {
                MessageGroupEmpty = true;
            }
            IsLoading = false;
        }

    }
}
