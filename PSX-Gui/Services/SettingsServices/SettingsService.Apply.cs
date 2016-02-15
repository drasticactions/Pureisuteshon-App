using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Xaml;
using PlayStation_Gui.Tools.Background;

namespace PlayStation_Gui.Services.SettingsServices
{
    public partial class SettingsService
    {
        public void ApplyAppTheme(ApplicationTheme value)
        {
            Views.Shell.HamburgerMenu.RefreshStyles(value);
        }

        public async void ChangeBackgroundStatus(bool value)
        {
            if (value)
            {
                var task = await
                        BackgroundTaskUtils.RegisterBackgroundTask(BackgroundTaskUtils.BackgroundTaskEntryPoint,
                            BackgroundTaskUtils.BackgroundTaskName,
                            new TimeTrigger(15, false),
                            null);
            }
            else
            {
                BackgroundTaskUtils.UnregisterBackgroundTasks(BackgroundTaskUtils.BackgroundTaskName);
            }
        }
    }
}

