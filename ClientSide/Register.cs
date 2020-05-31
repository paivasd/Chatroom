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
    public partial class Register : Form
    {
        public static string jsonTest;
        string ipText;
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


        public Register()
        {
            InitializeComponent();
            ipText = LoginPage.ipText;

        }

        #region HANDLE WINDOW MOVEMENT
        private void Register_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point currentScreenPos = PointToScreen(e.Location);
                Location = new Point
                    (currentScreenPos.X - _offset.X,
                     currentScreenPos.Y - _offset.Y);
            }
        }



        private void Register_MouseDown(object sender, MouseEventArgs e)
        {
            _offset.X = e.X;
            _offset.Y = e.Y;
            _dragging = true;
        }



        private void Register_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }
        #endregion

        private void iconButton4_Click(object sender, EventArgs e)
        {
            try
            {
                
                currentUser = new User();
                currentUser.Username = textBox1.Text;
                currentUser.Password = textBox2.Text;
                currentUser.Registered = false;
               
                if ((string)comboBox1.SelectedItem == "Student")
                {
                    currentUser.UserType = User.Type.Student;
                }
                else if ((string)comboBox1.SelectedItem == "Teacher")
                {
                    currentUser.UserType = User.Type.Teacher;
                }
                else { MessageBox.Show("Please choose a Type: Student or Teacher"); }

                if ((string)comboBox3.SelectedItem == "LESI")
                {
                    currentUser.CourseType = User.Course.LESI;
                }
                else if ((string)comboBox3.SelectedItem == "EDJD")
                {
                    currentUser.CourseType = User.Course.EDJD;
                }
                else if ((string)comboBox3.SelectedItem == "EEC")
                {
                    currentUser.CourseType = User.Course.EEC;
                }
                else
                {
                    MessageBox.Show("Please choose a Course");

                }

                 //Attempts the connection to the server
                 _ip = IPAddress.Parse(ipText);
                 _tcpClient = new TcpClient();
                 _tcpClient.Connect(_ip, 9000);

                 connected = true;

                 currentUser.GlobalIdentifier = Guid.NewGuid();
                    
                 //Open a StreamWriter
                 sw = new StreamWriter(_tcpClient.GetStream());
                 string json = JsonConvert.SerializeObject(currentUser);

                 sw.WriteLine(json);

                 sw.Flush();

                //Initialize thread
                this.Close();
                thrMessage = new Thread(new ThreadStart(Receive));
                thrMessage.Start();
            }
            catch(Exception ex)
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
                        currentUser = JsonConvert.DeserializeObject<User>(message.MessageBody);
                        jsonTest = message.MessageBody;

                        MessageBox.Show("Succesfully registered!");



                    }
                }

                
            }
            else 
            {
                MessageBox.Show("Connection Error");
            }

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
