using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientSide
{
    public partial class LoginPage : Form
    {
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
    }
}
