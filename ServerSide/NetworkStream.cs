using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace ServerSide
{

    // handle args to status change
    public class StatusChangedEventArgs : EventArgs
    {
        private string EventMsg;
        public string EventMessage
        {
            get { return EventMsg; }
            set { EventMsg = value; }
        }
        public StatusChangedEventArgs(string strEventMsg)
        {
            EventMsg = strEventMsg;
        }
    }

    // delegate to handle status change
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);

    class NetworkStream
    {
        /// <summary>
        /// Dictionaries
        /// </summary>
        /// 

        //Dictionary to store User username as Key and User password as Value
        public static Dictionary<string, string> dictionaryUserPasswordPlain = new Dictionary<string, string>();

        //Dictionary to store User username as Key and User object as Value
        public static Dictionary<string, User> dictionaryUserNameUser = new Dictionary<string, User>();

        //Dictionary to store User object as Key and User object GUID as Value
        public static Dictionary<User, Guid> dictionaryConnections = new Dictionary<User, Guid>();

        //Dictionary to store User object GUID as Key and User object as Value
        public static Dictionary<Guid, User> dictionaryUsers = new Dictionary<Guid, User>();

        //Dictionary to store Chatroom object GUID as Key and Chatroom object as Value
        public static Dictionary<Guid, Chatroom> dictionaryChatRoomV3 = new Dictionary<Guid, Chatroom>();

        //Dictionary to store Chatroom object GUID as Key and Chatroom object name as Value
        public static Dictionary<Guid, string> dictionaryChatRoomV2 = new Dictionary<Guid, string>();


        private IPAddress _ip;
        private TcpClient _tcpClient;

        private Thread thrListener;
        private TcpListener _tcpListener;

        //event arg handler
        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;
        bool serverUp = false;

        /// <summary>
        /// Class default constructor
        /// </summary>
        /// <param name="ip"></param>
        public NetworkStream(IPAddress ip)
        {
            _ip = ip;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            try
            {
                //Initialization of the Chatroom objects
                Chatroom chat1 = new Chatroom("LESI"); 
                Chatroom chat2 = new Chatroom("EDJD");
                
                //User initialization for testing purposes
                User baseUser = new User();
                baseUser.Username = "paiva";
                baseUser.Password = "123";
                baseUser.GlobalIdentifier = Guid.NewGuid();
                baseUser.Registered = true;
                baseUser.UserType = User.Type.Student;

                dictionaryUserPasswordPlain.Add(baseUser.Username, baseUser.Password);
                dictionaryUsers.Add(baseUser.GlobalIdentifier, baseUser);
                dictionaryUserNameUser.Add(baseUser.Username, baseUser);



                dictionaryChatRoomV2.Add(chat1.Identifier, chat1.ChatName);
                dictionaryChatRoomV2.Add(chat2.Identifier, chat2.ChatName);


                dictionaryChatRoomV3.Add(chat1.Identifier, chat1);
                dictionaryChatRoomV3.Add(chat2.Identifier, chat2);


                //Start the TcpListener for our server to be able to listen to any upcoming connection
                IPAddress ipaLocal = _ip;
                _tcpListener = new TcpListener(IPAddress.Any, 9000);
                _tcpListener.Start();


                serverUp = true;

                // listening thread
                thrListener = new Thread(ActiveListening);
                thrListener.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ActiveListening()
        {
            while (serverUp == true)
            {
                //Accept the tcpclient connection
                _tcpClient = _tcpListener.AcceptTcpClient();

                //instantiate the Connection class
                Connection newConnection = new Connection(_tcpClient);
            }
        }

        /// <summary>
        /// Method used to add User to a specific dicionary, also provides a server message warning all users that this one
        /// joined the server.
        /// </summary>
        /// <param name="currentUser"></param>
        public static void AddUserDictionary(User currentUser)
        {
            var shallow = dictionaryConnections.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (shallow.Count == 0)
            {
                dictionaryConnections.Add(currentUser, currentUser.GlobalIdentifier);
            }
            else 
            {
                foreach (KeyValuePair<User, Guid> item in shallow)
                {
                    if (!dictionaryConnections.ContainsValue(currentUser.GlobalIdentifier))
                    {
                        dictionaryConnections.Add(currentUser, currentUser.GlobalIdentifier);
                    }
                }
            }
            
            

            Message currentMessage = new Message();
            currentMessage.MessageBody = currentUser.Username + " connected to the server.";
            currentMessage.MessageType = Message.Type.Server;

     
            SendServerMessagesObject(currentMessage);

        }


        /// <summary>
        /// Method used to handle status changes on the thread
        /// </summary>
        /// <param name="e"></param>
        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                //Invoke handler
                statusHandler(null, e);
            }
        }

        /// <summary>
        /// Method used to send Chatroom dictionary to every clients that connects to the server
        /// </summary>
        /// <param name="currentUser">User object</param>
        public static void SendChatRoomForConnectedUser(User currentUser)
        {
            StreamWriter sw;
            StatusChangedEventArgs e = new StatusChangedEventArgs("Server: Sent ChatRooms");
            OnStatusChanged(e);
            try
            {
                if (dictionaryChatRoomV2.Count == 0)
                {
                    return;
                }
                string json = JsonConvert.SerializeObject(dictionaryChatRoomV2);
                Message chatrooms = new Message();
                chatrooms.MessageBody = json;
                chatrooms.MessageType = Message.Type.Room;

                string _json = JsonConvert.SerializeObject(chatrooms);

                sw = new StreamWriter(currentUser.UserTcp.GetStream());
                sw.WriteLine(_json);
                sw.Flush();
                sw = null;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Method used to send messages across all users as Server
        /// </summary>
        /// <param name="currentMessage">Message object</param>
        public static void SendServerMessagesObject(Message currentMessage)
        {
            StreamWriter sw;

            StatusChangedEventArgs e = new StatusChangedEventArgs("Server: " + currentMessage.MessageBody);
            OnStatusChanged(e);

            User[] users = new User[dictionaryUsers.Count];
            dictionaryUsers.Values.CopyTo(users, 0);

            string json = JsonConvert.SerializeObject(currentMessage);

            for (int i = 0; i < users.Length; i++)
            {
                try
                {
                    if (currentMessage.MessageBody.Trim() == "" || users[i] == null)
                    {
                        continue;
                    }

                    sw = new StreamWriter(users[i].UserTcp.GetStream());
                    sw.WriteLine(json);
                    sw.Flush();
                    sw = null;
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Method used to send messages across all users on the same Chatroom as User
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="currentMessage"></param>
        public static void SendUserMessagesObject(string userName, Message currentMessage)
        {
            StreamWriter sw;
            Chatroom aux;
            dictionaryChatRoomV3.TryGetValue(currentMessage.chatGuid, out aux);

            e = new StatusChangedEventArgs(userName + " said: " + currentMessage.MessageBody.ToString() + " to " + aux.ChatName + " chatroom.");
            OnStatusChanged(e);

            string json;
            json = JsonConvert.SerializeObject(currentMessage);

            User[] users = new User[aux.usersDictionary.Count];
            aux.usersDictionary.Values.CopyTo(users, 0);

            for (int i = 0; i < users.Length; i++)
            {
                try
                {
                    if (currentMessage.MessageBody.Trim() == "" || users[i].UserTcp == null)
                    {
                        continue;
                    }
                    sw = new StreamWriter(users[i].UserTcp.GetStream());
                    sw.WriteLine(json);
                    sw.Flush();
                    sw = null;
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Method used to assign User object message to a specific chatroom and to add him to the Users dictionary inside the Chatroom.
        /// </summary>
        /// <param name="currentUser">User object</param>
        /// <param name="currentMessage">Message object</param>
        public static void AssignUserToChat(User currentUser, Message currentMessage)
        {
            currentUser.CurrentChat = currentMessage.chatGuid;
            foreach (KeyValuePair<Guid, Chatroom> item in dictionaryChatRoomV3)
            {
                if (item.Key == currentMessage.chatGuid)
                {
                    if (!item.Value.usersDictionary.ContainsKey(currentUser.GlobalIdentifier))
                    {

                        item.Value.usersDictionary.Add(currentUser.GlobalIdentifier, currentUser);
                    }
                }
                else
                {
                    item.Value.usersDictionary.Remove(currentUser.GlobalIdentifier);
                }
            }
        }

        /// <summary>
        /// Method used to add the User object to different dicionaries.
        /// </summary>
        /// <param name="currentUser"></param>
        public static void AddUserToUserDictionary(User currentUser)
        {
            var shallow = dictionaryUsers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (dictionaryUsers.Count == 0)
            {
                currentUser.Registered = true;
                dictionaryUsers.Add(currentUser.GlobalIdentifier, currentUser);
                dictionaryUserNameUser.Add(currentUser.Username, currentUser);

                dictionaryUserPasswordPlain.Add(currentUser.Username, currentUser.Password);
                
            }
            else
            {
                foreach (KeyValuePair<Guid, User> item in shallow) //criar dicionário
                                                                           // temporário
                {
                    if (!dictionaryUsers.ContainsKey(currentUser.GlobalIdentifier))
                    {
                        currentUser.Registered = true;
                        dictionaryUsers.Add(currentUser.GlobalIdentifier, currentUser);
                        dictionaryUserNameUser.Add(currentUser.Username, currentUser);
                        dictionaryUserPasswordPlain.Add(currentUser.Username, currentUser.Password);
                    }
                }
            }
        }

        /// <summary>
        /// Method used to retrive full User object from dictionary
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static User RetrieveUserFromDictionary(User currentUser)
        {
            foreach (KeyValuePair<string, string> item in dictionaryUserPasswordPlain)
            {
                if (item.Key == currentUser.Username && item.Value == currentUser.Password)
                {
                    currentUser = dictionaryUserNameUser[currentUser.Username];
                }
                
            }
            return currentUser;
        }

        /// <summary>
        /// Method used to send to client the User object full info
        /// </summary>
        /// <param name="currentUser"></param>
        public static void SendUserInfo(User currentUser)
        {
            StreamWriter sw;
            StatusChangedEventArgs e = new StatusChangedEventArgs("Server: Sent info");
            OnStatusChanged(e);
            try
            {
                TcpClient aux;
                aux = currentUser.UserTcp;
                currentUser.UserTcp = null;
                string json = JsonConvert.SerializeObject(currentUser);
                Message message = new Message();
                message.MessageBody = json;
                message.MessageType = Message.Type.SuccessfulLogin;
                string _json = JsonConvert.SerializeObject(message);

                sw = new StreamWriter(aux.GetStream());
                sw.WriteLine(_json);
                sw.Flush();
                sw = null;
            }
            catch
            {

            }

        }

        class Connection
        {
            TcpClient _tcpClient;
            string json;
            User currentUser;
            Chatroom currentChat;
            Message currentMessage;


            private Thread thrSender;
            private StreamReader sr;
            private StreamWriter sw;

            private string strAnswer;

            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="tcpClient">TcpClient object</param>
            public Connection(TcpClient tcpClient)
            {
                _tcpClient = tcpClient;
                thrSender = new Thread(AcceptClient);
                thrSender.Start();
            }

            private void CloseConnection(User currentUser)
            {
                StatusChangedEventArgs e = new StatusChangedEventArgs("Server: " + currentUser.Username+ " disconnected");
                OnStatusChanged(e);
                _tcpClient.Close();
                sr.Close();
                sw.Close();
            }


            /// <summary>
            /// Method used to handle communication between server/client
            /// </summary>
            private void AcceptClient()
            {
                sr = new StreamReader(_tcpClient.GetStream());
                sw = new StreamWriter(_tcpClient.GetStream());

                //Receive info from StreamReader, probably the User object containing user+pass
                json = sr.ReadLine(); 

                currentUser = new User();
                currentUser = JsonConvert.DeserializeObject<User>(json); 
                currentUser.UserTcp = _tcpClient;                       

                //Verify if user is already registered or not
                if (currentUser.Username != "")
                {
                    if(currentUser.Registered == true)
                    {
                        //Search in the dictionary if this user+pass already exists
                        foreach (KeyValuePair<string, string> item in dictionaryUserPasswordPlain)
                        {
                            if (currentUser.Username == item.Key && currentUser.Password == item.Value) //pq o GlobalIdentifier ainda é zero
                            {
                                //Tell the client the connection was sucessfull
                                sw.WriteLine("1");
                                sw.Flush();

                                //Retrive full info from User object stored in the dictionary (Because of the GUID!)
                                currentUser = NetworkStream.RetrieveUserFromDictionary(currentUser);
                                currentUser.UserTcp = _tcpClient;

                                //Send to the user his full info
                                NetworkStream.SendUserInfo(currentUser);
                                sw.Flush();
                                currentUser.UserTcp = _tcpClient;

                                //Send to the user the info from Chatroom objects (so each of them have the same GUID for everyone)
                                NetworkStream.SendChatRoomForConnectedUser(currentUser);

                                //Add user to the connections dictionary.
                                NetworkStream.AddUserDictionary(currentUser);

                            }
                        }
                    }
                    else if(currentUser.Registered == false)
                    {
                        //If not, still tell the client the connection was succesfull
                        sw.WriteLine("1");
                        sw.Flush();
                        //Add the user to the dictionaries
                        NetworkStream.AddUserToUserDictionary(currentUser);
                        //Retrive the user the full info about his object
                        NetworkStream.SendUserInfo(currentUser);

                        currentUser.UserTcp = _tcpClient;
                        sw.Flush();

                        //Send to the user the info from Chatroom objects (so each of them have the same GUID for everyone)
                        NetworkStream.SendChatRoomForConnectedUser(currentUser);

                        //Add user to the connections dictionary.
                        NetworkStream.AddUserDictionary(currentUser);

                    }
                }
                else
                {
                    CloseConnection(currentUser);
                    return;
                }
                
                try
                {   //While the answer from client is not empty
                    while ((strAnswer = sr.ReadLine()) != "")
                    {
                        //if it's null, break and close the connection down the while loop
                        if (strAnswer == null)
                        {
                            break;
                        }
                        else
                        {
                            //Deserialize the string to a Message object and check his Message.Type for different outcomes
                            currentMessage = JsonConvert.DeserializeObject<Message>(strAnswer);
                            if (currentMessage.MessageType == Message.Type.Text)
                            {
                                NetworkStream.SendUserMessagesObject(currentUser.Username, currentMessage);
                            }
                            if (currentMessage.MessageType == Message.Type.Join)
                            {
                                NetworkStream.AssignUserToChat(currentUser, currentMessage);
                            }
                            if (currentMessage.MessageType == Message.Type.Register)
                            {
                                NetworkStream.AddUserToUserDictionary(currentUser);
                            }
                            if (currentMessage.MessageType == Message.Type.SuccessfulLogin)
                            {
                                User aux = new User();
                                aux = NetworkStream.RetrieveUserFromDictionary(currentUser);
                                NetworkStream.SendUserInfo(currentUser);
                            }

                        }
                    }
                    CloseConnection(currentUser);
                }
                catch(Exception e)
                {
                    
                }
                
            }
        }
    }
}
