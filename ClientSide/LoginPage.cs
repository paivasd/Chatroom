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
        User currentUser;
        IPAddress _ip;
        TcpClient _tcpClient;
        bool connected;
        StreamWriter sw;
        StreamReader sr;
        Thread thrMessage;
        Message message;

        public LoginPage()
        {
            InitializeComponent();
            string hexColor = "#49D49D";

            Color myColor = System.Drawing.ColorTranslator.FromHtml(hexColor);

            //iconButton1.IconColor = myColor;
            //iconButton2.IconColor = myColor;
            //iconButton3.IconColor = myColor;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            Register form = new Register();
            form.ShowDialog();

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            try
            {

                _ip = IPAddress.Parse(ipBox.Text);
                _tcpClient = new TcpClient();
                _tcpClient.Connect(_ip, 9000);

                connected = true;

                currentUser = new User();
                currentUser.Username = userNameBox.Text;
                currentUser.Password = passwordBox.Text;
                currentUser.Registered = true;


                sw = new StreamWriter(_tcpClient.GetStream());
                string json = JsonConvert.SerializeObject(currentUser);
                //string json2 = JsonConvert.SerializeObject(dictionaryChatRoom);
                sw.WriteLine(json); // // // // // // // // // // // dá para passar objetos tbm
                //sw.WriteLine(json2);
                //sw.WriteLine(json2);
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
                        //deserializar para objecto retornado do servidor
                        MessageBox.Show("Succesfull login!");
                        jsonTest = JsonConvert.SerializeObject(currentUser);

                        
                        //Form1 form = new Form1();
                        //form.ShowDialog();
                        //this.Hide();





                    }
                }

                
                //// update to inform we connect
                //this.Invoke(new UpdateLogCallBack(this.LogUpdate), new object[] { "You successfully connected to the server" });
            }
        
            Form1 form = new Form1();
            form.ShowDialog();
            //else // if the first char is not 1, it means the connection failed
            //{
            //    //string aux = "Not connected:  ";
            //    //// why? answer starts in the 3rd character
            //    //aux += connectionResponse.Substring(2, connectionResponse.Length - 2);

            //    //// Aupdate textbox
            //    //this.Invoke(new CloseConnectionCallBack(this.CloseConnection), new object[] { aux });
            //    //return;
            //}


            //while (connected)
            //{
            //    string response = sr.ReadLine();
            //    if (response != "")
            //    {
            //        message = JsonConvert.DeserializeObject<Message>(response);
            //        //if (message.MessageType == Message.Type.SuccessLogin)
            //        //{
            //        //    MessageBox.Show("Succesfully registered!");


            //        //}
            //    }
            //}
        }


        private void CloseConnection(string diff)
        {
            // Mostra o motivo porque a conexão encerrou
            connected = false;
            sw.Close();
            sr.Close();
            _tcpClient.Close();
        }


    }
}
