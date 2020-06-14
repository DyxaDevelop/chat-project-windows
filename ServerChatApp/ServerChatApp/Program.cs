using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Firebase.Database;
using Firebase;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using System.Threading.Tasks;


public class StateObject
{

    public Socket workSocket = null;

    public const int BufferSize = 1024;

    public byte[] buffer = new byte[BufferSize];

    public StringBuilder sb = new StringBuilder();

}

public class AsynchronousSocketListener
{
    /// <summary>
    /// Defining the needed variables and Lists;
    /// Lists of users, connected devices and messages;
    /// </summary>

    public static List<UserData> allUsersList = new List<UserData>();

    public static List<ChatMessage> allMessagesList = new List<ChatMessage>();

    public static List<Socket> allConnectedDevices = new List<Socket>();

    public static bool firstCheck = true;

    public static int initialMessageCount;

    /// <summary>
    /// Adding a connection to firebase using a unique key to the databse
    /// </summary>

    public static FirebaseClient firebaseClient = new FirebaseClient(
      "https://chat-project-windows.firebaseio.com/",
      new FirebaseOptions
      {
          AuthTokenAsyncFactory = () => Task.FromResult("tHmSMQiVIpGNN3SyXUFhaeefKQmE55rpzwpw4aj6")
      });


    public static ManualResetEvent allDone = new ManualResetEvent(false);

    public AsynchronousSocketListener()
    {
    }


    /// <summary>
    /// Starts listening to any requests from clients on the port 11000
    /// </summary>
    public static void StartListening()
    {

        getAllUsers();

        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        Console.WriteLine(ipHostInfo);
        IPAddress ipAddress = ipHostInfo.AddressList[0];
        Console.WriteLine(ipAddress);
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                allDone.Reset();

                Console.WriteLine("Waiting for a connection...");
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                allDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    /// <summary>
    /// 
    /// 
    /// </summary>
    /// 

    /**
     * Accepts and receives the data from the clients, in order to process it later;
     * Calls the ReadCallBack;
     * @param ar - gets the status of the current async operation, along with the data
    */

    public static void AcceptCallback(IAsyncResult ar)
    {
        allDone.Set();

        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);


        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
    }

    /**
      * Decodes the received data from the client, and acts accordingly to its content;
      * Identifies the requests and runs the code further
      * @param ar - gets the status of the current async operation, along with the data
     */

    public static void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

        int bytesRead = handler.EndReceive(ar);


        if (bytesRead > 0)
        {
            state.sb.Append(Encoding.ASCII.GetString(
                state.buffer, 0, bytesRead));

            content = state.sb.ToString();
            JObject json = JObject.Parse(content);

            string eventNameReceived = json["eventName"].Value<string>();
            if (eventNameReceived.Equals("Register"))
            {

                UserData receivedUserRegister = JsonConvert.DeserializeObject<UserData>(content);

                checkIfUserExists(handler, receivedUserRegister);
            }

            if (eventNameReceived.Equals("Login"))
            {
                UserLoginData receivedUserLogin = JsonConvert.DeserializeObject<UserLoginData>(content);

                attemptUserLogin(handler, receivedUserLogin);
            }

            if (eventNameReceived.Equals("Get_Messages"))
            {

                string messageJSON = JsonConvert.SerializeObject(allMessagesList, Formatting.Indented);

                Send(handler, messageJSON);
            }

            if (eventNameReceived.Equals("Send_Message"))
            {

                ChatMessage receivedUserMessage = JsonConvert.DeserializeObject<ChatMessage>(content);

                ChatMessageForUpload convertedUserMessage = new ChatMessageForUpload
                {
                    userName = receivedUserMessage.userName,
                    userMessage = receivedUserMessage.userMessage
                };

                uploadMessage(convertedUserMessage);
            }

            if (eventNameReceived.Equals("Delete_Message"))
            {
                ChatMessage receivedDeleteMessage = JsonConvert.DeserializeObject<ChatMessage>(content);

                deleteMessage(receivedDeleteMessage);
            }

            if (eventNameReceived.Equals("Disconnect"))
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
    }

    /**
     * Encodes the string data into ASCII, and sends to the defined client, 
     * using its unique socket
     * @param handler - socket of the client, that the data will be sent to; is needed to send it;
     * @param data - the message that is being sent in a string style, whether a simple message, or in JSON format
     */


    private static void Send(Socket handler, String data)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }

    /**
     * Makes sure what amount of bytes was sent, and displays it into the console
     * @param ar - gets the status of the current async operation, along with the data
     */

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket handler = (Socket)ar.AsyncState;

            int bytesSent = handler.EndSend(ar);

            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static int Main(String[] args)
    {
        SubscribeToMessages();
        StartListening();
        return 0;
    }

    /**
     * Adds the device (its socket) to the list of all connected devices;
     * @param handler - Socket of the device, that we want to add to the list
     */
    public static void addToListOfConnectedDevices(Socket handler)
    {
        allConnectedDevices.Add(handler); // Add connected device to list
    }


    /**
     * Starts listening to the changes in the firebase database;
     * Specifically, listens to the directory "Chat", where all
     * the messages are stored
     */

    public static async void SubscribeToMessages()
    {
        var messages = await firebaseClient
            .Child("Chat")
            .OnceAsync<ChatMessage>();

        initialMessageCount = messages.Count;

        var observable = firebaseClient
              .Child("Chat")
              .OrderByKey()
              .AsObservable<ChatMessage>()
              .Subscribe(d => getCurrentMessageCount(d.Object, d.Key));

    }

    /**
     * Gets the current amount of messages in the chat from firebase
     * @param message - the message being downloaded, that will be sent to another function
     * @param messageTimeStamp - time of that message;
     */
    public static async void getCurrentMessageCount(ChatMessage message, string messageTimeStamp)
    {
        var messages = await firebaseClient
            .Child("Chat")
            .OnceAsync<ChatMessage>();

        int currentMessageCount = messages.Count;

        addAndDisplayMessages(message, currentMessageCount, messageTimeStamp);

    }

    /**
     * Adds the message to the list of messages, that is stored locally in the firebase;
     * That list is used to send out to the connected clients;
     * @param currentMessage - the message that is being added
     * @param currentMessageCount - amount of messages already in the firebase
     * @param messageTimeStamp - time of that message sent
     */

    public static void addAndDisplayMessages(ChatMessage currentMessage, int currentMessageCount, string messageTimeStamp)
    {

        currentMessage.eventName = "ChatMessage";
        if (currentMessageCount < allMessagesList.Count)
        {
            allMessagesList.Remove(currentMessage);

            if (firstCheck != true)
            {
                initialMessageCount--;
            }
        }
        if (currentMessageCount > allMessagesList.Count)
        {
            currentMessage.timeStamp = messageTimeStamp;
            allMessagesList.Add(currentMessage);

            if (firstCheck != true)
            {
                initialMessageCount++;
            }
        }

        if (allMessagesList.Count == initialMessageCount)
        {
            string messageJSON = JsonConvert.SerializeObject(allMessagesList, Formatting.Indented);

            Console.WriteLine(displayAllUserMessageCount(allMessagesList));

            SendAllMessages(allConnectedDevices, messageJSON);
            firstCheck = false;
        }
    }
    /**
     * Broadcasts the received data to all the users in
     * the list of currently connected clients
     * @param receivers - receives a list of receivers
     * @param messageJSON - the new message to broadcast, coded as a JSON
    */

    public static void SendAllMessages(List<Socket> receivers, String messageJSON)
    {
        foreach (Socket currentHandler in receivers)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(messageJSON);


            try
            {
                currentHandler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), currentHandler);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message.ToString());
            }

        }
    }

    /// <summary>
    /// Downloads all the users from firebase,
    /// and stores them in the local list of users;
    /// </summary>

    public static async void getAllUsers()
    {
        var users = await firebaseClient
            .Child("Users")
            .OrderByKey()
            .OnceAsync<UserData>();

        foreach (var user in users)
        {
            allUsersList.Add(user.Object);
        }
    }


    /**
     * Uploads the received message object to firebase,
     * into a dedicated "Chat" folder, generating a uniqe timeStamp id for it;
     * @param receivedUserMessage - message object that will be uploaded to Firebase
     */

    public static async void uploadMessage(ChatMessageForUpload receivedUserMessage)
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        string messageID = currentTime.ToString();

        await firebaseClient.Child("Chat")
            .Child(messageID)
            .PutAsync(receivedUserMessage);
    }


    /**
     * Deletes the message from firebase, using
     * the unique timeStamp as an identifier
     * @param receivedDeleteMessage - message object that will be deleted from Firebase
     */

    public static async void deleteMessage(ChatMessage receivedDeleteMessage)
    {
        await firebaseClient
          .Child("Chat")
          .Child(receivedDeleteMessage.timeStamp)
          .DeleteAsync();
    }

    /**
     * Adds the user that wants to register to the firebase database
     * @param handler - socket of the client that sent this request, will be used to send him the response;
     * @param receivedUserRegister - user object, that will be uploaded to firebase
     */

    public static async void addUser(Socket handler, UserData receivedUserRegister)
    {

        addToListOfConnectedDevices(handler);

        await firebaseClient.Child("Users")
            .Child(receivedUserRegister.userName)
            .PutAsync(receivedUserRegister);

        allUsersList.Add(receivedUserRegister);

        Send(handler, "User created!");
    }

    /**
     * Checks, if the user already exists in the database or not, using the local list of users;
     * @param handler - socket of the client that is creating the account, used to send him the response
     * @param receivedUserRegister - user data in an object, that wants to be registered;
     */
    public static void checkIfUserExists(Socket handler, UserData receivedUserRegister)
    {
        bool userExists = false;

        foreach (var currentUser in allUsersList)
        {
            if (currentUser.userName == receivedUserRegister.userName)
            {
                userExists = true;
                Send(handler, "User already exists!");
            }
        }

        if (userExists == false)
        {
            addUser(handler, receivedUserRegister);
        }
    }

    /**
     * Scans the list of all messages, and
     * displays all the usernames with the count of written
     * messages to the console
     * @param messagesList - list of messages to scan
     */

    public static string displayAllUserMessageCount(List<ChatMessage> messagesList)
    {
        int count = 0;
        string allUsersCount = "";

        foreach (var message in messagesList)
        {
            if (!allUsersCount.Contains(message.userName))
            {
                foreach (var current in messagesList)
                {
                    if (current.userName == message.userName)
                    {
                        count++;
                    }
                }

                allUsersCount = allUsersCount + message.userName + ": " + count + " Messages; ";
                count = 0;
            }
        }

        return allUsersCount;
    }

    /**
     * Checks if the user exists in the list;
     * checks if the provided username and password are correct;
     * @param handler - socket of the client that is trying to sign in, used to send him a response;
     * @param receivedUserLogin - the data of the client in an object for checking
     */

    public static void attemptUserLogin(Socket handler, UserLoginData receivedUserLogin)
    {
        bool userExists = false;

        foreach (var currentUser in allUsersList)
        {
            if (currentUser.userName == receivedUserLogin.userName)
            {
                userExists = true;
            }
        }

        if (userExists == true)
        {
            checkUserValidity(handler, receivedUserLogin);
        }
        else
        {
            Send(handler, "User doesn't exist!");
        }
    }

    /**
     * Checks if the user exists in the list;
     * checks if the provided username and password are correct;
     * @param handler - socket of the client that is trying to sign in, used to send him a response;
     * @param receivedUserLogin - the data of the client in an object for checking
     */
    public static void checkUserValidity(Socket handler, UserLoginData receivedUserLogin)
    {
        bool loggedIn = false;
        foreach (var currentUser in allUsersList)
        {
            if (currentUser.userName == receivedUserLogin.userName &&
                currentUser.userPassword == receivedUserLogin.userPassword)
            {
                if (receivedUserLogin.userName == "admin" &&
                   receivedUserLogin.userPassword == "123123")
                {
                    addToListOfConnectedDevices(handler);
                    loggedIn = true;
                    Send(handler, "Admin Login successful!");
                }
                else
                {
                    addToListOfConnectedDevices(handler);
                    loggedIn = true;
                    Send(handler, "Login successful!");
                }


            }
        }

        if (loggedIn == false)
        {
            Send(handler, "Password incorrect!");
        }
    }

}

/// <summary>
/// Data of the user - his username, password and ID
/// </summary>

public class UserData
{
    public string userID { get; set; }
    public string userName { get; set; }
    public string userPassword { get; set; }
}

/// <summary>
/// Data of the user required for login - his username and password
/// </summary>
public class UserLoginData
{
    public string userName { get; set; }
    public string userPassword { get; set; }
}

/// <summary>
/// Message in the chat - 
/// eventname for passing the data;
/// timeStamp - time of it being sent;
/// userName - username of the user sending it;
/// userMessage - message that is being sent;
/// </summary>
public class ChatMessage
{
    public string eventName { get; set; }
    public string timeStamp { get; set; }
    public string userName { get; set; }
    public string userMessage { get; set; }
}

/// <summary>
/// Message in the chat for upload that ignores unncessary fields - 
/// userName - username of the user sending it;
/// userMessage - message that is being sent;
/// </summary>
public class ChatMessageForUpload
{
    public string userName { get; set; }
    public string userMessage { get; set; }
}
 
 
 
 
 
 
 