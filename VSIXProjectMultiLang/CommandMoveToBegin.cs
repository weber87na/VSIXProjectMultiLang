using Microsoft;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace VSIXProjectMultiLang
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CommandMoveToBegin
    {
        private IWpfTextView wpfTextView;
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid( "d7d69c46-e99c-4a3c-95b8-9ac3a1e45289" );

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMoveToBegin"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CommandMoveToBegin(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException( nameof( package ) );
            commandService = commandService ?? throw new ArgumentNullException( nameof( commandService ) );

            var menuCommandID = new CommandID( CommandSet, CommandId );
            var menuItem = new MenuCommand( this.Execute, menuCommandID );
            commandService.AddCommand( menuItem );
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CommandMoveToBegin Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        //private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        //{
        //    get
        //    {
        //        return this.package;
        //    }
        //}

        private IServiceProvider ServiceProvider
        {
            get { return this.package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in Command1's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync( package.DisposalToken );

            OleMenuCommandService commandService = await package.GetServiceAsync( typeof( IMenuCommandService ) ) as OleMenuCommandService;
            Instance = new CommandMoveToBegin( package, commandService );
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread( );
            Exec( );
        }

        private void Exec()
        {
            this.wpfTextView = GetCurrentTextView( );
            string text = wpfTextView.Caret.ToString( );

            var span = wpfTextView.Caret.ContainingTextViewLine.Extent;
            Debug.WriteLine( text );

            MethodLogic methodLogic = new MethodLogic( );
            methodLogic.MoveToBegin( wpfTextView );

        }


        public IWpfTextView GetCurrentTextView()
        {
            return GetTextView();
        }

        public IWpfTextView GetTextView()
        {
            var compService = ServiceProvider.GetService(typeof(SComponentModel)) as IComponentModel;
            Assumes.Present(compService);
            IVsEditorAdaptersFactoryService editorAdapter = compService.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapter.GetWpfTextView(GetCurrentNativeTextView());
        }

        public IVsTextView GetCurrentNativeTextView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GetService(typeof(SVsTextManager));
            Assumes.Present(textManager);
            IVsTextView activeView;
            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out activeView));
            return activeView;
        }


    }
}
