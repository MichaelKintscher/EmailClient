using CleanArchitecture.Core.Domain.EventArguments;
using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.EventArguments
{
    /// <summary>
    /// Contains event info for when a Service Provider Account event is raised.
    /// </summary>
    internal class ServiceProviderAccountEventArgs : EntityEventArgs<ServiceProviderAccount>
    {
        /// <summary>
        /// The account the event was raised for.
        /// </summary>
        public ServiceProviderAccount Account
        {
            get => this.Value;
        }

        public ServiceProviderAccountEventArgs(ServiceProviderAccount account)
            : base(account) { }
    }
}
