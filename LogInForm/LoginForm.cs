﻿using MaterialSkin.Controls;
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

        private void materialLabel1_Click_2(object sender, EventArgs e)
        {
            RegisterForm form2 = new RegisterForm();
            form2.Show();
            Hide();
        }

        public void showMessageBox(String message)
        {

            MessageBox.Show(message);
        }

        public void logIntoChatUser()
        {
            Messager chatForm = new Messager("null", userNickname, false);
            chatForm.Show();
            Hide();
        }

        public void logIntoChatAdmin()
        {
            Messager chatForm = new Messager("null", userNickname, true);
            chatForm.Show();
            Hide();
        }

    }

    public class ValidJSONClass
    {
        public bool isValidJSON(String json)
        {
            json = json.Trim();

            try
            {
                var obj = JToken.Parse(json);
                return true;
            }
            catch (JsonReaderException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
    }
}