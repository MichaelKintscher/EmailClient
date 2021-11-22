using EmailClient.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EmailClient.Controls
{
    /// <summary>
    /// Displays the subject and a preview of a message for the bound Message model.
    /// </summary>
    public sealed partial class EmailControl : UserControl, INotifyPropertyChanged
    {
        private Message _message;
        /// <summary>
        /// The message to display.
        /// </summary>
        public Message Message
        {
            get { return this._message; }
            set
            {
                this._message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public EmailControl()
        {
            this.InitializeComponent();
        }
    }
}
