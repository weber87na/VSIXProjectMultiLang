using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXProjectMultiLang
{
    /// <summary>
    /// Base class for Emmet command targets.
    /// </summary>
    public abstract class PostfixCompletionCommandTargetBase : IOleCommandTarget
    {
        private bool _reloadedWithHighPriority = false;

        private IOleCommandTarget _nextTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostfixCompletionCommandTargetBase"/> class.
        /// </summary>
        /// <param name="view">Context of the view to operate on.</param>
        public PostfixCompletionCommandTargetBase(PostfixCompletionViewContext view)
        {
            View = view;
            View.TextView.AddCommandFilter(this, out _nextTarget);
        }

        protected PostfixCompletionViewContext View { get; private set; }

        protected IOleCommandTarget NextTarget
        {
            get { return _nextTarget; }
        }

        public virtual int Exec(
            ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            // Put this command target instance at the end of the chain in order to be able to handle TAB key
            // before the intellisense system.
            if (!_reloadedWithHighPriority && (uint)VSConstants.VSStd2KCmdID.TYPECHAR == nCmdID)
            {
                int retVal = _nextTarget.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                _reloadedWithHighPriority = true;
                View.TextView.RemoveCommandFilter(this);
                View.TextView.AddCommandFilter(this, out _nextTarget);

                return retVal;
            }

            return NextTarget.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup != PackageGuids.guidVSIXProjectMultiLangPackageCmdSet)
                return NextTarget.QueryStatus( ref pguidCmdGroup, cCmds, prgCmds, pCmdText );

            for (uint i = 0; i < cCmds; i++)
                prgCmds[i].cmdf = (uint)GetCommandStatus(prgCmds[i].cmdID);

            return VSConstants.S_OK;
        }

        protected virtual OLECMDF GetCommandStatus(uint commandId)
        {
            return OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED;
        }
    }



}
