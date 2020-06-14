using System;
using MaterialSkin.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Firebase.Database;
using Firebase;
using Firebase.Database.Query;
using Firebase.Database.Streaming;

namespace LogInForm
{
    public partial class RegisterForm : MaterialForm

    {
        public String userID;
        public String userNickname;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {


        }

        private void materialLabel3_Click(object sender, EventArgs e)
        {
            LoginForm form1 = new LoginForm();
            form1.Show();
            Hide();
        }



        /// <summary>
        /// Performs all the necessary checks to the data,
        /// validating the length of them, and sends a request to the server
        /// through the AsyncClientEvent class;
        /// </summary>

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {

            Guid guid = Guid.NewGuid();
            string ID = guid.ToString();

            userID = ID;
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
                    RegisterForm currentForm = this;

                    var asyncClientEvent = new AsyncClientEvent { };

                    asyncClientEvent.StartClientWithRegisterForm(userNickname, userPassword, "Register", userID, currentForm);
                }

            }
            else
            {
                MessageBox.Show("Please fill in all the balnks");
            }


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

        public void logIntoChat()
        {
            Messager chatForm = new Messager(userID, userNickname, false);
            chatForm.Show();
            Hide();

        }

        private void InputPassword_Click(object sender, EventArgs e)
        {

        }
    }
}
