using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXProjectMultiLang
{
    [Export(typeof(IIntellisensePresenterProvider))]
    [ContentType("text")]
    [Order(Before = "Default Completion Presenter")]
    [Name("Object Intellisense Presenter")]
    internal class IntellisensePresenterProvider : IIntellisensePresenterProvider
    {
        [Import(typeof(SVsServiceProvider))]
        IServiceProvider ServiceProvider { get; set; }
        #region Try Create Intellisense Presenter
        #region Documentation
        /// <summary>
        /// Inject the IntelliSense presenter
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        #endregion // Documentation
        public IIntellisensePresenter TryCreateIntellisensePresenter(IIntellisenseSession session)
        {
            #region Validation (is C#)
            const string CSHARP_CONTENT = "CSharp";
            if (session.TextView.TextBuffer.ContentType.TypeName != CSHARP_CONTENT)
            {
                return null;
            }
            #endregion // Validation
            ICompletionSession completionSession = session as ICompletionSession;
            if (completionSession != null)
            {
            }
            return null;
        }
        #endregion // Try Create Intellisense Presenter
    }
}
