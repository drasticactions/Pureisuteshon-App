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
using PlayStation_App.Models;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace PlayStation_App
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            App.RootFrame = MainFrame;
        }

        private async void MenuClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as MenuItem;
            menuItem?.Command.Execute(null);
            if (Splitter.IsPaneOpen)
            {
                Splitter.IsPaneOpen = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Splitter.DisplayMode = (Splitter.DisplayMode == SplitViewDisplayMode.Inline) ? SplitViewDisplayMode.CompactInline : SplitViewDisplayMode.Inline;
            Splitter.IsPaneOpen = (Splitter.IsPaneOpen != true);
        }
    }
}
