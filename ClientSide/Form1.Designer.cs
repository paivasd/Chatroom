namespace ClientSide
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nickName = new System.Windows.Forms.Label();
            this.nickBox = new System.Windows.Forms.TextBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.chatBox = new System.Windows.Forms.TextBox();
            this.messageBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.ipBox = new System.Windows.Forms.TextBox();
            this.serverIP = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // nickName
            // 
            this.nickName.AutoSize = true;
            this.nickName.Location = new System.Drawing.Point(28, 37);
            this.nickName.Name = "nickName";
            this.nickName.Size = new System.Drawing.Size(55, 13);
            this.nickName.TabIndex = 0;
            this.nickName.Text = "Nickname";
            // 
            // nickBox
            // 
            this.nickBox.Location = new System.Drawing.Point(89, 34);
            this.nickBox.Name = "nickBox";
            this.nickBox.Size = new System.Drawing.Size(100, 20);
            this.nickBox.TabIndex = 1;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(195, 32);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // chatBox
            // 
            this.chatBox.Enabled = false;
            this.chatBox.Location = new System.Drawing.Point(31, 64);
            this.chatBox.Multiline = true;
            this.chatBox.Name = "chatBox";
            this.chatBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chatBox.Size = new System.Drawing.Size(374, 310);
            this.chatBox.TabIndex = 3;
            this.chatBox.TextChanged += new System.EventHandler(this.chatBox_TextChanged);
            // 
            // messageBox
            // 
            this.messageBox.Enabled = false;
            this.messageBox.Location = new System.Drawing.Point(31, 380);
            this.messageBox.Name = "messageBox";
            this.messageBox.Size = new System.Drawing.Size(374, 20);
            this.messageBox.TabIndex = 4;
            // 
            // sendButton
            // 
            this.sendButton.Enabled = false;
            this.sendButton.Location = new System.Drawing.Point(411, 378);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 5;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // ipBox
            // 
            this.ipBox.Location = new System.Drawing.Point(89, 6);
            this.ipBox.Name = "ipBox";
            this.ipBox.Size = new System.Drawing.Size(100, 20);
            this.ipBox.TabIndex = 6;
            this.ipBox.Text = "192.168.56.1";
            // 
            // serverIP
            // 
            this.serverIP.AutoSize = true;
            this.serverIP.Location = new System.Drawing.Point(35, 9);
            this.serverIP.Name = "serverIP";
            this.serverIP.Size = new System.Drawing.Size(48, 13);
            this.serverIP.TabIndex = 7;
            this.serverIP.Text = "ServerIP";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "LESI",
            "EDJD",
            "EEC",
            "DG",
            "SOL"});
            this.listBox1.Location = new System.Drawing.Point(602, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(135, 342);
            this.listBox1.TabIndex = 9;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(602, 351);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(135, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Join Room";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.serverIP);
            this.Controls.Add(this.ipBox);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.messageBox);
            this.Controls.Add(this.chatBox);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.nickBox);
            this.Controls.Add(this.nickName);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label nickName;
        private System.Windows.Forms.TextBox nickBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox chatBox;
        private System.Windows.Forms.TextBox messageBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox ipBox;
        private System.Windows.Forms.Label serverIP;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
    }
}

