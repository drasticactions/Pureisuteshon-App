using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using PlayStation_Gui.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using PlayStation_App.Database;
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
            InitializeComponent();

            #region App settings

            Settings = SettingsService.Instance;
            //RequestedTheme = _settings.AppTheme;

            #endregion

            #region Database
            var db = new UserAccountDataSource(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));
            db.CreateDatabase();
            #endregion  
        }

        // runs even if restored from state
        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            var launch = args as LaunchActivatedEventArgs;
            if (launch?.PreviousExecutionState == ApplicationExecutionState.NotRunning
                || launch?.PreviousExecutionState == ApplicationExecutionState.Terminated
                || launch?.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                // setup hamburger shell
                Frame = new Frame();
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include, Frame);
                var shell = new Shell(nav);
                //await shell.ViewModel.LoginUser();
                Window.Current.Content = shell;
            }

            await Task.CompletedTask;
        }

        public override async void OnResuming(object s, object e)
        {
            base.OnResuming(s, e);
            // On Restore, if we have a frame, remake navigation so we can go back to previous pages.
            if (Frame != null)
            {
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include, Frame);
                var shell = (Shell)Window.Current.Content;
               // await shell.ViewModel.LoginUser();
                shell.SetNav(nav);
                //var page = Frame.Content as BookmarksPage;
                //if (page != null)
                //{
                //    Current.NavigationService.FrameFacade.BackRequested +=
                //        page.ViewModel.MasterDetailViewControl.NavigationManager_BackRequested;
                //}
                //else
                //{
                //    var threadpage = Frame.Content as ThreadListPage;
                //    if (threadpage != null)
                //    {
                //        Current.NavigationService.FrameFacade.BackRequested += page.ViewModel.MasterDetailViewControl.NavigationManager_BackRequested;
                //    }
                //}
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
                var userAccountDatabase = new UserAccountDatabase(new SQLitePlatformWinRT(), DatabaseWinRTHelpers.GetWinRTDatabasePath(StringConstants.UserDatabase));
                if (await userAccountDatabase.HasAccounts())
                {

                    if (await userAccountDatabase.HasDefaultAccounts())
                    {
                        try
                        {
                            var result = await Shell.Instance.ViewModel.LoginDefaultUser();
                            NavigationService.Navigate(result ? typeof (Views.MainPage) : typeof (Views.AccountPage));
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

            await Task.CompletedTask;
        }
    }
}

