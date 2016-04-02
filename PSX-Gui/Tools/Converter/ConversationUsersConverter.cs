using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using PlayStation_App.Models.MessageGroups;
using PlayStation_Gui.Views;

namespace PlayStation_App.Tools.Converter
{
    public class ConversationUsersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var message = (MessageGroup) value;
            MessageGroupDetail messageGroupDetail = message.MessageGroupDetail;
            if (messageGroupDetail == null)
            {
                return $"Error getting title :(";
            }
            if (!string.IsNullOrEmpty(messageGroupDetail.MessageGroupName))
            {
                return messageGroupDetail.MessageGroupName;
            }
            var currentUsername = Shell.Instance.ViewModel.CurrentUser.Username;
            List<string> stringEnumerable =
                messageGroupDetail.Members.Where(member => !member.OnlineId.Equals(currentUsername))
                    .Select(member => member.OnlineId)
                    .ToList();
            return string.Join<string>(",", stringEnumerable);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}