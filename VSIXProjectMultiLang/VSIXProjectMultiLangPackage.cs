﻿using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace VSIXProjectMultiLang
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [ProvideOptionPage(typeof(MyConfigPage), "喇低賽的多語系顯示", "不負責任的選項", 987, 987, true)]
    [PackageRegistration( UseManagedResourcesOnly = true, AllowsBackgroundLoading = true )]
    [Guid( VSIXProjectMultiLangPackage.PackageGuidString )]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class VSIXProjectMultiLangPackage : AsyncPackage
    {
        /// <summary>
        /// VSIXProjectMultiLangPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "04cdc06a-6f85-4271-8905-882745183d53";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync( cancellationToken );
            await CommandMoveToIfBegin.InitializeAsync(this);
            await CommandMoveToBegin.InitializeAsync(this);
            await CommandMoveToEnd.InitializeAsync(this);
            await CommandSelectCurrentMethod.InitializeAsync(this);
            await CommandToggle.InitializeAsync(this);
            await CommandAddChinese.InitializeAsync(this);
            await CommandConvToVar.InitializeAsync(this);
            await CommandConvToBool.InitializeAsync(this);
            await CommandConvToEng.InitializeAsync(this);
            await CommandConvToInt.InitializeAsync(this);
            await CommandConvToDouble.InitializeAsync(this);
            await CommandConvToString.InitializeAsync(this);
            await CommandGoToFile.InitializeAsync(this);
            await CommandMatchit.InitializeAsync(this);
        }

        #endregion
    }
}
