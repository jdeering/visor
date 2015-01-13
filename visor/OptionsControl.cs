using System;
using System.Linq;
using System.Windows.Forms;
using Visor.Lib;

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
            foreach (SymDirectory d in Options.Directories)
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
                var serverInfo = new SymServerInfo
                    {
                        Host = host.Text,
                        TelnetPort = int.Parse(telnet.Text),
                        FtpPort = int.Parse(ftp.Text),
                        AixUsername = username.Text,
                        AixPassword = password.Text
                    };

                var newDirectory = new SymDirectory
                    {
                        Server = serverInfo,
                        Institution = int.Parse(directory.Text),
                        UserId = userId.Text
                    };

                SymDirectory existingDirectory =
                    Options.Directories.FirstOrDefault(
                        x => x.Institution == newDirectory.Institution && x.Server.Host == serverInfo.Host);

                if (existingDirectory == null)
                {
                    Options.Directories.Add(newDirectory);
                    directoryList.Items.Add(newDirectory);
                }
                else
                {
                    int index = Options.Directories.IndexOf(existingDirectory);
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
            ListBox.SelectedObjectCollection selectedDirectories = directoryList.SelectedItems;

            for (int i = 0; i < selectedDirectories.Count; i++)
            {
                var d = (SymDirectory) selectedDirectories[i];
                directoryList.Items.Remove(d);
                Options.Directories.Remove(d);
            }

            ClearFields();
        }

        private void LoadDirectoryInfo(object sender, EventArgs e)
        {
            if (directoryList.SelectedItems.Count == 1)
            {
                var symDirectory = (SymDirectory) directoryList.SelectedItem;

                host.Text = symDirectory.Server.Host;
                telnet.Text = symDirectory.Server.TelnetPort.ToString();
                ftp.Text = symDirectory.Server.FtpPort.ToString();

                username.Text = symDirectory.Server.AixUsername;
                password.Text = symDirectory.Server.AixPassword;

                directory.Text = symDirectory.Institution.ToString();
                userId.Text = symDirectory.UserId;
            }
        }

        private void ClearFields()
        {
            foreach (TextBox field in Controls.OfType<TextBox>())
            {
                field.Text = "";
            }
        }
    }
}