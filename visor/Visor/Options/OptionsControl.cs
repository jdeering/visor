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

            foreach (var d in selectedDirectories)
            {
                directoryList.Items.Remove(d);
            }
        }
    }
}
