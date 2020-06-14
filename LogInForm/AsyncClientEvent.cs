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
    public class AsyncClientEvent
    {

        /// <summary>
        /// Defines all the necessary variables and fields;
        /// </summary>

        static LoginForm currentLoginForm;
        static RegisterForm currentRegisterForm;
        static Messager currentChatForm;

        public static bool isDisconnecting = false;

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


        /// <summary>
        /// All the different methods available, in order to start the client
        /// from all different forms in the app, and in order to send different
        /// requests to the server, passing the necessary data to them
        /// </summary>


        /**
         * This method is used to start the client in order to send a login request to the sever;
         * @param userName - username for logging in
         * @param userPassword - password for logging in
         * @param eventName - eventName, that will be equal to Login, for the server to recognise the response
         * @param userID - ID of the user
         * @param form1 - the Login form, where the function is called from, to call methods on it later
         */
        public void StartClientWithLoginForm(String userName, String userPassword, String eventName, string userID, LoginForm form1)
        {
            currentLoginForm = form1;
            StartClient(userName, userPassword, eventName, userID);
        }

        /**
         * This method is used to start the client in order to send a register request to the sever;
         * @param userName - username for registration
         * @param userPassword - password for registration
         * @param eventName - eventName, that will be equal to Register, for the server to recognise the response
         * @param userID - ID of the user
         * @param form2 - the Register form, where the function is called from, to call methods on it later
         */
        public void StartClientWithRegisterForm(String userName, String userPassword, String eventName, string userID, RegisterForm form2)
        {
            currentRegisterForm = form2;
            StartClient(userName, userPassword, eventName, userID);
        }

        /**
         * This method is used to start the client in order to send a request to the server to send or get messages from the database
         * @param userName - username of current user
         * @param userPassword - password of current user
         * @param eventName - eventName, that will be equal to Send_Message or Get_Messages, for the server to recognise the response
         * @param userID - ID of the user
         * @param chatForm - the current Chat form, where the function is called from, to call methods on it later
         * @param userMessage - the message, that the user wants to send to the chat
         */
        public void StartClientWithChatForm(String userName, String userPassword, String eventName, string userID, Messager chatForm, String userMessage)
        {
            currentChatForm = chatForm;
            extraData = userMessage;
            StartClient(userName, userPassword, eventName, userID);
        }

        /**
         * This method is used to start the client in order to send a request to the server to delete a message from the database
         * @param userName - username of current user
         * @param messageTimeStamp - the time of the message being sent
         * @param eventName - eventName, that will be equal to Delete_Message, for the server to recognise the response
         * @param chatForm - the current Chat form, where the function is called from, to call methods on it later
         */
        public void StartClientWithChatFormDeleteMessage(String userName, String messageTimeStamp, String eventName, Messager chatForm)
        {
            currentChatForm = chatForm;
            extraData = messageTimeStamp;
            StartClient(userName, "null", eventName, "null");
        }

        /**
         * This method is used to start the client in order to disconnect from the server
         * @param eventName - eventName, that will be equal to Disconnect, for the server to recognise the response
         * @param chatForm - the current Chat form, where the function is called from, to call methods on it later
         */
        public void StartClientWithChatFormDisconnect(String eventName, Messager chatForm)
        {
            StartClient("null", "null", eventName, "null");
        }

        /**
         * Starts the client, in order to send requests and receive responses to and from the server
         * @param userName - username of the current user
         * @param userPassword - password of the current user
         * @param eventName - eventName, that's used to recognise the request
         * @param chatForm - the current Chat form, where the function is called from, to call methods on it later
         */
        public void StartClient(String userName, String userPassword, String eventName, string userID)
        {


            /// <summary>
            /// Connects to the remote server,
            /// meaning to the server
            /// </summary>
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


                /// <summary>
                /// Determines, whether the program should listen to some kind of respone from the server,
                /// or just move on without expecting any response;
                /// This depends on the type of request sent by the client
                /// </summary>

                if (eventName != "Send_Message" ||
                   eventName != "Delete_Message" ||
                   eventName != "Disconnect")
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
                            currentLoginForm.logIntoChatUser();
                            break;

                        case "Admin Login successful!":
                            currentLoginForm.showMessageBox(response);
                            currentLoginForm.logIntoChatAdmin();
                            break;

                        case "Password incorrect!":
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

                if (isDisconnecting == true)
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
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

        /**
         * Receive and ReceiveCallBack are used to receive, decode, and handle the data
         * received from the server as a response to the request from the client;
         * @param client - the current socket, used to send requests
         */

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
                if (response.Equals("User created!") ||
                   response.Equals("User already exists!") ||
                   response.Equals("Login successful!") ||
                   response.Equals("Admin Login successful!") ||
                   response.Equals("Password incorrect!") ||
                   response.Equals("User doesn't exist!") ||
                   response.Equals("Some Messages") ||
                   response.Equals("Message Sent!") ||
                   isValidJSON(response) == true)
                {
                    // All the data has arrived; put it in response.  

                    if (isValidJSON(response) == true)
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

        /**
         * Sends the data to the server, encoding it into ASCII,
         * and encapsulating into the needed class, and that depends
         * on the type of request, that is being sent to the server
         * @param client - the current socket of the client,
         * @param eventName - the name of the event, so the server can recognise the request
         * @param userName - current username of the user
         * @param userPassword - password of current user
         * @param userID - id of the user
         */

        private static void Send(Socket client, String eventName, String userName, String userPassword, String userID)
        {
            string json;


            switch (eventName)
            {
                case "Send_Message":

                    var messageData = new ChatMessage
                    {
                        eventName = eventName,
                        timeStamp = "null",
                        userName = userName,
                        userMessage = extraData
                    };

                    json = JsonConvert.SerializeObject(messageData, Formatting.Indented);
                    break;

                case "Delete_Message":

                    var deleteMessageData = new ChatMessage
                    {
                        eventName = eventName,
                        timeStamp = extraData,
                        userName = userName,
                        userMessage = extraData
                    };

                    json = JsonConvert.SerializeObject(deleteMessageData, Formatting.Indented);
                    break;

                case "Disconnect":

                    var disconnectRequestData = new DisconnectRequest
                    {
                        eventName = eventName
                    };

                    json = JsonConvert.SerializeObject(disconnectRequestData, Formatting.Indented);
                    isDisconnecting = true;
                    break;

                default:
                    var userData = new UserData
                    {
                        eventName = eventName,
                        userID = userID,
                        userName = userName,
                        userPassword = userPassword
                    };

                    json = JsonConvert.SerializeObject(userData, Formatting.Indented);
                    break;

            }

            byte[] byteData = Encoding.ASCII.GetBytes(json);

            // Begin sending the data to the remote device. 
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }



        /**
         * Tests and displays the actual amount of
         * data (bytes) that was sent to the server;
         * @param ar - the state of the request that was sent to the server
         */

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

        /**
         * Determines, whether the received string
         * is a valid, readable JSON, that we will be able to read data from;
         * @param json - a string, that is being checked, whether it is a valid json, or just a string
         * @return Returns a true or false value, depending on the result
         */

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
    }
}
