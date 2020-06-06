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

namespace LogInForm
{
    public partial class Form1 : MaterialForm

    {

        public Form1()
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
            string userNickname = InputNickname.Text;
            string userPassword = InputPassword.Text;



            if (!String.IsNullOrEmpty(userNickname) && !String.IsNullOrEmpty(userPassword))
            {

                Form1 currentForm = this;

                var asyncClientEvent = new AsyncClientEvent { };

                asyncClientEvent.StartClientWithForm1(userNickname, userPassword, "Login", "null", currentForm);

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
            Form2 form2 = new Form2();
            form2.Show();
            Hide();
        }

        public void showMessageBox(String message)
        {

            MessageBox.Show(message);
        }

        public void logIntoChat()
        {
            Messager chatForm = new Messager();
            chatForm.Show();
            Hide();
        }
    }
}
