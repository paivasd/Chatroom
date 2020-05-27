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
        User currentUser;
        IPAddress _ip;
        TcpClient _tcpClient;
        bool connected;
        StreamWriter sw;
        StreamReader sr;
        Thread thrMessage;
        Message message;

        public Register()
        {
            InitializeComponent();
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

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
                    //currentUser.UserType = textbox

                }

                 _ip = IPAddress.Parse("192.168.56.1");
                 _tcpClient = new TcpClient();
                 _tcpClient.Connect(_ip, 9000);

                 connected = true;

                 currentUser.GlobalIdentifier = Guid.NewGuid();
                    

                 sw = new StreamWriter(_tcpClient.GetStream());
                 string json = JsonConvert.SerializeObject(currentUser);

                    //string json2 = JsonConvert.SerializeObject(dictionaryChatRoom);
                 sw.WriteLine(json); // // // // // // // // // // // dá para passar objetos tbm
                                        //sw.WriteLine(json2);
                                        //sw.WriteLine(json2);
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
            sr = new StreamReader(_tcpClient.GetStream());
            string connectionResponse = sr.ReadLine();

            if (connectionResponse[0] == '1')
            {
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

                //// update to inform we connect
                //this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { "You successfully connected to the server" });
            }
            else // if the first char is not 1, it means the connection failed
            {
                //string aux = "Not connected:  ";
                //// why? answer starts in the 3rd character
                //aux += connectionResponse.Substring(2, connectionResponse.Length - 2);

                //// Aupdate textbox
                //this.Invoke(new CloseConnectionCallBack(this.CloseConnection), new object[] { aux });
                //return;
            }


            //while (connected)
            //{
            //    string response = sr.ReadLine();
            //    if (response != "")
            //    {
            //        message = JsonConvert.DeserializeObject<Message>(response);
            //        if (message.MessageType == Message.Type.Register)
            //        {
            //            MessageBox.Show("Succesfully registered!");

                        
            //        }
            //    }
            //}
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
