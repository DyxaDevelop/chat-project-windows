using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogInForm
{

    /// <summary>
    /// Message in the chat - 
    /// eventname for passing the data;
    /// timeStamp - time of it being sent;
    /// userName - username of the user sending it;
    /// userMessage - message that is being sent;
    /// </summary>
    public class ChatMessage
    {
        public string eventName { get; set; }
        public string timeStamp { get; set; }
        public string userName { get; set; }
        public string userMessage { get; set; }
    }
}
