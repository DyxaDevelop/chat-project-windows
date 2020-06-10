using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogInForm
{
    public partial class ChatListItemLayout : UserControl
    {
        public ChatListItemLayout()
        {
            InitializeComponent();
        }


        #region Properties

        private string userName;
        private string userMessage;

        [Category("Custom Props")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; UserNameLabel.Text = value; }
        }

        [Category("Custom Props")]
        public string UserMessage
        {
            get { return userMessage; }
            set { userMessage = value; MessageLabel.Text = value; }
        }

        #endregion

        private void UserNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void MessageLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
