using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Models
{
    /// <summary>
    /// Represents an email or other message.
    /// </summary>
    public class Message : INotifyPropertyChanged
    {
        private Guid _id;
        /// <summary>
        /// The unique ID of the message.
        /// </summary>
        public Guid Id
        {
            get { return this._id; }
            set
            {
                _id = value;
                this.RaisePropertyChanged("Id");
            }
        }

        private string _subject;
        /// <summary>
        /// The subject or header of the message.
        /// </summary>
        public string Subject
        {
            get { return this._subject; }
            set
            {
                this._subject = value;
                this.RaisePropertyChanged("Subject");
            }
        }

        private string _body;
        /// <summary>
        /// The body or content of the message.
        /// </summary>
        public string Body
        {
            get { return this._body; }
            set
            {
                this._body = value;
                this.RaisePropertyChanged("Body");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
