using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogInForm
{

    /// <summary>
    /// Data of the user - 
    /// eventname for passing the data;
    /// userID - his unique ID;
    /// userName - username of the user sending it;
    /// userPassword - password of the user;
    /// </summary>

    [Serializable]
    internal class UserData
    {
        public string eventName { get; set; }
        public string userID { get; set; }
        public string userName { get; set; }
        public string userPassword { get; set; }
    }
}
