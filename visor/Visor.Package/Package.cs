﻿using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Visor.LanguageService;

namespace Visor
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.VisorPackageString)]

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
    [ProvideLanguageExtension(typeof(IronyLanguageService), ".rg")]
    public sealed class Package : IronyPackage
    {
        /// <summary>
        ///     Default constructor of the package.
        ///     Inside this method you can place any initialization code that does not require
        ///     any Visual Studio service because at this point the package object is created but
        ///     not sited yet inside Visual Studio environment. The place to do all the other
        ///     initialization is the Initialize method.
        /// </summary>
        public Package()
        {
        }

        #region Package Members

        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        #endregion
    }
}