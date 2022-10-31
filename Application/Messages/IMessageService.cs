using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Messages
{
    public interface IMessageService
    {
        /// <summary>
        /// The name of the API.
        /// </summary>
        public string Name { get; set; }
    }
}
