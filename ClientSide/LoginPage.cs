using Newtonsoft.Json;
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
    public partial class LoginPage : Form
    {
        public static string jsonTest;
        public static string ipText;
        public static string userText;
        User currentUser;
        IPAddress _ip;
        TcpClient _tcpClient;
        bool connected;
        StreamWriter sw;
        StreamReader sr;
        Thread thrMessage;
        Message message;
        private bool _dragging;
        private Point _offset;




        public LoginPage()
        {

            InitializeComponent();
            //iconButton1.IconColor = myColor;
            //iconButton2.IconColor = myColor;
            //iconButton3.IconColor = myColor;
        }
        #region HANDLE WINDOW MOVEMENT
        private void LoginPage_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point currentScreenPos = PointToScreen(e.Location);
                Location = new Point
                    (currentScreenPos.X - _offset.X,
                     currentScreenPos.Y - _offset.Y);
            }
        }
        private void LoginPage_MouseDown(object sender, MouseEventArgs e)
        {
            _offset.X = e.X;
            _offset.Y = e.Y;
            _dragging = true;
        }
        private void LoginPage_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }
        #endregion

        private void iconButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            ipText = ipBox.Text;
            Register form = new Register();
            form.ShowDialog();
            

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            
            try
            {
                ipText = ipBox.Text;
                userText = userNameBox.Text;

                //Attempts the connection to the server
                _ip = IPAddress.Parse(ipBox.Text);
                _tcpClient = new TcpClient();
                _tcpClient.Connect(_ip, 9000);

                connected = true;

                currentUser = new User();
                currentUser.Username = userNameBox.Text;
                currentUser.Password = passwordBox.Text;
                currentUser.Registered = true;

                //Open a StreamWriter
                sw = new StreamWriter(_tcpClient.GetStream());
                string json = JsonConvert.SerializeObject(currentUser);

                sw.WriteLine(json); 

                sw.Flush();

                //Initialize thread
                
                thrMessage = new Thread(new ThreadStart(Receive));
                thrMessage.Start();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Receive()
        {
            //Open a StreamReader to be able to fetch Server responses
            sr = new StreamReader(_tcpClient.GetStream());
            //Fetch the server response
            string connectionResponse = sr.ReadLine();

            //Flag we use on successful connection
            if (connectionResponse[0] == '1')
            {
                //Reading line from StreamReader
                string response = sr.ReadLine();
                if (response != "")
                {
                    message = JsonConvert.DeserializeObject<Message>(response);
                    if (message.MessageType == Message.Type.SuccessfulLogin)
                    {
                        //Deserialize into User object
                        currentUser = JsonConvert.DeserializeObject<User>(message.MessageBody);
                        
                        MessageBox.Show("Succesfull login!");
                        jsonTest = JsonConvert.SerializeObject(currentUser);

                        
                       




                    }
                }
                Form1 form = new Form1();
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Connection Error");
            }
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
