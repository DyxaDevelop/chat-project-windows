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
        public bool isAdmin;

        public Messager(String userID, String userName, bool isAdmin)
        {
            InitializeComponent();
            this.userID = userID;
            this.userName = userName;
            this.isAdmin = isAdmin;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            getMessages();
            WelcomeLabel.Text += userName;
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
            if(isAdmin == false)
            {
                ChatListItemLayout[] chatListItems = new ChatListItemLayout[chatMessages.Count];

                for (int i = 0; i < chatListItems.Length; i++)
                {
                    chatListItems[i] = new ChatListItemLayout();
                    chatListItems[i].UserName = chatMessages[i].userName;
                    chatListItems[i].UserMessage = chatMessages[i].userMessage;

                    if(chatListItems[i].UserName == "admin")
                    {
                        chatListItems[i].BackColor = Color.FromArgb(192, 255, 192);
                    }

                    UpdateFlowLayoutPanel(chatListItems[i], i, chatListItems.Length - 1, this.isAdmin);
                }
            }

            if(isAdmin == true)
            {
                ChatListItemAdmin[] chatListItemsAdmin = new ChatListItemAdmin[chatMessages.Count];

                for (int i = 0; i < chatListItemsAdmin.Length; i++)
                {
                    chatListItemsAdmin[i] = new ChatListItemAdmin(this);
                    chatListItemsAdmin[i].UserName = chatMessages[i].userName;
                    chatListItemsAdmin[i].UserMessage = chatMessages[i].userMessage;
                    chatListItemsAdmin[i].MessageTimeStamp = chatMessages[i].timeStamp;

                    if (chatListItemsAdmin[i].UserName == "admin")
                    {
                        chatListItemsAdmin[i].BackColor = Color.FromArgb(192, 255, 192);
                    }

                    UpdateFlowLayoutPanel(chatListItemsAdmin[i], i, chatListItemsAdmin.Length - 1, this.isAdmin);
                }
            }


        }

        private void UpdateFlowLayoutPanel(Object currentChatMessageObject, int currentItemCount, int listSize, bool isAdmin)
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

                                if (isAdmin == true)
                                {
                                    ChatListItemAdmin currentChatMessageAdmin = (ChatListItemAdmin)currentChatMessageObject;

                                        flowLayoutPanel1.Controls.Add(currentChatMessageAdmin);

                                        if (currentItemCount == listSize)
                                        {
                                            flowLayoutPanel1.ScrollControlIntoView(currentChatMessageAdmin);
                                        }
                                }
                                else
                                {
                                    ChatListItemLayout currentChatMessageUser = (ChatListItemLayout)currentChatMessageObject;

                                        flowLayoutPanel1.Controls.Add(currentChatMessageUser);

                                        if (currentItemCount == listSize)
                                        {
                                            flowLayoutPanel1.ScrollControlIntoView(currentChatMessageUser);
                                        }
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

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            Messager currentForm = this;

            var asyncClientEvent = new AsyncClientEvent { };

            asyncClientEvent.StartClientWithChatFormDisconnect("Disconnect", currentForm);

            Form1 loginForm = new Form1();
            loginForm.Show();
            Hide();
        }
    }
}
