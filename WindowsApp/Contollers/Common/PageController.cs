using Domain.Common;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.Contollers.Common
{
    /// <summary>
    /// Base controller for managing an app page.
    /// </summary>
    internal abstract class PageController<T, U> : Singleton<U>
        where T : Page
        where U : new()
    {
        #region Properties
        /// <summary>
        /// A reference to the view the controller is controlling.
        /// </summary>
        protected T View { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor - initializes all properties to null.
        /// </summary>
        public PageController()
        {
            this.View = null;
        }
        #endregion
    }
}
