using EmailClient.Controllers.Navigation;
using EmailClient.EventArguments;
using EmailClient.Models.AppConfigModels;
using EmailClient.Models.Gmail;
using EmailClient.Pages;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Controllers
{
    /// <summary>
    /// The main controller for the app - handles navigation between app-level processes.
    /// </summary>
    internal class AppController : SingletonController<AppController>
    {
        #region Properties
        /// <summary>
        /// The root page for app navigation.
        /// </summary>
        public MainWindow RootPage { get; private set; }

        /// <summary>
        /// The current navigation state of the app.
        /// </summary>
        public AppNavigationState NavState { get; set; }

        /// <summary>
        /// Reference to currently displayed page.
        /// </summary>
        public Page CurrentPage { get; set; }

        /// <summary>
        /// State variable to store a unique identifier for a currently pending
        /// account authorization.
        /// </summary>
        private Guid accountIdPendingAuthorization = Guid.Empty;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of the app controller class, with default values for properties.
        /// </summary>
        public AppController()
        {
            // Register for the API initialization events.
            GmailAPI.Instance.Initialized += this.GoogleApi_Initialized;
            GmailAPI.Instance.Authorized += this.GoogleApi_Authorized;

            // Initialize the controller with a main page as the root and the start state for the app navigation.
            this.RootPage = null;
            this.NavState = new StartState(this);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles when the Gmail API is initialized and read to use.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GoogleApi_Initialized(object sender, ApiEventArgs e)
        {
            
        }

        /// <summary>
        /// Handles when the Gmail API has changed authorized status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GoogleApi_Authorized(object sender, ApiAuthorizedEventArgs e)
        {
            this.AddNewAccountDataAsync(e.AccountID);
        }

        /// <summary>
        /// Handles a navigation request between app pages.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigationRequested(object sender, NavigationRequestedEventArgs e)
        {
            // If this is a back request, go back and exit this method.
            if (e.IsBackRequest)
            {
                this.NavState.GoBack();
                return;
            }

            switch (e.ToPage)
            {
                case PageTypes.Home:
                    // Navigate to the home page.
                    this.NavState.GotoHome();
                    break;
                case PageTypes.Settings:
                    // Navigate to the settings page.
                    this.NavState.GotoSettings();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Handles when a navigation between pages is complete. Subscribes to the new page's
        /// events and unsubscribes from the old page's events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNavigated(object sender, NavigatedEventArgs e)
        {
            // Store a reference to the page as the new current page.
            this.CurrentPage = e.PageNavigatedTo;

            // Initialize the new page.
            string toPageName = "";
            if (e.PageNavigatedTo is HomePage homePage)
            {
                this.InitializeHomePageAsync(homePage);
                toPageName = "Home Page";
            }
            else if (e.PageNavigatedTo is SettingsPage settingsPage)
            {
                this.InitializeSettingsPageAsync(settingsPage);
                toPageName = "Settings Page";
            }

            // Unsubscribe from the old page's events. This is to clear any handles on the page
            //      so that garbage collection deletes it and prevents a memory leak.
            string fromPageName = "";
            if (e.PageNavigatedFrom is null)
            {
                // There is no from page (this is the app initialization).
                fromPageName = "App Launch";
            }
            else if (e.PageNavigatedFrom is HomePage homePageFrom)
            {
                fromPageName = "Home Page";
            }
            else if (e.PageNavigatedFrom is SettingsPage settingsPageFrom)
            {
                settingsPageFrom.ChangeAccountConnectionRequested -= this.SettingsPage_ChangeAccountConnectionRequested;
                settingsPageFrom.OauthCodeAcquired -= this.SettingsPage_OauthCodeAcquired;
                settingsPageFrom.ConnectionRequestCancelled -= this.SettingsPage_ConnectionRequestCancelled;
                fromPageName = "Settings Page";
            }

            System.Diagnostics.Debug.WriteLine("Navigated to " + toPageName + " from " + fromPageName);
        }
        #endregion

        #region Event Handlers - Settings Page
        /// <summary>
        /// Handles when the user requests to change a connection to a service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsPage_ChangeAccountConnectionRequested(object sender, ChangeAccountConnectionRequestedEventArgs e)
        {
            if (sender is SettingsPage settingsPage)
            {
                switch (e.Action)
                {
                    // A request was issued to connect to the service.
                    case ConnectionAction.Connect:
                        this.StartOAuthFlowAsync(e.AccoutId, settingsPage);
                        break;

                    // A request was issued to retry connecting to the service.
                    case ConnectionAction.RetryConnect:
                        this.StartOAuthFlowAsync(e.AccoutId, settingsPage);
                        break;

                    // A request was issued to disconnect the service.
                    case ConnectionAction.Disconnect:
                        this.DisconnectAccountAsync(e.AccoutId, settingsPage);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Handles when the user has entered an OAuth code to connect a service.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsPage_OauthCodeAcquired(object sender, OAuthFlowContinueEventArgs e)
        {
            // Validate the code.
            if (String.IsNullOrWhiteSpace(e.Code))
            {
                // The code is definitely invalid; no point in reaching out to the server.
                if (sender is SettingsPage settingsPage)
                {
                    string errorMessage = "No code was entered!";
                    settingsPage.ShowOAuthErrorUIAsync(errorMessage);
                }
            }
            else
            {
                // Complete the OAuth flow and clear the state data for the pending authorization.
                GmailAPI.Instance.GetOauthTokenAsync(this.accountIdPendingAuthorization.ToString(), e.Code);
                this.accountIdPendingAuthorization = Guid.Empty;
            }
        }

        /// <summary>
        /// Handles when the user cancells the pending connect account request from the settings page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsPage_ConnectionRequestCancelled(object sender, OAuthFlowContinueEventArgs e)
        {
            // Clear the associated state value.
            this.accountIdPendingAuthorization = Guid.Empty;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Intended to be called upon app launch. Initializes app navigation.
        /// </summary>
        /// <param name="rootPage">The root page for this initialization of the app.</param>
        public void StartApp(MainWindow rootPage)
        {
            // Subscribe to the root page's events.
            rootPage.NavigationRequested += this.OnNavigationRequested;
            rootPage.Navigated += this.OnNavigated;

            // Set the given page as the root page.
            this.RootPage = rootPage;

            // Navigate to the home page.
            this.NavState.GotoHome();
        }

        /// <summary>
        /// Saves the state of the application.
        /// </summary>
        public async Task SaveAppStateAsync()
        {
            // Save the connection state for the Google Calendar API Connection.
            await GmailAPI.Instance.SaveConnectionDataAsync();

            // Save the user's app config settings.
            await AppConfig.Instance.SaveAsync();
        }
        #endregion

        #region Helper Methods
        /*
         * This design pattern is necessary because the NavigationView.Navigate method
         * takes in a Type, and not a page instance. It creates the page instance and
         * places that instance into the event args for a "Navigated" event. Handling
         * this event is the way to get access to the instance of the new page, and
         * thus initialize its data.
         */

        /// <summary>
        /// Initializes the given instance of the home page.
        /// </summary>
        /// <param name="homePage">The instance of the home page to initialize.</param>
        private async Task InitializeHomePageAsync(HomePage homePage)
        {
            // Subscribe to the new page's events.

            // Initialize the any controls.
        }

        /// <summary>
        /// Initializes the given instance of the settings page.
        /// </summary>
        /// <param name="settingsPage">The instance of the settings page to initialize.</param>
        private async Task InitializeSettingsPageAsync(SettingsPage settingsPage)
        {
            // Subscribe to the new page's events.
            settingsPage.ChangeAccountConnectionRequested += this.SettingsPage_ChangeAccountConnectionRequested;
            settingsPage.OauthCodeAcquired += this.SettingsPage_OauthCodeAcquired;
            settingsPage.ConnectionRequestCancelled += this.SettingsPage_ConnectionRequestCancelled;

            // Add the accounts logged into with this app to the page.
            List<EmailProviderAccount> accounts = await AppConfig.Instance.GetAccountsAsync();
            foreach (EmailProviderAccount account in accounts)
            {
                // Get the account and add it to the list on the page.
                settingsPage.Accounts.Add(account);
            }
        }

        /// <summary>
        /// Starts the OAuth flow on the Settings Page.
        /// </summary>
        /// <returns></returns>
        private async Task StartOAuthFlowAsync(string accountName, SettingsPage settingsPage)
        {
            // Assign a unique ID to this authorization request so it can be identified later in
            //      the OAuth flow.
            this.accountIdPendingAuthorization = Guid.NewGuid();

            // Get the OAuth uri, and display it on the settings page.
            Uri oauthUri = GmailAPI.Instance.GetOAuthUri();
            await settingsPage.ShowServiceOAuthCodeUIAsync(accountName, oauthUri);
        }

        /// <summary>
        /// Populates the account info for the given account ID and adds that account to the app's data.
        /// </summary>
        /// <param name="accountId">The app-assigned ID of the account to get data for and add.</param>
        /// <returns></returns>
        private async Task AddNewAccountDataAsync(string accountId)
        {
            // Create the account model and add it to the app config.
            EmailProviderAccount account = await GmailAPI.Instance.GetAccountAsync(accountId);
            await AppConfig.Instance.AddAccountAsync(account);

            // If the home page is currently displayed...
            if (this.RootPage.CurrentPage is HomePage homePage)
            {
                
            }
            // If the settings page is currently displayed, refresh the API status message.
            else if (this.RootPage.CurrentPage is SettingsPage settingsPage)
            {
                // Add the account to the page.
                settingsPage.Accounts.Add(account);
            }
        }

        /// <summary>
        /// Disconnects an account.
        /// </summary>
        /// <param name="accountId">The ID of the account to remove.</param>
        /// <param name="settingsPage">The settings page instance to update with the disconnected account.</param>
        /// <returns></returns>
        private async Task DisconnectAccountAsync(string accountId, SettingsPage settingsPage)
        {
            // Remove the account's connection with Google.
            await GmailAPI.Instance.RemoveAccount(accountId);

            // Remove cahced account data.
            await AppConfig.Instance.RemoveAccountAsync(accountId);

            // Remove any necessary data from the UI associated with the removed account.

            // Remove the account from the settings page.
            EmailProviderAccount accountToRemove = settingsPage.Accounts.Where(a => a.ID == accountId).FirstOrDefault();
            if (accountToRemove != null)
            {
                settingsPage.Accounts.Remove(accountToRemove);
            }
        }
        #endregion
    }
}
