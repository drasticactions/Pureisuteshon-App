using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Newtonsoft.Json;
using PlayStation.Managers;
using PlayStation_App.Common;
using PlayStation_App.Models.Response;
using PlayStation_App.Tools.Debug;
using PlayStation_App.Tools.Helpers;

namespace PlayStation_App.ViewModels
{
    public class FriendLinkViewModel : NotifierBase
    {
        private readonly FriendManager _friendManager = new FriendManager();
        private readonly ResourceLoader _loader = new ResourceLoader();
        public async Task<string> CreateFriendLink()
        {
            var result = await _friendManager.GetFriendLink(Locator.ViewModels.MainPageVm.CurrentTokens);
            await AccountAuthHelpers.UpdateTokens(Locator.ViewModels.MainPageVm.CurrentUser, result);
            var resultCheck = await ResultChecker.CheckSuccess(result);
            if (!resultCheck)
            {
                return string.Empty;
            }
            var tokenEntity = JsonConvert.DeserializeObject<TokenResponse>(result.ResultJson);
            return tokenEntity.Token;
        }

        public async Task SendFriendLinkViaSms()
        {
            IsLoading = true;
            var result = await ChatMessageManager.GetTransportsAsync();
            if (!result.Any())
            {
                IsLoading = false;
                await ResultChecker.SendMessageDialogAsync(_loader.GetString("NoSMS/Text"), false);
                return;
            }
            var link = await CreateFriendLink();

            if (!string.IsNullOrEmpty(link))
            {
                var chat = new ChatMessage
                {
                    Subject = _loader.GetString("FriendRequestBody/Text"),
                    Body = string.Format("{0} {1}", _loader.GetString("FriendRequestBody/Text"), link)
                };
                await ChatMessageManager.ShowComposeSmsMessageAsync(chat);
            }
            IsLoading = false;
        }

        public async Task SendFriendLinkViaEmail()
        {
            IsLoading = true;
            var link = await CreateFriendLink();
            if (!string.IsNullOrEmpty(link))
            {
                var em = new EmailMessage
                {
                    Subject = _loader.GetString("FriendRequestBody/Text"),
                    Body = string.Format("{0} {1}", _loader.GetString("FriendRequestBody/Text"), link)
                };
                await EmailManager.ShowComposeNewEmailAsync(em);
            }
            IsLoading = false;
        }
    }
}
