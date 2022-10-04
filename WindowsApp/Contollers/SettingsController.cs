using Controllers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsApp.Pages;

namespace WindowsApp.Contollers
{
    /// <summary>
    /// The controller for managing the app settings.
    /// </summary>
    internal class SettingsController : Singleton<SettingsController>
    {
        #region Properties
        private SettingsPage View { get; set; }
        #endregion

        #region Methods
        public void Initialize(SettingsPage view)
        {
            this.View = view;
        }
        #endregion
    }
}
