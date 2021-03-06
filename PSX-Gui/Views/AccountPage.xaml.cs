﻿using System;
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
using PlayStation_App.Models.Authentication;
using PlayStation_Gui.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PlayStation_Gui.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        public AccountViewModel ViewModel => this.DataContext as AccountViewModel;

        public AccountPage()
        {
            this.InitializeComponent();
        }

        private async void DeleteAccount_OnClick(object sender, RoutedEventArgs e)
        {
            var menuFlyout = sender as MenuFlyoutItem;
            if (menuFlyout == null) return;
            await ViewModel.DeleteUserAccount(menuFlyout.CommandParameter as AccountUser);
        }
    }
}
