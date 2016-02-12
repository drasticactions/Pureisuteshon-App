using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Microsoft.ApplicationInsights;
using PlayStation.Entities.Web;

namespace PlayStation_App.Tools.Debug
{
    public static class ResultChecker
    {
        public async static Task SendMessageDialogAsync(string message, bool isSuccess)
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var title = isSuccess ? loader.GetString("SuccessText/Text") : loader.GetString("ErrorText/Text");
            var dialog = new MessageDialog((string.Concat(title, Environment.NewLine, Environment.NewLine, message)));
            await dialog.ShowAsync();
            if (!isSuccess)
            {
                try
                {
                    var tc = new TelemetryClient();
                    var prop = new Dictionary<string, string> { { "errorMessage", message } };
                    tc.TrackEvent("Error", prop);
                }
                catch (Exception)
                {
                    // Ignore analytics failing. :\
                }
            }
        }

        public static void LogEvent(string eventName, Dictionary<string, string> events)
        {
            try
            {
                var tc = new TelemetryClient();
                tc.TrackEvent(eventName, events);
            }
            catch (Exception)
            {
                // Ignore errors? From logs? :\
            }
        }



        public async static Task<bool> CheckSuccess(Result result, bool showMessage = true)
        {
            if (result.IsSuccess)
                return true;
            if(showMessage)
            await SendMessageDialogAsync(result.Error + result.ResultJson, false);
            return false;
        }
    }
}
