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
    public partial class Form2 : MaterialForm

    {


        FirebaseClient firebaseClient = new FirebaseClient(
              "https://chat-project-windows.firebaseio.com/",
              new FirebaseOptions
              {
                  AuthTokenAsyncFactory = () => Task.FromResult("tHmSMQiVIpGNN3SyXUFhaeefKQmE55rpzwpw4aj6")
              });


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
           

        }

        private void materialLabel3_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            Hide();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {

            Guid guid = Guid.NewGuid();
            string ID = guid.ToString();

            string userID = ID;
            string userNickname = InputNickname.Text;
            string userPassword = InputPassword.Text;



            if (!String.IsNullOrEmpty(userNickname) && !String.IsNullOrEmpty(userPassword)) {

                Form2 currentForm = this;

                var asyncClientEvent = new AsyncClientEvent { };

                asyncClientEvent.StartClient(userNickname, userPassword, "register", ID ,currentForm);

            }


        }

        public void getResponse() {

            MessageBox.Show("Received Response");
        }

        private void InputPassword_Click(object sender, EventArgs e)
        {

        }
    }
}
