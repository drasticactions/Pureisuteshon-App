using Windows.UI.Xaml.Controls;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace PlayStation_App.Views
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

        private async void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox == null) return;
            switch (FilterComboBox.SelectedIndex)
            {
                case 0:
                    await Locator.ViewModels.LiveFromPlayStationVm.BuildList();
                    break;
                case 1:
                    await Locator.ViewModels.LiveFromPlayStationVm.BuildListInteractive();
                    break;
                case 2:
                    await Locator.ViewModels.LiveFromPlayStationVm.BuildNicoList();
                    break;
                case 3:
                    await Locator.ViewModels.LiveFromPlayStationVm.BuildTwitch();
                    break;
                case 4:
                    await Locator.ViewModels.LiveFromPlayStationVm.BuildUstreamList();
                    break;
            }
        }
    }
}
