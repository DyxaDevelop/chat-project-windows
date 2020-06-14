using MaterialSkin.Controls;
using MaterialSkin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LogInForm
{
    public partial class LoginForm : MaterialForm

    {
        public String userNickname;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void materialLabel1_Click_1(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Checks if the username and passwords are empty or not,
        /// validates password's lenth;
        /// if all requirements are met, starts the client, sending a request to the server;
        /// </summary>

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            userNickname = InputNickname.Text;
            string userPassword = InputPassword.Text;



            if (!String.IsNullOrEmpty(userNickname) && !String.IsNullOrEmpty(userPassword))
            {

                if (userPassword.Length < 6)
                {
                    MessageBox.Show("The password must be at least 6 characters.");
                }
                else
                {
                    LoginForm currentForm = this;

                    var asyncClientEvent = new AsyncClientEvent { };

                    asyncClientEvent.StartClientWithLoginForm(userNickname, userPassword, "Login", "null", currentForm);
                }



            }
            else
            {
                MessageBox.Show("Please fill in all the balnks");
            }
        }

        private void materialDivider1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opens the Register Form, creating a new instance of it;
        /// </summary>
        private void materialLabel1_Click_2(object sender, EventArgs e)
        {
            RegisterForm form2 = new RegisterForm();
            form2.Show();
            Hide();
        }

        /**
         * Shows the message box with received information
         * @param message - a string message, that needs to be displayed
         */

        public void showMessageBox(String message)
        {

            MessageBox.Show(message);
        }


        /// <summary>
        /// Opens the Chat Form as a user, creating a new instance of it;
        /// </summary>


        public void logIntoChatUser()
        {
            Messager chatForm = new Messager("null", userNickname, false);
            chatForm.Show();
            Hide();
        }


        /// <summary>
        /// Opens the Chat Form as an admin, creating a new instance of it;
        /// </summary>

        public void logIntoChatAdmin()
        {
            Messager chatForm = new Messager("null", userNickname, true);
            chatForm.Show();
            Hide();
        }

    }
}
