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
    /// Accepts and receives the data from the clients, in order to process it later;
    /// Calls the ReadCallBack;
    /// </summary>

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

    /// <summary>
    /// Decodes the received data from the client, and acts accordingly to its content;
    /// Identifies the requests and runs the code further
    /// </summary>

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

                uploadMessage(handler, convertedUserMessage);
            }

            if (eventNameReceived.Equals("Delete_Message"))
            {
                ChatMessage receivedDeleteMessage = JsonConvert.DeserializeObject<ChatMessage>(content);

                deleteMessage(handler, receivedDeleteMessage);
            }

            if (eventNameReceived.Equals("Disconnect"))
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }

        }
    }

    /// <summary>
    /// Encodes the string data into ASCII, and sends to the defined client,
    /// using its unique socket
    /// </summary>

    private static void Send(Socket handler, String data)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }

    /// <summary>
    /// Makes sure what amount of bytes was sent, and displays it into the console
    /// </summary>

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

    public static void addToListOfConnectedDevices(Socket handler)
    {
        allConnectedDevices.Add(handler); // Add connected device to list
    }

    /// <summary>
    /// Starts listening to the changes in the firebase database;
    /// Specifically, listens to the directory "Chat", where all
    /// the messages are stored
    /// </summary>

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

    public static async void getCurrentMessageCount(ChatMessage message, string messageTimeStamp)
    {
        var messages = await firebaseClient
            .Child("Chat")
            .OnceAsync<ChatMessage>();

        int currentMessageCount = messages.Count;

        addAndDisplayMessages(message, currentMessageCount, messageTimeStamp);

    }

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

    /// <summary>
    /// Broadcasts the received data to all the users in
    /// the list of currently connected clients
    /// </summary>

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

    /// <summary>
    /// Uploads the received message object to firebase,
    /// into a dedicated "Chat" folder, generating a uniqe timeStamp id for it;
    /// </summary>

    public static async void uploadMessage(Socket handler, ChatMessageForUpload receivedUserMessage)
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        string messageID = currentTime.ToString();

        await firebaseClient.Child("Chat")
            .Child(messageID)
            .PutAsync(receivedUserMessage);
    }

    /// <summary>
    /// Deletes the message from firebase, using
    /// the unique timeStamp as an identifier
    /// </summary>

    public static async void deleteMessage(Socket handler, ChatMessage receivedDeleteMessage)
    {
        await firebaseClient
          .Child("Chat")
          .Child(receivedDeleteMessage.timeStamp)
          .DeleteAsync();
    }

    public static async void addUser(Socket handler, UserData receivedUserRegister)
    {

        addToListOfConnectedDevices(handler);

        await firebaseClient.Child("Users")
            .Child(receivedUserRegister.userName)
            .PutAsync(receivedUserRegister);

        allUsersList.Add(receivedUserRegister);

        Send(handler, "User created!");
    }

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

    /// <summary>
    /// Scans the list of all messages, and
    /// displays all the usernames with the count of written
    /// messages to the console
    /// </summary>

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

    /// <summary>
    /// Checks if the user exists in the list;
    /// checks if the provided username and password
    /// are correct;
    /// </summary>

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
/// All the necessary classes for the program
/// </summary>

public class UserData
{
    public string userID { get; set; }
    public string userName { get; set; }
    public string userPassword { get; set; }
}

public class UserLoginData
{
    public string userName { get; set; }
    public string userPassword { get; set; }
}

public class ChatMessage
{
    public string eventName { get; set; }
    public string timeStamp { get; set; }
    public string userName { get; set; }
    public string userMessage { get; set; }
}

public class ChatMessageForUpload
{
    public string userName { get; set; }
    public string userMessage { get; set; }
}