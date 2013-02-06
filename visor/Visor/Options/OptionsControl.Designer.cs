namespace Visor.Options
{
    partial class OptionsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.directoryList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.host = new System.Windows.Forms.TextBox();
            this.telnet = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ftp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.username = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.directory = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.userId = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // directoryList
            // 
            this.directoryList.FormattingEnabled = true;
            this.directoryList.Location = new System.Drawing.Point(3, 3);
            this.directoryList.Name = "directoryList";
            this.directoryList.Size = new System.Drawing.Size(120, 199);
            this.directoryList.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(172, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Host";
            // 
            // host
            // 
            this.host.Location = new System.Drawing.Point(210, 0);
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size(179, 20);
            this.host.TabIndex = 2;
            // 
            // telnet
            // 
            this.telnet.Location = new System.Drawing.Point(210, 26);
            this.telnet.Name = "telnet";
            this.telnet.Size = new System.Drawing.Size(60, 20);
            this.telnet.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(142, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Telnet Port";
            // 
            // ftp
            // 
            this.ftp.Location = new System.Drawing.Point(328, 26);
            this.ftp.Name = "ftp";
            this.ftp.Size = new System.Drawing.Size(61, 20);
            this.ftp.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(276, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "FTP Port";
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(210, 73);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(179, 20);
            this.username.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(129, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "AIX Username";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(210, 99);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(179, 20);
            this.password.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(131, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "AIX Password";
            // 
            // directory
            // 
            this.directory.Location = new System.Drawing.Point(210, 148);
            this.directory.Name = "directory";
            this.directory.Size = new System.Drawing.Size(60, 20);
            this.directory.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(132, 151);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Sym Directory";
            // 
            // userId
            // 
            this.userId.Location = new System.Drawing.Point(210, 174);
            this.userId.Name = "userId";
            this.userId.PasswordChar = '*';
            this.userId.Size = new System.Drawing.Size(179, 20);
            this.userId.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(158, 177);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "User ID";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(27, 208);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 15;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(314, 208);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 16;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(229, 208);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 23);
            this.clearButton.TabIndex = 17;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // OptionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.userId);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.directory);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.password);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.username);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ftp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.telnet);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.host);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.directoryList);
            this.Name = "OptionsControl";
            this.Size = new System.Drawing.Size(392, 238);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox directoryList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox host;
        private System.Windows.Forms.TextBox telnet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ftp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox directory;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox userId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button clearButton;
    }
}
