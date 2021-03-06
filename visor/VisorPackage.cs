﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ninject;
using Symitar;
using Visor.Extensions;
using Visor.LanguageService;
using Visor.Lib;
using Visor.Net.Ftp;
using Visor.Options;
using Visor.Project;
using Visor.ReportRunner;
using Visor.Toolbar;
using File = System.IO.File;
using Task = System.Threading.Tasks.Task;

namespace Visor
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    ///     The minimum requirement for a class to be considered a valid package for Visual Studio
    ///     is to implement the IVsPackage interface and register itself with the shell.
    ///     This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///     to do it: it derives from the Package class that provides the implementation of the
    ///     IVsPackage interface and uses the registration attributes defined in the framework to
    ///     register itself and its components with the shell.
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
    [ProvideService(typeof (IronyLanguageService))]
    [ProvideLanguageService(typeof (IronyLanguageService),
        "RepGen",
        106, // resource ID of localized language name
        ShowCompletion = true, // Automatically show completion
        CodeSense = true, // Supports IntelliSense
        EnableCommenting = true, // Supports commenting out code
        EnableAsyncCompletion = true, // Supports background parsing
        RequestStockColors = true
        )]
    [ProvideLanguageExtension(typeof (IronyLanguageService), ".rg")]
    // -------------------------------------------------------------------------------
    // Project Factory service
    // -------------------------------------------------------------------------------
    [ProvideProjectFactory(
        typeof (ProjectFactory),
        "Repgen",
        "Repgen Project Files (*.symproj);*.symproj",
        "symproj", "symproj",
        @"Project\Templates\Projects\BasicProject",
        LanguageVsTemplate = "Repgen")]
    [ProvideProjectItem(GuidList.VisorPackageString, "Code", @"Project\Templates\ItemTemplate\Batch", 1)]
    [ProvideProjectItem(GuidList.VisorPackageString, "Code", @"Project\Templates\ItemTemplate\Demand", 2)]
    [ProvideProjectItem(GuidList.VisorPackageString, "Data", @"Project\Templates\ItemTemplate\Letter", 3)]
    [ProvideProjectItem(GuidList.VisorPackageString, "Data", @"Project\Templates\ItemTemplate\Help", 4)]
    [ProvideProjectItem(GuidList.VisorPackageString, "Web", @"Project\Templates\ItemTemplate\HTML", 5)]
    // -------------------------------------------------------------------------------
    // Toolbar resource
    // -------------------------------------------------------------------------------
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof (VisorOptions), "Visor", "General", 0, 0, true)]
    // -------------------------------------------------------------------------------
    // Tool windows
    // -------------------------------------------------------------------------------
    [ProvideToolWindow(typeof (ReportToolWindow), Style = VsDockStyle.Tabbed,
        Window = GuidList.VisorReportRunnerWindowString)]
    //[ProvideToolWindow(typeof(ReportPromptDialog), Style = VsDockStyle.AlwaysFloat, Window = GuidList.VisorReportPromptWindowString)]
    public sealed class VisorPackage : IronyPackage
    {
        public ToolWindowPane ReportRunnerToolWindow;
        private List<string> _answers;
        private string _comboSelection;
        private SymDirectory _currentSymDirectory;
        private List<SymDirectory> _directories;
        private IKernel _kernel;

        /// <summary>
        ///     Default constructor of the package.
        ///     Inside this method you can place any initialization code that does not require
        ///     any Visual Studio service because at this point the package object is created but
        ///     not sited yet inside Visual Studio environment. The place to do all the other
        ///     initialization is the Initialize method.
        /// </summary>
        public VisorPackage()
        {
            Debug.WriteLine("Entering constructor for: {0}", ToString());
        }


        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine("Entering Initialize() of: {0}", ToString());
            base.Initialize();

            _kernel = new StandardKernel(new ServiceModule());

            ReportRunnerToolWindow = FindToolWindow(typeof (ReportToolWindow), 0, true);
            ShowReportWindow();
            RegisterProjectFactory(new ProjectFactory(this));
            RegisterMenuCommands();
        }
        

        private void RegisterMenuCommands()
        {
            // Add our command handlers for menu (commands must exist in the .vsct file)
            var menuCommandService = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
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
            var menuCommandID = new CommandID(GuidList.VisorCmdSet, (int) commandId);
            var menuItem = new OleMenuCommand(commandHandler, menuCommandID);

            if (commandId != PkgCmdIDList.SymDirectorySelect && commandId != PkgCmdIDList.SymDirectorySelectOptions)
                menuItem.BeforeQueryStatus += CommandVisibility;

            var menuCommandService = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            menuCommandService.AddCommand(menuItem);

            return menuItem;
        }

        private void CommandVisibility(object sender, EventArgs e)
        {
            var item = sender as OleMenuCommand;
            var dte = GetGlobalService(typeof (DTE)) as DTE;

            if (item == null || dte == null) return;

            item.Enabled = (_currentSymDirectory != null && dte.ActiveDocument != null);
        }

        private void CommandRequiresLogin(object sender, EventArgs e)
        {
            var item = sender as OleMenuCommand;
            var dte = GetGlobalService(typeof (DTE)) as DTE;

            if (item == null || dte == null) return;

            item.Enabled = (_currentSymDirectory != null && dte.ActiveDocument != null && _currentSymDirectory.LoggedIn);
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
                    _comboSelection = (string) input;

                    int openParen = _comboSelection.IndexOf('(');
                    int closeParen = _comboSelection.IndexOf(')');
                    int length = closeParen - openParen - 1;

                    string host = _comboSelection.Substring(openParen + 1, length);
                    int institution = int.Parse(_comboSelection.Split(' ')[1]);

                    _currentSymDirectory = _directories.Single(x => x.Institution == institution && x.Server.Host == host);
                }
                else if (output != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(_comboSelection, output);
                }
            }

            if (_directories == null || _currentSymDirectory != null) return;

            if (_directories.Count > 0)
            {
                _currentSymDirectory = _directories.First();
                _comboSelection = _currentSymDirectory.ToString();
            }
        }

        private void LoadOptions()
        {
            _directories = new List<SymDirectory>();
            var options = new VisorOptions();

            options.LoadSettingsFromStorage();

            _directories.AddRange(options.Directories);
        }

        private async void UploadCurrentFile(object sender, EventArgs e)
        {
            var currentFile = GetCurrentFilePath();

            try
            {
                var fileUploaded = await Task.Run(() =>
                {
                    var result = DialogResult.Yes;
                    if (_currentSymDirectory.FileExists(currentFile))
                        result = Confirmation("File Exists on Server", " Upload?");

                    return result == DialogResult.Yes ? _currentSymDirectory.UploadFile(currentFile) : "";
                });
                FtpUploadSuccess(fileUploaded);
            }
            catch (FtpException ex)
            {
                FtpError(ex);
            }
            catch (Exception ex)
            {
                ErrorMessage("Error!", ex.Message);
            }
        }

        private void FtpUploadSuccess(string fileName)
        {
            if (fileName.IsBlank()) return;
            var message = String.Format("{0} successfully uploaded to {1}", Path.GetFileNameWithoutExtension(fileName), _currentSymDirectory);
            MessageBox("File Uploaded", message);
        }

        private void FtpError(Exception exception)
        {
            FtpError(exception.Message);
        }

        private void FtpError(string message)
        {
            ErrorMessage("File Transfer Failed!", message);
        }

        private void DownloadCurrentFile(object sender, EventArgs e)
        {
            var path = GetCurrentFilePath();
            try
            {
                var result = _currentSymDirectory.DownloadFile(path);
                if(result.IsBlank()) FtpError("File not found.");
            }
            catch (FtpException ex)
            {
                FtpError(ex);
            }
            catch (Exception ex)
            {
                ErrorMessage("Error!", ex.Message);
            }
        }

        private async void InstallCurrentFile(object sender, EventArgs e)
        {
            try
            {
                var fileUploaded = await Task.Run(() =>
                {
                    return _currentSymDirectory.UploadFile(GetCurrentFilePath());
                });
                if (fileUploaded.IsNotBlank()) RunInstall(fileUploaded);
            }
            catch (FtpException ex)
            {
                FtpError(ex);
            }
            catch (Exception ex)
            {
                ErrorMessage("Error Installing Specfile!", ex.Message);
            }
        }

        private async void RunInstall()
        {
            RunInstall(GetCurrentFilePath());
        }

        private async void RunInstall(string fileName)
        {
            _currentSymDirectory.Connect();
            try
            {
                var result = await Task.Run(() =>
                {
                    return _currentSymDirectory.Install(fileName);
                });

                InstallSuccess(fileName, result);
            }
            catch (Exception e)
            {
                InstallFail(fileName, e.Message);
            }
            finally
            {
                _currentSymDirectory.Disconnect();
            }
        }

        private void InstallSuccess(string fileName, int installSize)
        {
            MessageBox(String.Format("{0} successfully installed in {1}", fileName, _currentSymDirectory),
                       String.Format("Install Size: {0:N0} Bytes", installSize));
        }

        private void InstallFail(string fileName, string errorMessage)
        {
            ErrorMessage(String.Format("{0} failed to install!", fileName),
                         errorMessage);
        }

        private async void RunCurrentFile(object sender, EventArgs e)
        {
            _answers = new List<string>();

            string currentFile = GetCurrentFilePath();

            List<ReportPrompt> prompts = GetPrompts(currentFile);

            if (prompts.Any())
            {
                var promptDialog = new ReportPromptDialog(prompts);
                promptDialog.SizeToContent = SizeToContent.WidthAndHeight;
                promptDialog.Title = String.Format("{0} Prompts", Path.GetFileNameWithoutExtension(currentFile));
                bool? dialogResult = promptDialog.ShowModal();

                if (dialogResult.HasValue && dialogResult.Value)
                {
                    _answers = promptDialog.Answers;
                }
                else
                {
                    return;
                }
            }

            try
            {
                var fileUploaded = await Task.Run(() =>
                {
                    return _currentSymDirectory.UploadFile(GetCurrentFilePath());
                });
                if (fileUploaded.IsNotBlank()) RunReport(fileUploaded);
            }
            catch (FtpException ex)
            {
                FtpError(ex);
            }
            catch (Exception ex)
            {
                ErrorMessage("Error Running Report!", ex.Message);
            }
        }

        private List<ReportPrompt> GetPrompts(string path)
        {
            var result = new List<ReportPrompt>();

            string[] lines = File.ReadAllLines(path);

            var dateReadExpression = new Regex(@"dateread\s*\((?<prompts>[^\)]*)\)", RegexOptions.IgnoreCase);
            var promptExpression = new Regex(@"read\s*\((?<prompts>[^\)]*)\)", RegexOptions.IgnoreCase);

            foreach (string line in lines)
            {
                var match = dateReadExpression.Match(line);
                if (match.Success)
                {
                    string text = GetFullPromptText(match.Groups["prompts"].Captures);
                    if (!string.IsNullOrEmpty(text))
                        result.Add(new ReportPrompt {Type = PromptType.Date, Text = text.Trim()});
                }
                else
                {
                    match = promptExpression.Match(line);
                    if (match.Success)
                    {
                        string text = GetFullPromptText(match.Groups["prompts"].Captures);
                        if (!string.IsNullOrEmpty(text))
                            result.Add(new ReportPrompt {Type = PromptType.Character, Text = text});
                    }
                }
            }

            return result;
        }

        private string GetFullPromptText(CaptureCollection captures)
        {
            var promptList = new Regex(@"\""(?<prompt>[^\""]*)\""\s*,?\s*", RegexOptions.IgnoreCase);
            foreach (Capture c in captures)
            {
                var promptMatches = promptList.Matches(c.Value);

                var promptTextArray = promptMatches.Cast<Match>()
                                                        .SelectMany(m => m.Groups["prompt"].Captures.Cast<Capture>())
                                                        .Select(p => p.Value).ToArray();
                return String.Join("\n", promptTextArray);
            }
            return "";
        }

        private void RunReport(string fileName)
        {
            ShowReportWindow();
            Dispatch(
                () => ((ReportRunnerControl) ReportRunnerToolWindow.Content).AddBatchJob(fileName, _currentSymDirectory));

            try
            {
                _currentSymDirectory.Connect();
                _currentSymDirectory.Run(fileName, _answers, UpdateReportStatus, ReportCompleted);
            }
            catch (Exception e)
            {
                Dispatch(() => ((ReportRunnerControl) ReportRunnerToolWindow.Content).RemoveBatchJob());
                ErrorMessage("Error Running Report!", e.Message);
            }
            finally
            {
                _answers = new List<string>();
            }
        }

        private void UpdateReportStatus(SymSession.RunState state, object data)
        {
            string file;
            switch (state)
            {
                case SymSession.RunState.Running:
                    var sequence = (int) data;
                    ((ReportRunnerControl) ReportRunnerToolWindow.Content).SetSequence(sequence);
                    ((ReportRunnerControl) ReportRunnerToolWindow.Content).UpdateStatus(sequence, "Running");
                    break;
                case SymSession.RunState.Failed:
                    file = (string) data;
                    ((ReportRunnerControl) ReportRunnerToolWindow.Content).UpdateStatus(file, "Failed");
                    break;
                case SymSession.RunState.Cancelled:
                    file = (string) data;
                    ((ReportRunnerControl) ReportRunnerToolWindow.Content).UpdateStatus(file, "Cancelled");
                    break;
            }
        }

        private void ReportCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            if (args.Error != null)
            {
                var fileName = (string) ((object[]) args.Result)[0];
                UpdateReportStatus(SymSession.RunState.Failed, fileName);
                ErrorMessage("Run Report Failed!", args.Error.Message);
            }
            else if (args.Cancelled)
            {
                MessageBox("Run Report Cancelled", "");
            }
            else
            {
                var jobSequence = (int) ((object[]) args.Result)[1];
                var outputSequence = (int) ((object[]) args.Result)[2];

                List<Report> reports = _currentSymDirectory.GetReports(outputSequence);

                Dispatch(
                    () => ((ReportRunnerControl) ReportRunnerToolWindow.Content).UpdateStatus(jobSequence, "Complete"));

                BatchJob job = ((ReportRunnerControl) ReportRunnerToolWindow.Content).GetJob(jobSequence);

                if (job != null)
                {
                    job.Reports = new ReportList();
                    foreach (Report report in reports)
                    {
                        Report r = report;
                        Dispatch(() => job.AddReport(r));
                    }
                }
            }

            _currentSymDirectory.Disconnect();
        }

        private void ShowReportWindow()
        {
            try
            {
                ToolWindowPane pane = FindToolWindow(typeof (ReportToolWindow), 0, true);
                var frame = pane.Frame as IVsWindowFrame;
                ErrorHandler.ThrowOnFailure(frame.Show());
            }
            catch
            {
                throw new COMException("Error opening Report Tool Window");
            }
        }

        private int MessageBox(string title, string message)
        {
            // Show a Message Box to prove we were here
            var uiShell = (IVsUIShell) GetService(typeof (SVsUIShell));
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

        private void ErrorMessage(string title, string message)
        {
            // Show a Message Box to prove we were here
            var uiShell = (IVsUIShell) GetService(typeof (SVsUIShell));
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
                OLEMSGICON.OLEMSGICON_WARNING,
                0, // false
                out result));
        }

        private int Confirmation(string title, string message)
        {
            // Show a Message Box to prove we were here
            var uiShell = (IVsUIShell) GetService(typeof (SVsUIShell));
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
            return FileHandler.GetActiveDocumentPath();
        }

        private void Dispatch(Action action)
        {
            Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                action);
        }
    }
}