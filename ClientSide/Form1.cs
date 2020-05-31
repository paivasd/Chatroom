using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientSide
{
    public partial class Form1 : Form
    {
        /*
         * MOVER "A FAZER" PARA FEITO" E GUARDAR
         * 
         * FEITO: GUI DO CLIENTE - AINDA NÃO ESTÁ 100% PRONTO PJ
         *        SERVER SIDE - AINDA NÃO ESTÃ 100% PRONTO PAIVA
         *        
         * A FAZER:
         *        SALAS - temos que discutir a abordagem os dois -> dia 23?
         *        LOGIN - PJ/PAIVA?
         *        BD - PAIVA (SE HOUVER TEMPO)
         *        FTP - ACHO QUE NÃO VAI DAR....
         *        
         *        
         * CALENDÁRIO
         *  ////////////////////////////////////////////////
         * A FAZER 23/05/2020
         * IMPLEMENTAR AS SALAS! QUANTO TEMPO? 2/3 DIAS? =/
         * 
         * A FAZER 25/05/2020
         * LOGIN - ACHO QUE NÃO VAI SER COMPLICADO... 1 DIA?
         *  ////////////////////////////////////////////////
         * 
         * 
         * DISCUTIR:
         * O SERVIDOR É QUE ALOCA AS SALAS?
         * AS SALAS TÊM QUE TER UMA LISTA DE UTILIZADORES CONECTADOS.
         * QUANDO O UTILIZADOR ESCOLHE A SALA, O QUE ACONTECE? CLIENTE INFORMA SERVIDOR E SERVIDOR ADICIONA O _TCPCLIENT ÀQUELA SALA?
         */

        private bool _dragging;
        private Point _offset;

        public static string jsonTest = "";
        private StreamWriter sw;
        private StreamReader sr;
        private TcpClient _tcpClient;
        private User x;
        private Chatroom currentChatRoom;
        private Dictionary<Guid, string> dictionaryChatRoom;
        private Dictionary<Chatroom, Guid> dictionaryChatRoomV2;
        private List<Guid> usersGuid;
        private Message message;
        private Guid messageGuid;
        enum Type
        {
            Text,
            Room,
        };

        /// <summary>
        /// Delegate that will handle the textbox text changes
        /// </summary>
        /// <param name="message">Text to update</param>
        private delegate void UpdateLogCallBack(string message);

        /// <summary>
        /// Delegate that will handle the disconnected state from the thread, update textbox text
        /// </summary>
        /// <param name="aux">Text to update</param>
        private delegate void CloseConnectionCallBack(string aux);

        private Thread thrMessage;
        private IPAddress _ip;
        private bool connected;


        public Form1()
        {
            InitializeComponent();
            dictionaryChatRoom = new Dictionary<Guid, string>();
            dictionaryChatRoomV2 = new Dictionary<Chatroom, Guid>();

            jsonTest = LoginPage.jsonTest;
            IpText.Text = LoginPage.ipText;
            YourNick.Text = LoginPage.userText;




        }


        #region HANDLE WINDOW MOVEMENT
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point currentScreenPos = PointToScreen(e.Location);
                Location = new Point
                    (currentScreenPos.X - _offset.X,
                     currentScreenPos.Y - _offset.Y);
            }
        }



        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _offset.X = e.X;
            _offset.Y = e.Y;
            _dragging = true;
        }



        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }
        #endregion


        private void Start()
        {
            try
            {
                //Attempts the connection to the server
                 _ip = IPAddress.Parse(IpText.Text);
                _tcpClient = new TcpClient();
                _tcpClient.Connect(_ip, 9000);

                connected = true;

                //Unlock controls
                connectButton.Text = "Disconnected";
                listBox1.Enabled = true;
                button1.Enabled = true;


                //Open a StreamWriter 
                sw = new StreamWriter(_tcpClient.GetStream());

                //So we can write to the server
                sw.WriteLine(jsonTest);
               
                //Clear buffers
                sw.Flush();

                //Initialize the thread
                thrMessage = new Thread(new ThreadStart(Receive));
                thrMessage.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Method that handles the upcoming responses from the Server
        /// </summary>
        private void Receive()
        {
           
            //Open a StreamReader to be able to fetch Server responses
            sr = new StreamReader(_tcpClient.GetStream());

            //Fetch the server response
            string connectionResponse = sr.ReadLine();

            //Flag we use on successful connection
            if (connectionResponse[0] == '1')
            {
                //Update textbox thru the delegate
                this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { "You successfully connected to the server. Please choose a room." });

                while (connected)
                {   
                    try
                    {
                        //Reading line from StreamReader
                        string response = sr.ReadLine();
                        if (response != "")
                        {
                            //If response string is not empty, we deserialize the response into a Message object
                            message = JsonConvert.DeserializeObject<Message>(response);

                            //Depending on the Type of the Message object
                            if (message.MessageType == Message.Type.Text)
                            {
                                this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { message.UserName + " said: " + message.MessageBody });
                            }
                            if (message.MessageType == Message.Type.Server)
                            {
                                this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { "Server said: " + message.MessageBody });
                            }
                            if (message.MessageType == Message.Type.Room) //If Type is Room, we populate our chatroom dictionary with the info
                            {
                                dictionaryChatRoom = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(message.MessageBody);
                                foreach (KeyValuePair<Guid, string> item in dictionaryChatRoom)
                                {
                                    Chatroom auxRoom = new Chatroom();
                                    auxRoom.Identifier = item.Key;
                                    auxRoom.ChatName = item.Value;
                                    dictionaryChatRoomV2.Add(auxRoom, auxRoom.Identifier);
                                }
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        string aux = "Disconnected";
                        this.Invoke(new CloseConnectionCallBack(this.CloseConnection), new object[] { aux });
                        return;
                    }
                    
                }

            }
            else //If the flag is not 1, it means the connection failed/User disconnected
            {
                string aux = "Disconnected";

                //Update textbox thru the delegate
                this.Invoke(new CloseConnectionCallBack(this.CloseConnection), new object[] { aux });
                return;
            }
            
        }

        //Method for the delegate
        private void LogUpdate(string message)
        {
            
            chatBox.AppendText(message + "\r\n");
        }


       //Method to send textbox message to server
        private void SendMessage()
        {
            if (messageBox.Lines.Length >= 1)
            {
                // MESSAGE OBJECT //
                message = new Message();
                message.MessageBody = messageBox.Text;
                message.MessageType = Message.Type.Text;
                message.chatGuid = messageGuid;
                message.userGuid = x.GlobalIdentifier;
                message.UserName = x.Username;
                string json;
                json = JsonConvert.SerializeObject(message);

                //sw.WriteLine(messageBox.Text);
                sw.WriteLine(json);
                sw.Flush();
                messageBox.Lines = null;
            }
            messageBox.Text = "";
        }

        //Close the connection to the server.
        private void CloseConnection(string message)
        {
            chatBox.AppendText(message + "\r\n");


            messageBox.Enabled = false;
            sendButton.Enabled = false;
            connectButton.Text = "Connect";

            connected = false;
            sw.Close();
            sr.Close();
            _tcpClient.Close();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            //If not connected, starts the connection method
            if (connected == false)
            {
                Start();
            }
            else //If connected, disconnects current connection
            {
                CloseConnection("Disconnected");
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            messageGuid = AssignMessageId();
            SendJoinMessage();
            messageBox.Enabled = true;
            sendButton.Enabled = true;
            chatBox.Enabled = true;


        }
        
        //Method used to create a Message object to inform the Server we joined a chatroom.
        private void SendJoinMessage()
        {
                // MESSAGE OBJECT //
                message = new Message();
                message.MessageBody = null;
                message.MessageType = Message.Type.Join;
                message.chatGuid = messageGuid;
                message.userGuid = x.GlobalIdentifier;
                string json;
                json = JsonConvert.SerializeObject(message);

                //sw.WriteLine(messageBox.Text);
                sw.WriteLine(json);
                sw.Flush();
        }
        
        //Method used to assign a message guid to match the chatroom guid that we want to chat to.
        //Also adds the current user Guid to the chatroom user dictionary.
        private Guid AssignMessageId()
        {
            string curItem = listBox1.SelectedItem.ToString();

            foreach (KeyValuePair<Chatroom, Guid> item in dictionaryChatRoomV2)
            {
                if (item.Key.ChatName == curItem)
                {
                    item.Key.usersGuid.Add(x.GlobalIdentifier); //not implemented
                    //was to show a list of users in the chatroom clientside (????)

                    //button1.Enabled = false;

                    messageGuid = item.Key.Identifier;

                    break;

                }
            }
            if(messageGuid == Guid.Empty)
            {
                MessageBox.Show("Please choose a room");
            }
            return messageGuid;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            jsonTest = LoginPage.jsonTest;
            x = JsonConvert.DeserializeObject<User>(jsonTest);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
