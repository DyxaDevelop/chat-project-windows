using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogInForm
{
    public class ChatMessage
    {
        public string eventName { get; set; }
        public string timeStamp { get; set; }
        public string userName { get; set; }
        public string userMessage { get; set; }
    }
}
