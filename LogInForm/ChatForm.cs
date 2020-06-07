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
        public Messager()
        {
            InitializeComponent();
         
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            getMessages();
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

        public void populateChat(List<ChatMessage> chatMessages)
        {
                ChatListItem[] chatListItems = new ChatListItem[chatMessages.Count];

                for (int i = 0; i < chatListItems.Length; i++)
                {
                    chatListItems[i] = new ChatListItem();
                    chatListItems[i].UserName = chatMessages[i].userName;
                    chatListItems[i].UserMessage = chatMessages[i].userMessage;

                    UpdateFlowLayoutPanel(chatListItems[i]);
            }


        }

        private void UpdateFlowLayoutPanel(ChatListItem currentChatMessage)
        {

            Thread FlowLayoutThread = new Thread(delegate () {

                // Change the status of the buttons inside the TypingThread
                // This won't throw an exception anymore !
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
                            flowLayoutPanel1.Controls.Add(currentChatMessage);
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

                asyncClientEvent.StartClientWithChatForm("null", "null", "Get_Messages", "null", currentForm);
        }
        public void showMessageBox(String message)
        {
            MessageBox.Show(message);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
