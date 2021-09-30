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
    public class PostfixCompletionCommandTarget : PostfixCompletionCommandTargetBase
    {
        private readonly ICompletionBroker _completionBroker;

        public PostfixCompletionCommandTarget(PostfixCompletionViewContext view, ICompletionBroker completionBroker)
            : base(view)
        {
            _completionBroker = completionBroker;
        }


        public override int Exec(
            ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (PackageGuids.guidVSIXProjectMultiLangPackageCmdSet != pguidCmdGroup ||
                PackageIds.cmdidPostfixVar != nCmdID)
            {
                return base.Exec( ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut );
            }

            // Get mnemonic content from editor.
            SnapshotPoint caretPosition = View.WpfView.Caret.Position.BufferPosition;
            ITextSnapshotLine line = View.WpfView.Caret.Position.BufferPosition.GetContainingLine( );
            string lineText = line.GetText( );
            //if (caretPosition.Position != line.End || lineText.Length < 3)
            //    return VSConstants.S_OK;

            string mnemonic = lineText.TrimStart( );
            string indent = new string( ' ', lineText.Length - mnemonic.Length );
            string snippet = string.Empty;
            int caretOffset = 0;
            bool isVar = lineText.EndsWith( ".var" );
            bool isReturn = lineText.EndsWith( ".return" );
            if (isVar)
            {
                caretOffset = snippet.Length + ".var".Length + 1 - mnemonic.Length;
                snippet = "var x = ";
                snippet += mnemonic.Replace( ".var", ";" );
                return CommitPostfixCompletion( ref caretPosition, line, mnemonic, snippet, caretOffset );
            }

            if (isReturn)
            {
                caretOffset = snippet.Length + ".return".Length + 1 - mnemonic.Length;
                snippet = "return ";
                snippet += mnemonic.Replace( ".return", ";" );
                return CommitPostfixCompletion( ref caretPosition, line, mnemonic, snippet, caretOffset );
            }


            return VSConstants.S_OK;
        }

        private int CommitPostfixCompletion(ref SnapshotPoint caretPosition, ITextSnapshotLine line, string mnemonic, string snippet, int caretOffset)
        {
            // Insert generated snippet into the current editor window
            int startPosition = line.End.Position - mnemonic.Length;
            Span targetPosition = new Span( startPosition, mnemonic.Length );
            View.CurrentBuffer.Replace( targetPosition, snippet );

            // Close all intellisense windows
            _completionBroker.DismissAllSessions( View.WpfView );

            // Move caret to the position where user can start typing new member name
            caretPosition = new SnapshotPoint(
                View.CurrentBuffer.CurrentSnapshot,
                caretPosition.Position + caretOffset );
            View.WpfView.Caret.MoveTo( caretPosition );

            return VSConstants.S_OK;
        }

        protected override OLECMDF GetCommandStatus(uint commandId)
        {
            if (PackageIds.cmdidPostfixVar == commandId &&
                (
                    //這兩個應該要一組才有辦法 trigger razor
                    ("CSharp" == View.WpfView.TextBuffer.ContentType.TypeName) ||
                    ("RazorCSharp" == View.WpfView.TextBuffer.ContentType.TypeName))
                )
            {
                return OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED;
            }
            else
            {
                return OLECMDF.OLECMDF_INVISIBLE;
            }
        }
    }



}
