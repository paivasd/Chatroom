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
        //Dictionary
        public static Dictionary<Guid, User> dictionaryUsers = new Dictionary<Guid, User>();
        public static Dictionary<User, Guid> dictionaryConnections = new Dictionary<User, Guid>();
        public static Dictionary<Chatroom, Guid> dictionaryChatRoom = new Dictionary<Chatroom, Guid>();
        public static Dictionary<Guid, Chatroom> dictionaryChatRoomV3 = new Dictionary<Guid, Chatroom>();
        public static Dictionary<Guid, string> dictionaryChatRoomV2 = new Dictionary<Guid, string>();

        //  public static Dictionary<int, List<User>> dictionaryUsers = new Dictionary<int, List<User>>();

        private IPAddress _ip;
        private TcpClient _tcpClient;

        private Thread thrListener;
        private TcpListener _tcpListener;
        private Message currentMessage;

        //event arg handler
        public static event StatusChangedEventHandler StatusChanged;
        private static StatusChangedEventArgs e;
        bool serverUp = false;

        //constructor
        public NetworkStream(IPAddress ip)
        {
            _ip = ip;
        }


        public void Start()
        {
            try
            {
                Chatroom chat1 = new Chatroom("LESI"); // VERIFICAR ESTA ABORDAGEM, NÃO PARECE CORRETA.................
                Chatroom chat2 = new Chatroom("EDJD");
                dictionaryChatRoomV2.Add(chat1.Identifier, chat1.ChatName);
                dictionaryChatRoomV2.Add(chat2.Identifier, chat2.ChatName);

                dictionaryChatRoom.Add(chat1, chat1.Identifier);
                dictionaryChatRoom.Add(chat2, chat2.Identifier);


                dictionaryChatRoomV3.Add(chat1.Identifier, chat1);
                dictionaryChatRoomV3.Add(chat2.Identifier, chat2);


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
                _tcpClient = _tcpListener.AcceptTcpClient();
                Connection newConnection = new Connection(_tcpClient);
            }
        }


        public static void AddUserDictionary(User currentUser)
        {

            //DICTIONARIES
            dictionaryUsers.Add(currentUser.GlobalIdentifier, currentUser);
            dictionaryConnections.Add(currentUser, currentUser.GlobalIdentifier);

            Message currentMessage = new Message();
            currentMessage.MessageBody = dictionaryUsers[currentUser.GlobalIdentifier] + " connected to server.";
            currentMessage.MessageType = Message.Type.Server;

            //SendServerMessages(dictionaryUsers[currentUser.GlobalIdentifier] + " connected to server");
            SendServerMessagesObject(currentMessage); // TÁ CERTO
                                                                                                             // SÓ FALTA ARRANJAR

        }

        public static void DeleteUser(User currentUser)
        {
            if (dictionaryConnections[currentUser] != null)
            {
                SendServerMessages(dictionaryConnections[currentUser] + " left server.");

                dictionaryUsers.Remove(currentUser.GlobalIdentifier);
                dictionaryConnections.Remove(currentUser);
            }
        }

        public static void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChangedEventHandler statusHandler = StatusChanged;
            if (statusHandler != null)
            {
                // invoke delegate
                statusHandler(null, e);
            }
        }


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


        public static void SendServerMessages(string message)
        {
            StreamWriter sw;

            StatusChangedEventArgs e = new StatusChangedEventArgs("Server: " + message);
            OnStatusChanged(e);

            User[] users = new User[dictionaryUsers.Count];
            dictionaryUsers.Values.CopyTo(users, 0);


            for (int i = 0; i < users.Length; i++)
            {
                try
                {
                    if (message.Trim() == "" || users[i] == null)
                    {
                        continue;
                    }

                    sw = new StreamWriter(users[i].UserTcp.GetStream());
                    sw.WriteLine("Server: " + message);
                    sw.Flush();
                    sw = null;
                }
                catch
                {
                    DeleteUser(users[i]);
                }
            }
        }


        // MENSAGENS OBJECTO //
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
                    //sw.WriteLine("Server: " + currentMessage.MessageBody);
                    sw.WriteLine(json);
                    sw.Flush();
                    sw = null;
                }
                catch
                {
                    DeleteUser(users[i]);
                }
            }
        }

        ////VERIFICAR SE NÃO PODEMOS REFATORAR ESTE MÉTODO E O DE CIMA////////////////
        public static void SendUserMessages(string userName, string message)
        {
            StreamWriter sw;

            e = new StatusChangedEventArgs(userName + " said: " + message);
            OnStatusChanged(e);


            User[] users = new User[dictionaryUsers.Count];

            dictionaryUsers.Values.CopyTo(users, 0);

            for (int i = 0; i < users.Length; i++)
            {
                try
                {
                    if (message.Trim() == "" || users[i].UserTcp == null)
                    {
                        continue;
                    }
                    sw = new StreamWriter(users[i].UserTcp.GetStream());
                    sw.WriteLine(userName + " said: " + message);
                    sw.Flush();
                    sw = null;
                }
                catch
                {
                    DeleteUser(users[i]);
                }
            }
        }

        // MENSAGENS OBJECTO //

        public static void SendUserMessagesObject(string userName, Message currentMessage)
        {
            StreamWriter sw;

            e = new StatusChangedEventArgs(userName + " said: " + currentMessage.MessageBody.ToString());
            OnStatusChanged(e);

            string json;
            json = JsonConvert.SerializeObject(currentMessage);

            User[] users = new User[dictionaryUsers.Count];

            dictionaryUsers.Values.CopyTo(users, 0);

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
                    DeleteUser(users[i]);
                }
            }
        }

        public static void AddUserToChatRoom(Message currentMessage)
        {
            //Dictionary<Chatroom, Guid> temp;
            //temp = JsonConvert.DeserializeObject<Dictionary<Chatroom, Guid>>(currentMessage.MessageBody);

            //foreach (KeyValuePair<Chatroom,Guid> item in temp)
            //{
            //    foreach(KeyValuePair<Chatroom, Guid> item2 in dictionaryChatRoom)
            //    {
            //        if(item.Value == item2.Value)
            //        {
            //            item.Key.usersGuid.Contains(item2.Key.usersGuid.Any());
            //        }
            //    }


            //}
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

        public Connection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            thrSender = new Thread(AcceptClient);
            thrSender.Start();
        }

        private void CloseConnection()
        {
            _tcpClient.Close();
            sr.Close();
            sw.Close();
        }

        //private void SendChatRoomAvailable(Chatroom chatroom)
        //{
        //    StreamWriter sw;

        //    string json = JsonConvert.SerializeObject(chatroom);
        //    sw = new StreamWriter(currentUser.UserTcp.GetStream());
        //    sw.WriteLine(json);
        //}

        private void AcceptClient()
        {
            sr = new StreamReader(_tcpClient.GetStream());
            sw = new StreamWriter(_tcpClient.GetStream());

                                  // PJ: A FAZER //
            json = sr.ReadLine(); //RECEBER OBJETO USER SERIALIZADO
                                  // FAZER DESERIALIZAÇÃO
                                  // CRIAR NOVO OBJETO USER
                                  // FAZER CENAS
            currentUser = new User();
            currentUser = JsonConvert.DeserializeObject<User>(json); // PAIVA: A FAZER //
            currentUser.UserTcp = _tcpClient;                       // VERIFICAR EXISTENCIA NO DICIONARIO //
                                                                    // SE EXISTIR, INFORMAR E FECHAR LIGAÇÃO
            if (currentUser.Username != "")
            {
                foreach (KeyValuePair<Guid, User> entry in NetworkStream.dictionaryUsers)
                {
                    if (currentUser.Username == entry.Value.Username)
                    {
                        sw.WriteLine("This user already exists.");
                        sw.Flush();
                        CloseConnection();
                        return;
                        
                    }

                }


                //chatroom
                sw.WriteLine("1");
                sw.Flush();

                sw.WriteLine(currentChat);
                sw.Flush();

                NetworkStream.AddUserDictionary(currentUser); 
                                                                        // PAIVA: A FAZER //
                                                                        // IMPLEMENTAR AS SALAS //
                                                                        // ENVIAR DO SERVIDOR O GUID DAS SALAS E O CLIENT //
                                                                        // DESERIALIZA E CONSTROI OS OBJETOS? DEVOLVE DPS LISTA COM OS USERS CONNECTADOS LA??????? //

                NetworkStream.SendChatRoomForConnectedUser(currentUser);

            }
            else
            {
                CloseConnection();
                return;
            }
            //
            try
            {               // USER MESSAGE //
                while ((strAnswer = sr.ReadLine()) != "")
                {
                    if (strAnswer == null)
                    {
                        NetworkStream.DeleteUser(currentUser);
                    }
                    else
                    {

                        /// MENSAGEM OBJETO //

                        currentMessage = JsonConvert.DeserializeObject<Message>(strAnswer);
                        if(currentMessage.MessageType == Message.Type.Text)
                        {
                            NetworkStream.SendUserMessagesObject(currentUser.Username, currentMessage);
                        }
                        if(currentMessage.MessageType == Message.Type.Room)
                        {
                           
                            NetworkStream.AddUserToChatRoom(currentMessage);
                        }


                        // broadcast
                        //NetworkStream.SendUserMessages(currentUser.Username, strAnswer);
                       
                    }
                }
            }
            catch
            {
                NetworkStream.DeleteUser(currentUser);
            }
        }
    }
}
