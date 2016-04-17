using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using PlayStation_Gui.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Microsoft.ApplicationInsights;
using PlayStation_App.Database;
using PlayStation_Gui.Tools.Background;
using PlayStation_Gui.Tools.Database;
using PlayStation_Gui.Tools.Debug;
using PlayStation_Gui.Views;
using SQLite.Net.Platform.WinRT;

namespace PlayStation_Gui
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    sealed partial class App : Template10.Common.BootStrapper
    {
        public static ISettingsService Settings;

        public static Frame Frame;

        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync();
            InitializeComponent();

            #region App settings

            Settings = SettingsService.Instance;
            //RequestedTheme = _settings.AppTheme;

            #endregion

            #region Database
            var db = new UserAccountDataSource(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));
            db.CreateDatabase();

            var dbs = new StickersDataSource(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.StampDatabase));
            dbs.CreateDatabase();
            #endregion  
        }

        // runs even if restored from state
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // Setup Background
            var isIoT = ApiInformation.IsTypePresent("Windows.Devices.Gpio.GpioController");

            if (!isIoT)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
                //BackgroundTaskUtils.UnregisterBackgroundTasks(BackgroundTaskUtils.ToastBackgroundTaskName);
                //var task2 = await
                //    BackgroundTaskUtils.RegisterBackgroundTask(BackgroundTaskUtils.ToastBackgroundTaskEntryPoint,
                //        BackgroundTaskUtils.ToastBackgroundTaskName, new ToastNotificationActionTrigger(),
                //        null);

                if (Settings.BackgroundEnable)
                {
                    BackgroundTaskUtils.UnregisterBackgroundTasks(BackgroundTaskUtils.BackgroundTaskName);
                    var task = await
                        BackgroundTaskUtils.RegisterBackgroundTask(BackgroundTaskUtils.BackgroundTaskEntryPoint,
                            BackgroundTaskUtils.BackgroundTaskName,
                            new TimeTrigger(15, false),
                            null);
                }
            }

            var launch = args as LaunchActivatedEventArgs;
            if (launch?.PreviousExecutionState == ApplicationExecutionState.NotRunning
                || launch?.PreviousExecutionState == ApplicationExecutionState.Terminated
                || launch?.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                // setup hamburger shell
                Frame = new Frame();
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include, Frame);
                var shell = new Shell(nav);
                if (Shell.Instance.ViewModel.CurrentUser == null)
                {
                    var userAccountDatabase = new UserAccountDatabase(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));
                    if (await userAccountDatabase.HasDefaultAccounts())
                    {
                        try
                        {
                            var result = await Shell.Instance.ViewModel.LoginDefaultUser();
                        }
                        catch (Exception)
                        {
                            // error happened, send them to account page so we can check on it.
                        }
                    }
                }
                Window.Current.Content = shell;
            }

            await Task.CompletedTask;
        }

        public override async void OnResuming(object s, object e, AppExecutionState previousExecutionState)
        {
            base.OnResuming(s, e, previousExecutionState);
            // On Restore, if we have a frame, remake navigation so we can go back to previous pages.
            if (Frame != null)
            {
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include, Frame);
                var shell = (Shell)Window.Current.Content;
                shell.SetNav(nav);
                if (Shell.Instance.ViewModel.CurrentUser == null)
                {
                    var userAccountDatabase = new UserAccountDatabase(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));
                    if (await userAccountDatabase.HasDefaultAccounts())
                    {
                        try
                        {
                            var result = await Shell.Instance.ViewModel.LoginDefaultUser();
                        }
                        catch (Exception)
                        {
                            // error happened, send them to account page so we can check on it.
                        }
                    }
                }
                else
                {
                    await Shell.Instance.ViewModel.UpdateTokens();
                }
                var page = Frame.Content as MessagesPage;
                if (page != null)
                {
                    Current.NavigationService.FrameFacade.BackRequested +=
                        page.ViewModel.MasterDetailViewControl.NavigationManager_BackRequested;
                }
                else
                {
                    var threadpage = Frame.Content as FriendsPage;
                    if (threadpage != null)
                    {
                        Current.NavigationService.FrameFacade.BackRequested += threadpage.ViewModel.MasterDetailViewControl.NavigationManager_BackRequested;
                    }
                }
            }

            await Task.CompletedTask;
        }

        // runs only when not restored from state
        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            var launch = args as LaunchActivatedEventArgs;
            if (launch?.PreviousExecutionState == ApplicationExecutionState.NotRunning
                || launch?.PreviousExecutionState == ApplicationExecutionState.Terminated
                || launch?.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                if (Shell.Instance.ViewModel.CurrentUser == null)
                {
                    var userAccountDatabase = new UserAccountDatabase(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));
                    if (await userAccountDatabase.HasAccounts())
                    {

                        if (await userAccountDatabase.HasDefaultAccounts())
                        {
                            try
                            {
                                var result = await Shell.Instance.ViewModel.LoginDefaultUser();
                                NavigationService.Navigate(result ? typeof(Views.MainPage) : typeof(Views.AccountPage));
                            }
                            catch (Exception)
                            {
                                // error happened, send them to account page so we can check on it.
                                NavigationService.Navigate(typeof(Views.AccountPage));
                            }

                        }
                        else
                        {
                            NavigationService.Navigate(typeof(Views.AccountPage));
                        }
                    }
                    else
                    {
                        NavigationService.Navigate(typeof(Views.LoginPage));
                    }
                }
                else
                {
                    NavigationService.Navigate(typeof(Views.MainPage));
                }
            }

            await Task.CompletedTask;
        }
    }
}

