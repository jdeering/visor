using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Visor.Extensions;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Visor.Options
{
    [Guid(GuidList.VisorOptionsPageString)]
    public class VisorOptions : DialogPage
    {
        private string _saveDirectory;
        private string _savePath;

        public List<SymDirectory> Directories;

        public VisorOptions()
        {
            Directories = new List<SymDirectory>();

            _saveDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Visor");
            _savePath = Path.Combine(_saveDirectory, @"serverOptions.xml");
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(
            DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window
        {
            get
            {
                var page = new OptionsControl();
                page.Options = this;
                page.Initialize();
                return page;
            }
        }

        public override void SaveSettingsToStorage()
        {
            // Delete existing file
            if (File.Exists(_savePath))
                File.Delete(_savePath);

            Directory.CreateDirectory(_saveDirectory);

            var json = JsonConvert.SerializeObject(Directories);

            var encryptedData = Crypto.Encrypt(json);

            File.WriteAllText(_savePath, Convert.ToBase64String(encryptedData));

            base.SaveSettingsToStorage();
        }

        public override void LoadSettingsFromStorage()
        {
            if (!File.Exists(_savePath)) return;

            try
            {
                string fileData = File.ReadAllText(_savePath);
                var encryptedData = Convert.FromBase64String(fileData);
                var json = Crypto.Decrypt(encryptedData);

                Directories = JsonConvert.DeserializeObject<List<SymDirectory>>(json);
            }
            catch(Exception e)
            {
                var exc = e;
                throw e;
            }

            base.LoadSettingsFromStorage();
        }
    }
}
