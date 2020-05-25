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

        // Will handle textbox updates
        private delegate void UpdateLogCallBack(string message);

        // 
        // will handle disconnected state from thread
        private delegate void CloseConnectionCallBack(string aux);

        private Thread thrMessage;
        private IPAddress _ip;
        private bool connected;

        public Form1()
        {
            InitializeComponent();
            dictionaryChatRoom = new Dictionary<Guid, string>();
            dictionaryChatRoomV2 = new Dictionary<Chatroom, Guid>();
           

            

        }

        private void Start()
        {
            try
            {
                
                 _ip = IPAddress.Parse(ipBox.Text);
                _tcpClient = new TcpClient();
                _tcpClient.Connect(_ip, 9000);

                connected = true;

                x = new User();
                x.Username = nickBox.Text;
                x.GlobalIdentifier = Guid.NewGuid();
                

                ipBox.Enabled = false;
                nickBox.Enabled = false;
                //messageBox.Enabled = true;
                //sendButton.Enabled = true;
                connectButton.Text = "Disconnected";

                
                sw = new StreamWriter(_tcpClient.GetStream());
                string json = JsonConvert.SerializeObject(x);
                //string json2 = JsonConvert.SerializeObject(dictionaryChatRoom);
                sw.WriteLine(json); // // // // // // // // // // // dá para passar objetos tbm
                //sw.WriteLine(json2);
                //sw.WriteLine(json2);
                sw.Flush();

                //Initialize thread
                thrMessage = new Thread(new ThreadStart(Receive));
                thrMessage.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Receive()
        {
           
            sr = new StreamReader(_tcpClient.GetStream());
            string connectionResponse = sr.ReadLine();
            
            if (connectionResponse[0] == '1')
            {
                // update to inform we connect
                this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { "You successfully connected to the server" });
            }
            else // if the first char is not 1, it means the connection failed
            {
                string aux = "Not connected:  ";
                // why? answer starts in the 3rd character
                aux += connectionResponse.Substring(2, connectionResponse.Length - 2);

                // Aupdate textbox
                this.Invoke(new CloseConnectionCallBack(this.CloseConnection), new object[] { aux });
                return;
            }

            
            while (connected)
            {
                string response = sr.ReadLine();
                if (response != "")
                {
                    message = JsonConvert.DeserializeObject<Message>(response);
                    if (message.MessageType == Message.Type.Text)
                    {
                        this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { message.UserName +" said: " + message.MessageBody });
                    }
                    if(message.MessageType == Message.Type.Server)
                    {
                        this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { "Server said: " + message.MessageBody });
                    }
                    if(message.MessageType == Message.Type.Room)
                    {
                        dictionaryChatRoom = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(message.MessageBody);
                        foreach(KeyValuePair<Guid, string> item in dictionaryChatRoom)
                        {
                            Chatroom auxRoom = new Chatroom();
                            auxRoom.Identifier = item.Key;
                            auxRoom.ChatName = item.Value;
                            dictionaryChatRoomV2.Add(auxRoom, auxRoom.Identifier);
                        }
                    }
                }
                

                //Message message = new Message();
                //try
                //{
                //    message = (Message)JsonConvert.DeserializeObject(response);
                //    try
                //    {
                //        if ((int)message.type == 1)
                //        {
                //            dictionaryChatRoom = (Dictionary<Guid, Chatroom>)JsonConvert.DeserializeObject(response);
                //            //currentChat = (Chatroom)JsonConvert.DeserializeObject(response);
                //        }
                //        else
                //        {
                //            return;
                //        }
                //    }
                //    catch (Exception ex)
                //    {

                //        MessageBox.Show("Error : " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    }
                //}
                //catch (Exception ex)
                //{

                //    MessageBox.Show("Error : " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

               //this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { response });

                //try
                //{
                //    currentChatRoom = (Chatroom)JsonConvert.DeserializeObject(response);
                //    MessageBox.Show(currentChatRoom.ChatName + currentChatRoom.Identifier);
                //}
                //catch (Exception ex)
                //{

                //    MessageBox.Show("Error : " + ex.Message, "Failed Conversion JSON", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                //// exibe mensagems no Textbox
                //this.Invoke(new UpdateLogCallBack(this.AtualizaLog), new object[] { response });
            }
        }

        private void LogUpdate(string message)
        {
            
            chatBox.AppendText(message + "\r\n");
        }

        private void messageBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                SendMessage();
            }
        }

       //method to send msg to server
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

        //close connection to serv
        private void CloseConnection(string diff)
        {
            // Mostra o motivo porque a conexão encerrou
            chatBox.AppendText(diff + "\r\n");
          

            ipBox.Enabled = true;
            nickBox.Enabled = true;
            messageBox.Enabled = false;
            sendButton.Enabled = false;
            connectButton.Text = "Connected";

            connected = false;
            sw.Close();
            sr.Close();
            _tcpClient.Close();
        }

        
        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (connected == true)
            {
                //close everything
                connected = false;
                sw.Close();
                sr.Close();
                _tcpClient.Close();
            }
        }

        private void frmCliente_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if not connected, starts the connection method
            //TESTE GIT
            if (connected == false)
            {
                Start();
            }
            else //if connected, disconnects current connection
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
            //string curItem = listBox1.SelectedItem.ToString();

            //foreach (KeyValuePair<Chatroom, Guid> item in dictionaryChatRoomV2)
            //{
            //    if (item.Key.ChatName == curItem)
            //    {
            //        item.Key.usersGuid.Add(x.GlobalIdentifier);

            //        button1.Enabled = false;
            //        messageBox.Enabled = true;
            //        sendButton.Enabled = true;
            //        chatBox.Enabled = true;
            //        message.chatGuid = item.Key.Identifier;



            //    }
            //}
            //sw.Flush();



        }
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
        
        private Guid AssignMessageId()
        {
            string curItem = listBox1.SelectedItem.ToString();

            foreach (KeyValuePair<Chatroom, Guid> item in dictionaryChatRoomV2)
            {
                if (item.Key.ChatName == curItem)
                {
                    item.Key.usersGuid.Add(x.GlobalIdentifier);

                    //button1.Enabled = false;
                    
                    messageGuid = item.Key.Identifier;

                    break;

                }
                //else
                //{
                //    MessageBox.Show("Please choose a room");
                //    messageGuid = Guid.Empty;
                //}
            }
            if(messageGuid == Guid.Empty)
            {
                MessageBox.Show("Please choose a room");
            }
            return messageGuid;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            

            //currentRoom.chatRoomUsers.Add(x.GlobalIdentifier, x);
        }

        private void CheckUserInChatRoom()
        {

        }

        private void chatBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
