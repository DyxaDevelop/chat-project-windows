using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace LogInForm
{
    internal class AsyncClientEvent
    {
        static Form1 currentLoginForm;
        static Form2 currentRegisterForm;
        static Messager currentChatForm;

        public static String extraData = "null";

        public AsyncClientEvent() { }

        // The port number for the remote device.  
        private const int port = 11000;

        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        // The response from the remote device.  
        private static String response = String.Empty;

        public void StartClientWithForm1(String userName, String userPassword, String eventName, string userID, Form1 form1)
        {
            currentLoginForm = form1;
            StartClient(userName, userPassword, eventName, userID);
        }

        public void StartClientWithForm2(String userName, String userPassword, String eventName, string userID, Form2 form2)
        {
            currentRegisterForm = form2;
            StartClient(userName, userPassword, eventName, userID);
        }

        public void StartClientWithChatForm(String userName, String userPassword, String eventName, string userID, Messager chatForm, String userMessage)
        {
            currentChatForm = chatForm;
            extraData = userMessage;
            StartClient(userName, userPassword, eventName, userID);
        }

        public void StartClient(String userName, String userPassword, String eventName, string userID)
        {

            // Connect to a remote device.  
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP socket.  
                Socket client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.  
                client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                // Send test data to the remote device.  
                Send(client, eventName, userName, userPassword, userID);
                sendDone.WaitOne();

                if(eventName != "Send_Message")
                {
                    // Receive the response from the remote device.  
                    Receive(client);
                    receiveDone.Reset();
                    receiveDone.WaitOne();

                    // Write the response to the console.  
                    Console.WriteLine("Response received : {0}", response);

                    switch (response)
                    {
                        case "User created!":
                            currentRegisterForm.showMessageBox(response);
                            currentRegisterForm.logIntoChat();
                            break;

                        case "User already exists!":
                            currentRegisterForm.showMessageBox(response);
                            break;

                        case "Login successful!":
                            currentLoginForm.showMessageBox(response);
                            currentLoginForm.logIntoChat();
                            break;

                        case "Data incorrect!":
                            currentLoginForm.showMessageBox(response);
                            break;

                        case "Some Messages":
                            currentChatForm.showMessageBox(response);
                            break;

                        case "Message Sent!":
                            currentChatForm.showMessageBox(response);
                            break;

                        case "User doesn't exist!":
                            currentLoginForm.showMessageBox(response);
                            break;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                ClientStateObject state = new ClientStateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, ClientStateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {

            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object. 
                ClientStateObject state = (ClientStateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);
                Console.WriteLine("Bytes Read: " + bytesRead);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    response = state.sb.ToString();

                }
                    if(response.Equals("User created!") ||
                       response.Equals("User already exists!") ||
                       response.Equals("Login successful!") ||
                       response.Equals("Data incorrect!") ||
                       response.Equals("User doesn't exist!") ||
                       response.Equals("Some Messages") ||
                       response.Equals("Message Sent!") ||
                       isValidJSON(response) == true)
                    {
                         // All the data has arrived; put it in response.  

                            if(isValidJSON(response) == true)
                                {
                                    List<ChatMessage> receivedChatMessageList = JsonConvert.DeserializeObject<List<ChatMessage>>(response);
                                    currentChatForm.populateChat(receivedChatMessageList);
                                }
                            
                         // Signal that all bytes have been received.
                            state.sb.Clear();
                            receiveDone.Set();

                            Receive(client);
                }
                    else
                {
                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, ClientStateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + "Exception");
            }
        }

        private static void Send(Socket client, String eventName, String userName, String userPassword, String userID)
        {
            string json;

            if(eventName.Equals("Send_Message"))
            {
                var messageData = new ChatMessage
                {
                    eventName = eventName,
                    userName = userName,
                    userMessage = extraData
                };

                json = JsonConvert.SerializeObject(messageData, Formatting.Indented);
            }
            else
            {
                var userData = new UserRegisterData
                {
                    eventName = eventName,
                    userID = userID,
                    userName = userName,
                    userPassword = userPassword
                };

                json = JsonConvert.SerializeObject(userData, Formatting.Indented);
            }          

            byte[] byteData = Encoding.ASCII.GetBytes(json);

            // Begin sending the data to the remote device. 
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static bool isValidJSON(String json)
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
    [Serializable]
    public class Status
    {
        [NonSerialized]
        public Socket Socket;
        [NonSerialized]
        public List<byte> TransmissionBuffer = new List<byte>();
        [NonSerialized]
        public byte[] buffer = new byte[1024];

        public string msg; 
        public byte[] Serialize()
        {
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            bin.Serialize(mem, this);
            return mem.GetBuffer();
        }

        public Status DeSerialize()
        {
            byte[] dataBuffer = TransmissionBuffer.ToArray();
            BinaryFormatter bin = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();
            mem.Write(dataBuffer, 0, dataBuffer.Length);
            mem.Seek(0, 0);
            return (Status)bin.Deserialize(mem);
        }
    }
}
