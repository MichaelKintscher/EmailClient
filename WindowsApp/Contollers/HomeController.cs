using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApp.Contollers.Common;
using WindowsApp.Pages;

namespace WindowsApp.Contollers
{
    /// <summary>
    /// The controller for managing the app home page.
    /// </summary>
    internal class HomeController : PageController<HomePage, HomeController>
    {
        #region Properties
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor - initializes all properties to null.
        /// </summary>
        public HomeController()
        {
            this.View = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the controller with the given view.
        /// </summary>
        /// <param name="view">The view this controller will control.</param>
        /// <returns></returns>
        internal async Task InitializeAsync(HomePage view)
        {
            // Subscribe to the page's events.

            // Store a reference to the page.
            this.View = view;
        }
        #endregion
    }
}
