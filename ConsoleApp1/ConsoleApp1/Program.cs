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

    public static List<UserRegisterData> allUsersList = new List<UserRegisterData>();

    public static List<ChatMessage> allMessagesList = new List<ChatMessage>();

    public static bool firstCheck = true;

    public static int initialMessageCount;
    public static int repeatCount;

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
            if (eventNameReceived.Equals("Register")) {

                UserRegisterData receivedUserRegister = JsonConvert.DeserializeObject<UserRegisterData>(content);

                checkIfUserExists(handler, receivedUserRegister);
            }

            if (eventNameReceived.Equals("Login"))
            {
                UserLoginData receivedUserLogin = JsonConvert.DeserializeObject<UserLoginData>(content);

                attemptUserLogin(handler, receivedUserLogin);
            }
        }
    }

    private static void Send(Socket handler, String data)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket handler = (Socket)ar.AsyncState;

            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

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

    public static async void SubscribeToMessages()
    {
        var messages = await firebaseClient
            .Child("Chat")
            .OnceAsync<ChatMessage>();

        initialMessageCount = messages.Count;

        Console.WriteLine(initialMessageCount + " Message Count");

        repeatCount = 0;

        var observable = firebaseClient
              .Child("Chat")
              .AsObservable<ChatMessage>()
              .Subscribe(d => addAndDisplayMessages(d.Object));

        Console.WriteLine("TRYNA GET");
    }

    public static void addAndDisplayMessages(ChatMessage d)
    {
        if (firstCheck != true)
        {
            initialMessageCount++;   
        }

        allMessagesList.Add(d);

        if (allMessagesList.Count == initialMessageCount)
        {
            foreach (var message in allMessagesList)
            {
                Console.WriteLine(message.userMessage);
                Console.WriteLine("MESSAGE ABOVE");
                Console.WriteLine("------------------------");
            }

            firstCheck = false;
        }
    }

    public static async void getAllUsers() {

        var users = await firebaseClient
            .Child("Users")
            .OrderByKey()
            .OnceAsync<UserRegisterData>();

        foreach (var user in users)
        {
            allUsersList.Add(user.Object);
        }
    }

    public static async void addUser(Socket handler, UserRegisterData receivedUserRegister)
    {
            await firebaseClient.Child("Users")
                .Child(receivedUserRegister.userName)
                .PutAsync(receivedUserRegister);

            allUsersList.Add(receivedUserRegister);

            Send(handler, "User created!");
    }

    public static void checkIfUserExists(Socket handler, UserRegisterData receivedUserRegister)
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
    }

    public static void checkUserValidity(Socket handler, UserLoginData receivedUserLogin)
    {
        bool loggedIn = false;
        foreach (var currentUser in allUsersList)
        {
            if (currentUser.userName == receivedUserLogin.userName &&
                currentUser.userPassword == receivedUserLogin.userPassword)
            {
                loggedIn = true;
                Send(handler, "Login successful!");
            }
        }

        if (loggedIn == false)
        {
            Send(handler, "Data incorrect!");
        }
    }

}
public class UserRegisterData
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
    public string userName { get; set; }
    public string userMessage { get; set; }
}