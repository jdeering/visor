using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Visor.Options
{
    public partial class OptionsControl : UserControl
    {
        internal VisorOptions Options;

        public OptionsControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
            foreach (var d in Options.Directories)
            {
                directoryList.Items.Add(d);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            host.Text = "";
            telnet.Text = "";
            ftp.Text = "";
            username.Text = "";
            password.Text = "";
            directory.Text = "";
            userId.Text = "";
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                SymServerInfo serverInfo = new SymServerInfo
                    {
                        Host = host.Text,
                        TelnetPort = int.Parse(telnet.Text),
                        FtpPort = int.Parse(ftp.Text),
                        AixUsername = username.Text,
                        AixPassword = password.Text
                    };

                SymDirectory newDirectory = new SymDirectory
                    {
                        Server = serverInfo,
                        Institution = int.Parse(directory.Text),
                        UserId = userId.Text
                    };

                var existingDirectory =
                    Options.Directories.FirstOrDefault(x => x.Institution == newDirectory.Institution && x.Server.Host == serverInfo.Host);

                if (existingDirectory == null)
                {
                    Options.Directories.Add(newDirectory);
                    directoryList.Items.Add(newDirectory);
                }
                else
                {
                    var index = Options.Directories.IndexOf(existingDirectory);
                    Options.Directories[index] = newDirectory;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Invalid option specified.");
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            var selectedDirectories = directoryList.SelectedItems;

            for (int i = 0; i < selectedDirectories.Count; i++)
            {
                var d = (SymDirectory)selectedDirectories[i];
                directoryList.Items.Remove(d);
                Options.Directories.Remove(d);
            }

            ClearFields();
        }

        private void LoadDirectoryInfo(object sender, EventArgs e)
        {
            if (directoryList.SelectedItems.Count == 1)
            {
                var directory = (SymDirectory)directoryList.SelectedItem;

                this.host.Text = directory.Server.Host;
                this.telnet.Text = directory.Server.TelnetPort.ToString();
                this.ftp.Text = directory.Server.FtpPort.ToString();

                this.username.Text = directory.Server.AixUsername;
                this.password.Text = directory.Server.AixPassword;

                this.directory.Text = directory.Institution.ToString();
                this.userId.Text = directory.UserId;
            }
        }

        private void ClearFields()
        {
            foreach (TextBox field in this.Controls.OfType<TextBox>())
            {
                field.Text = "";
            }
        }
    }
}
