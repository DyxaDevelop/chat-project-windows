using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogInForm
{

    [Serializable]
    internal class UserRegisterData
    {
        public string eventName { get; set; }
        public string userID { get; set; }
        public string userName { get; set; }
        public string userPassword { get; set; }
    }
}
