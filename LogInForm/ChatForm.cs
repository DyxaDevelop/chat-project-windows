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
        public String currentUserName;
        public bool isAdmin;

        public Messager(String userID, String userName, bool isAdmin)
        {
            InitializeComponent();
            this.userID = userID;
            this.currentUserName = userName;
            this.isAdmin = isAdmin;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            getMessages();
            WelcomeLabel.Text += currentUserName;
        }



        /// <summary>
        /// Gets the message from the textbox, validates it;
        /// Sends a request through the AsyncClientEvent class to the server;
        /// </summary>

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            String userMessage = MessageTextBox.Text.Trim();

            if (!String.IsNullOrEmpty(userMessage))
            {
                MessageTextBox.Text = String.Empty;
                Messager currentForm = this;

                var asyncClientEvent = new AsyncClientEvent { };

                asyncClientEvent.StartClientWithChatForm(currentUserName, "null", "Send_Message", userID, currentForm, userMessage);

            }
            else
            {
                MessageBox.Show("Please type in your message");
            }

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }


        /// <summary>
        /// Populates the chat with messages, validating the type of the message
        /// if the userName is admin, makes the background of the message green;
        /// if the userName is different from the current user - makes it light grey;
        /// if the userName is current's user userName, makes no changes to the layout;
        /// </summary>

        public void populateChat(List<ChatMessage> chatMessages)
        {
            if (isAdmin == false)
            {
                ChatListItemLayout[] chatListItems = new ChatListItemLayout[chatMessages.Count];

                for (int i = 0; i < chatListItems.Length; i++)
                {
                    chatListItems[i] = new ChatListItemLayout();
                    chatListItems[i].UserName = chatMessages[i].userName;
                    chatListItems[i].UserMessage = chatMessages[i].userMessage;
                    if (chatListItems[i].UserName != currentUserName)
                    {
                        if (chatListItems[i].UserName == "admin")
                        {
                            chatListItems[i].BackColor = Color.FromArgb(192, 255, 192);
                        }
                        else
                        {
                            chatListItems[i].BackColor = Color.FromArgb(209, 209, 209);
                        }
                    }

                    UpdateFlowLayoutPanel(chatListItems[i], i, chatListItems.Length - 1, this.isAdmin);
                }
            }

            if (isAdmin == true)
            {
                ChatListItemAdmin[] chatListItemsAdmin = new ChatListItemAdmin[chatMessages.Count];

                for (int i = 0; i < chatListItemsAdmin.Length; i++)
                {
                    chatListItemsAdmin[i] = new ChatListItemAdmin(this);
                    chatListItemsAdmin[i].UserName = chatMessages[i].userName;
                    chatListItemsAdmin[i].UserMessage = chatMessages[i].userMessage;
                    chatListItemsAdmin[i].MessageTimeStamp = chatMessages[i].timeStamp;

                    if (chatListItemsAdmin[i].UserName != currentUserName)
                    {
                        if (chatListItemsAdmin[i].UserName == "admin")
                        {
                            chatListItemsAdmin[i].BackColor = Color.FromArgb(192, 255, 192);
                        }
                        else
                        {
                            chatListItemsAdmin[i].BackColor = Color.FromArgb(209, 209, 209);
                        }
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
                            if (currentItemCount == 0)
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


        /// <summary>
        /// Sends a request to the server through the
        /// AsyncClientEvent, in order to get all the messages in the chat
        /// </summary>

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



        /// <summary>
        /// Logs the user out of his account, sending a request to the server;
        /// </summary>

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            Messager currentForm = this;

            var asyncClientEvent = new AsyncClientEvent { };

            asyncClientEvent.StartClientWithChatFormDisconnect("Disconnect", currentForm);

            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            Hide();
        }
    }
}
