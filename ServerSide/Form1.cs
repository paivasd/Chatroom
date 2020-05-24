﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerSide
{
    public partial class Form1 : Form
    {
        private delegate void UpdateStatusCallback(string strMessage);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                string ip = GetIPAddress();
                IPAddress _ipAddress = IPAddress.Parse(ip);

                NetworkStream instance = new NetworkStream(_ipAddress);
                NetworkStream.StatusChanged += new StatusChangedEventHandler(socket_StatusChanged);

                instance.Start();

                logfileTextBox.AppendText("Listening connections: " + ip + "...\r\n");
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Error: " + ex.Message);
            }
        }



        private void UpdateStatus(string strMessage)
        {

            logfileTextBox.AppendText(strMessage + "\r\n");
        }

        public void socket_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            this.Invoke(new UpdateStatusCallback(this.UpdateStatus), new object[] { e.EventMessage });
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private string GetIPAddress()
        {
            IPAddress[] ips = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress x in ips)
            {
                if (x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return x.ToString();
                }
            }
            return "127.0.0.1";
        }
    }
}