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

namespace LogInForm
{
    public partial class Messager : MaterialForm
    {
        public Messager()
        {
            InitializeComponent();
         
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            populateChat();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void populateChat()
        {
            ChatListItem[] chatListItems = new ChatListItem[10];

            for (int i = 0; i < chatListItems.Length; i++)
            {
                chatListItems[i] = new ChatListItem();
                chatListItems[i].UserName = "Some Username";
                chatListItems[i].UserMessage = "Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum Lorem ipsum ";
                if (flowLayoutPanel1.Controls.Count < 0)
                {
                    flowLayoutPanel1.Controls.Clear();
                }
                else
                {
                    flowLayoutPanel1.Controls.Add(chatListItems[i]);
                }
            }
        }
    }
}
