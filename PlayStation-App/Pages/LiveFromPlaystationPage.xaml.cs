using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace PlayStation_App.Pages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class LiveFromPlaystationPage : Page
    {
        public LiveFromPlaystationPage()
        {
            this.InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox == null) return;
            switch (FilterComboBox.SelectedIndex)
            {
                case 0:
                    Locator.ViewModels.LiveFromPlayStationVm.BuildList();
                    break;
                case 1:
                    Locator.ViewModels.LiveFromPlayStationVm.BuildListInteractive();
                    break;
                case 2:
                    Locator.ViewModels.LiveFromPlayStationVm.BuildNicoList();
                    break;
                case 3:
                    Locator.ViewModels.LiveFromPlayStationVm.BuildTwitch();
                    break;
                case 4:
                    Locator.ViewModels.LiveFromPlayStationVm.BuildUstreamList();
                    break;
            }
        }
    }
}
