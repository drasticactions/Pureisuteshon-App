using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace PlayStation_Gui.Services.SettingsServices
{
    public partial class SettingsService
    {
        public void ApplyAppTheme(ApplicationTheme value)
        {
            Views.Shell.HamburgerMenu.RefreshStyles(value);
        }
    }
}

