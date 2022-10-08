using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.Contollers.Navigation.EventArguments
{
    /// <summary>
    /// Contains event info for when a navigation has completed.
    /// </summary>
    internal class NavigatedEventArgs : EventArgs
    {
        /// <summary>
        /// The page that was navigated from.
        /// </summary>
        public Page PageNavigatedFrom { get; private set; }

        /// <summary>
        /// The page that was navigated to.
        /// </summary>
        public Page PageNavigatedTo { get; private set; }

        public NavigatedEventArgs(Page pageNavigatedFrom, Page pageNavigatedTo)
        {
            this.PageNavigatedFrom = pageNavigatedFrom;
            this.PageNavigatedTo = pageNavigatedTo;
        }
    }
}
