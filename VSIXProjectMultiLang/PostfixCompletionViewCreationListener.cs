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
    /// View creation listener that is responsible for injecting command filters.
    /// </summary>
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("CSharp")]
    [ContentType("RazorCSharp")]
    [TextViewRole( PredefinedTextViewRoles.Editable )]
    public class PostfixCompletionViewCreationListener : IVsTextViewCreationListener
    {
        /// <summary>
        /// Abbreviation prefix for JSX files, required to avoid collision with JavaScript intellisense.
        /// </summary>
        //public const string JsxPrefix = "<";

        /// <summary>
        /// Gets or sets the editor adapters factory service, injected through MEF.
        /// </summary>
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        /// <summary>
        /// Gets or sets the completion broker, injected through MEF.
        /// </summary>
        [Import]
        public ICompletionBroker CompletionBroker { get; set; }

        /// <summary>
        /// Called when a IVsTextView adapter has been created and initialized.
        /// </summary>
        /// <param name="textViewAdapter">The newly created and initialized text view adapter.</param>
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            IWpfTextView textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            PostfixCompletionViewContext context = new PostfixCompletionViewContext(textView, textViewAdapter);
            string contentType = textView.TextBuffer.ContentType.TypeName;

            // As of v2019 Visual Studio does not use projection buffer for JSX files and thus we cannot
            // detect JS and HTML buffers. So, in order to prevent unintended JS and Emmet snippets
            // collisions we require prefix for emmet abbreviations to work correctly.
            //if (contentType.EndsWith("script", StringComparison.InvariantCultureIgnoreCase))
            //    context.AbbreviationPrefix = JsxPrefix;

            textView.Properties.GetOrCreateSingletonProperty(
                "PostfixCompletionCommandTarget",
                () => new PostfixCompletionCommandTarget(context, CompletionBroker));
        }
    }



}
