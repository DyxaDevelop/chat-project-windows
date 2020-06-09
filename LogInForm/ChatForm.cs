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
using System.Threading;

namespace LogInForm
{
    public partial class Messager : MaterialForm
    {
        public String userID;
        public String userName;

        public Messager(String userID, String userName)
        {
            InitializeComponent();
            this.userID = userID;
            this.userName = userName;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            getMessages();
        }

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            String userMessage = MessageTextBox.Text;


            if (!String.IsNullOrEmpty(userMessage))
            {
                MessageTextBox.Text = String.Empty;
                Messager currentForm = this;

                var asyncClientEvent = new AsyncClientEvent { };

                asyncClientEvent.StartClientWithChatForm(userName, "null", "Send_Message", userID, currentForm, userMessage);

            }
            else
            {
                MessageBox.Show("Please fill in all the balnks");
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        public void populateChat(List<ChatMessage> chatMessages)
        {
                ChatListItem[] chatListItems = new ChatListItem[chatMessages.Count];

                for (int i = 0; i < chatListItems.Length; i++)
                {
                    chatListItems[i] = new ChatListItem();
                    chatListItems[i].UserName = chatMessages[i].userName;
                    chatListItems[i].UserMessage = chatMessages[i].userMessage;

                    UpdateFlowLayoutPanel(chatListItems[i], i, chatListItems.Length - 1);
                }


        }

        private void UpdateFlowLayoutPanel(ChatListItem currentChatMessage, int currentItemCount, int listSize)
        {

            Thread FlowLayoutThread = new Thread(delegate () {
                if (flowLayoutPanel1.InvokeRequired)
                {
                    flowLayoutPanel1.Invoke(new MethodInvoker(delegate
                    {
                        if (flowLayoutPanel1.Controls.Count < 0)
                        {
                            flowLayoutPanel1.Controls.Clear();
                        }
                        else
                        {
                            if(currentItemCount == 0)
                            {
                                flowLayoutPanel1.Controls.Clear();
                            }

                            flowLayoutPanel1.Controls.Add(currentChatMessage);

                            if(currentItemCount == listSize)
                            {
                                flowLayoutPanel1.ScrollControlIntoView(currentChatMessage);
                            }
                        }
                    }));
                }
            });

            FlowLayoutThread.Start();

        }

        private void getMessages()
            {
                Messager currentForm = this;

                var asyncClientEvent = new AsyncClientEvent { };

                asyncClientEvent.StartClientWithChatForm("null", "null", "Get_Messages", "null", currentForm, "null");
        }
        public void showMessageBox(String message)
        {
            MessageBox.Show(message);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MessageTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
