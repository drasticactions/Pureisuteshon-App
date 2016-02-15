using System;
using Windows.UI.Xaml;

namespace PlayStation_Gui.Services.SettingsServices
{
    // DOCS: https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-SettingsService
    public partial class SettingsService : ISettingsService
    {
        public static SettingsService Instance { get; }
        static SettingsService()
        {
            // implement singleton pattern
            Instance = Instance ?? new SettingsService();
        }

        Template10.Services.SettingsService.ISettingsHelper _helper;
        private SettingsService()
        {
            _helper = new Template10.Services.SettingsService.SettingsHelper();
        }

        public bool BackgroundEnable
        {
            get { return _helper.Read<bool>(nameof(BackgroundEnable), false); }
            set
            {
                _helper.Write(nameof(BackgroundEnable), value);
                ChangeBackgroundStatus(value);
            }
        }

        public bool RecentActivityBackground
        {
            get { return _helper.Read<bool>(nameof(RecentActivityBackground), false); }
            set
            {
                _helper.Write(nameof(RecentActivityBackground), value);
            }
        }

        public ApplicationTheme AppTheme
        {
            get
            {
                var theme = ApplicationTheme.Dark;
                var value = _helper.Read<string>(nameof(AppTheme), theme.ToString());
                return Enum.TryParse<ApplicationTheme>(value, out theme) ? theme : ApplicationTheme.Dark;
            }
            set
            {
                _helper.Write(nameof(AppTheme), value.ToString());
                ApplyAppTheme(value);
            }
        }
    }
}

