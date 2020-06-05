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

namespace LogInForm
{
    internal class AsyncClientEvent
    {
        static Form2 currentForm;

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

        public void StartClient(String userName, String userPassword, String eventName, string userID,  Form2 form2)
        {

            currentForm = form2;

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // The name of the
                // remote device is "host.contoso.com".  
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
                Send(client, userName, userPassword, userID);
                sendDone.WaitOne();

                // Receive the response from the remote device.  
                Receive(client);
                receiveDone.WaitOne();


                // Write the response to the console.  
                Console.WriteLine("Response received : {0}", response);


                if (currentForm != null)
                {
                    currentForm.getResponse();
                }

                // Release the socket.  
                client.Shutdown(SocketShutdown.Both);
                client.Close();

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

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, ClientStateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String userName, String userPassword, String userID)
        {
            Console.WriteLine("ENTERED -----------------------------------");
            // Convert the string data to byte data using ASCII encoding.
            var data = new UserRegisterData {
                eventName = "Register",
                userID = userID,
                userName = userName,
                userPassword = userPassword
            };

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

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

        public static byte[] ObjectToByteArray(UserRegisterData obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteArrayToObjectTest(byte[] arrBytes)
        {

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(arrBytes);
            return formatter.Deserialize(ms);
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

        public string msg;     //the only thing we really send.

        //Usually you shouldn't but these 2 methods in your class because they don't operate specifically on this object
        //and we would get allot of duplicate code if we would put those 2 methods in each class we would like to
        //be able to send but to not wind up having to write a couple of utility classes (where these should reside)
        // I let them reside here for now.
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
