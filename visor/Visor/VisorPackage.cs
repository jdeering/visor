using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Visor.LanguageService;
using Visor.Toolbar;
using Visor.Options;

namespace Visor
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]

    [Guid(GuidList.VisorPackageString)]

    // -------------------------------------------------------------------------------
    // Language Service
    // -------------------------------------------------------------------------------
    [ProvideService(typeof(IronyLanguageService))]
    [ProvideLanguageService(typeof(IronyLanguageService),
        "RepGen",
        106, // resource ID of localized language name
        ShowCompletion = true, // Automatically show completion
        CodeSense = true, // Supports IntelliSense
        EnableCommenting = true, // Supports commenting out code
        EnableAsyncCompletion = true, // Supports background parsing
        RequestStockColors = true
    )]
    [ProvideLanguageExtension(typeof(IronyLanguageService), ".rg")]

    // -------------------------------------------------------------------------------
    // Project Factory service
    // -------------------------------------------------------------------------------
    [ProvideProjectFactory(
        typeof(Visor.Project.ProjectFactory),
        "Repgen",
        "Repgen Project Files (*.symproj);*.symproj",
        "symproj", "symproj",
        @"Project\Templates\Projects\BasicProject",
        LanguageVsTemplate = "VisorProject")]

    [ProvideProjectItem(GuidList.VisorPackageString, "Code", @"Project\Templates\ItemTemplate\Batch", 1)]
    [ProvideProjectItem(GuidList.VisorPackageString, "Code", @"Project\Templates\ItemTemplate\Demand", 2)]
    [ProvideProjectItem(GuidList.VisorPackageString, "Data", @"Project\Templates\ItemTemplate\Letter", 3)]
    [ProvideProjectItem(GuidList.VisorPackageString, "Data", @"Project\Templates\ItemTemplate\Help", 4)]
    [ProvideProjectItem(GuidList.VisorPackageString, "Web", @"Project\Templates\ItemTemplate\HTML", 5)]

    // -------------------------------------------------------------------------------
    // Toolbar resource
    // -------------------------------------------------------------------------------
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(VisorOptions), "Visor", "General", 0, 0, true)]
    public sealed class VisorPackage : IronyPackage
    {
        private string _comboSelection;
        private SymDirectory _currentDirectory;
        private List<SymDirectory> _directories;


        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public VisorPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            RegisterProjectFactory(new Visor.Project.ProjectFactory(this));
            RegisterMenuCommands();
        }


        private void RegisterMenuCommands()
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            var menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menuCommandService == null) return;

            RegisterCommand(PkgCmdIDList.Upload, UploadCurrentFile);
            RegisterCommand(PkgCmdIDList.Download, DownloadCurrentFile);
            RegisterCommand(PkgCmdIDList.InstallSpecfile, InstallCurrentFile);
            RegisterCommand(PkgCmdIDList.RunReport, RunCurrentFile);

            RegisterCommand(PkgCmdIDList.SymDirectorySelect, SetSymDirectory);
            RegisterCommand(PkgCmdIDList.SymDirectorySelectOptions, LoadSymDirectoryCombo);
        }

        private OleMenuCommand RegisterCommand(uint commandId, EventHandler commandHandler)
        {
            var menuCommandID = new CommandID(GuidList.VisorCmdSet, (int)commandId);
            var menuItem = new OleMenuCommand(commandHandler, menuCommandID);

            var menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            menuCommandService.AddCommand(menuItem);

            return menuItem;
        }

        private void LoadSymDirectoryCombo(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;

            if (eventArgs != null)
            {
                IntPtr output = eventArgs.OutValue;
                object input = eventArgs.InValue;
                if (output != IntPtr.Zero)
                {
                    LoadOptions();
                    var directories = new List<string>();
                    directories.AddRange(_directories.Select(x => x.ToString()));

                    Marshal.GetNativeVariantForObject(directories.ToArray(), output);
                }
            }
        }

        private void SetSymDirectory(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                IntPtr output = eventArgs.OutValue;
                object input = eventArgs.InValue;
                if (input != null)
                {
                    LoadOptions();
                    _comboSelection = (string)input;

                    var openParen = _comboSelection.IndexOf('(');
                    var closeParen = _comboSelection.IndexOf(')');
                    var length = closeParen - openParen - 1;

                    var host = _comboSelection.Substring(openParen + 1, length);
                    var institution = int.Parse(_comboSelection.Split(' ')[1]);

                    if (_currentDirectory != null) _currentDirectory.Disconnect();

                    _currentDirectory = _directories.Single(x => x.Institution == institution && x.Server.Host == host);
                    StartSessionAsync();
                }
                else if (output != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(_comboSelection, output);
                }
            }

            if (_directories == null || _currentDirectory != null) return;

            if (_directories.Count > 0)
            {
                _currentDirectory = _directories.First();
                StartSessionAsync();
                _comboSelection = String.Format("Sym {0} ({1})", _currentDirectory.Institution, _currentDirectory.Server.Host);
            }
        }

        private void StartSessionAsync()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(delegate { _currentDirectory.Connect(); }, null);
        }

        private void LoadOptions()
        {
            _directories = new List<SymDirectory>();
            var options = new VisorOptions();

            options.LoadSettingsFromStorage();

            _directories.AddRange(options.Directories);
        }

        private void UploadCurrentFile(object sender, EventArgs e)
        {
            string currentFile = GetCurrentFilePath();

            int result = DialogResult.Yes;
            if (_currentDirectory.FileExists(currentFile))
                result = Confirmation("File Exists on Server", " Upload?");

            if (result == DialogResult.Yes)
            {
                _currentDirectory.UploadFile(currentFile, FtpUploadSuccess, FtpError);
            }
        }

        private void FtpUploadSuccess(string fileName)
        {
            MessageBox("File Uploaded",
                       String.Format("{0} successfully uploaded to {1}",
                                    Path.GetFileNameWithoutExtension(fileName),
                                    String.Format("Sym {0} ({1})", _currentDirectory.Institution, _currentDirectory.Server.Host)));
        }

        private void FtpDownloadSuccess(string fileName)
        {
            // Not showing a message on success
        }

        private void FtpError(Exception exception)
        {
            MessageBox("An exception has occured!", exception.Message);
        }

        private void DownloadCurrentFile(object sender, EventArgs e)
        {
            _currentDirectory.DownloadFile(GetCurrentFilePath(), FtpDownloadSuccess, FtpError);
        }

        private void InstallCurrentFile(object sender, EventArgs e)
        {
            /*
            try
            {
                var specfileName = Path.GetFileNameWithoutExtension(GetCurrentFilePath());
                UploadCurrentFile(sender, e);
                _directory.Install(specfileName, _currentDirectory);
            }
            catch (Exception exception)
            {
                MessageBox("Error Installing Specfile", exception.Message);
            }
            */
        }

        private void RunCurrentFile(object sender, EventArgs e)
        {
            /*
            try
            {
                var specfileName = Path.GetFileNameWithoutExtension(GetCurrentFilePath());
                UploadCurrentFile(sender, e);
                _server.Run(specfileName, _currentDirectory);
            }
            catch (Exception exception)
            {
                MessageBox("Error Running Report", exception.Message);
            }
            */
        }

        private int MessageBox(string title, string message)
        {
            // Show a Message Box to prove we were here
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                0,
                ref clsid,
                title,
                message,
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0, // false
                out result));

            return result;
        }

        private int Confirmation(string title, string message)
        {
            // Show a Message Box to prove we were here
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(
                uiShell.ShowMessageBox(
                    0,
                    ref clsid,
                    title,
                    message,
                    string.Empty,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_YESNOCANCEL,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    OLEMSGICON.OLEMSGICON_INFO,
                    0, // false
                    out result));

            return result;
        }

        private string GetCurrentFilePath()
        {
            try
            {
                var dte = GetGlobalService(typeof(DTE)) as DTE;
                return dte.ActiveDocument.FullName;
            }
            catch (Exception e)
            {
                throw new Exception("There is no active document.", e);
            }
        }
    }
}