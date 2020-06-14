using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogInForm
{

    /// <summary>
    /// Disconnect request
    /// eventName - used to indentufy the request on the server
    /// </summary>
    public class DisconnectRequest
    {
        public string eventName { get; set; }
    }
}
