using System;
using Windows.UI.Xaml;

namespace PlayStation_Gui.Services.SettingsServices
{
    public interface ISettingsService
    {
        ApplicationTheme AppTheme { get; set; }

        bool BackgroundEnable { get; set; }

        bool RecentActivityBackground { get; set; }
    }
}
