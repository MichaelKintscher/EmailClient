// Updated by XamlIntelliSenseFileGenerator 10/8/2022 12:56:12 PM
#pragma checksum "C:\Users\micha\source\repos\EmailClient\WindowsApp\Pages\SettingsPage.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9AD9A05085A40F027A7CDF5D2563ADEEE28DD2EE28D5B51836AA6A4AA3DCDFCC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowsApp.Pages
{
    partial class SettingsPage : global::Microsoft.UI.Xaml.Controls.Page
    {
#pragma warning restore 0649
#pragma warning restore 0169
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler", " 1.0.0.0")]
        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler", " 1.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent()
        {
            if (_contentLoaded)
                return;

            _contentLoaded = true;

            global::System.Uri resourceLocator = new global::System.Uri("ms-appx:///Pages/SettingsPage.xaml");
            global::Microsoft.UI.Xaml.Application.LoadComponent(this, resourceLocator, global::Microsoft.UI.Xaml.Controls.Primitives.ComponentResourceLocation.Application);
        }

        partial void UnloadObject(global::Microsoft.UI.Xaml.DependencyObject unloadableObject);

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.UI.Xaml.Markup.Compiler", " 1.0.0.0")]
        private interface ISettingsPage_Bindings
        {
            void Initialize();
            void Update();
            void StopTracking();
            void DisconnectUnloadedObject(int connectionId);
        }

        private interface ISettingsPage_BindingsScopeConnector
        {
            global::System.WeakReference Parent { get; set; }
            bool ContainsElement(int connectionId);
            void RegisterForElementConnection(int connectionId, global::Microsoft.UI.Xaml.Markup.IComponentConnector connector);
        }

        internal global::Microsoft.UI.Xaml.Controls.ContentDialog OauthErrorDialog;
        internal global::Microsoft.UI.Xaml.Controls.TextBlock OAuthErrorTextBlock;
        internal global::Microsoft.UI.Xaml.Controls.ContentDialog FinishAddingServiceDialog;
        internal global::Microsoft.UI.Xaml.Controls.WebView2 OAuthWebView;
        internal global::Microsoft.UI.Xaml.Controls.TextBox ServiceOauthCodeTextBox;
        internal global::Microsoft.UI.Xaml.Controls.ContentDialog ConfirmRemoveAccountDialog;
        internal global::Microsoft.UI.Xaml.Controls.ContentControl AccountToRemoveContentControl;
        internal global::Microsoft.UI.Xaml.Controls.ListView AccountsListView;
        internal global::Microsoft.UI.Xaml.Controls.Button AuthenticateGoogleButton;
#pragma warning restore 0649
#pragma warning restore 0169
    }
}


