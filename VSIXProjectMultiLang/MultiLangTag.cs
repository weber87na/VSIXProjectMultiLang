using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VSIXProjectMultiLang
{
    internal class MultiLangTag : SpaceNegotiatingAdornmentTag
    {
        public MultiLangTag(double width, double topSpace, double baseline, double textHeight, double bottomSpace, Microsoft.VisualStudio.Text.PositionAffinity affinity, object identityTag, object providerTag) : base( width, topSpace, baseline, textHeight, bottomSpace, affinity, identityTag, providerTag )
        {
        }
    }
    internal sealed class MultiLangTagger : ITagger<MultiLangTag>
    {
        public MultiLangTagger()
        {
        }
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<MultiLangTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (MyConfig.IsEnable == true)
            {
                foreach (var span in spans)
                {
                    var currentLineText = span.GetText( );
                    int find = -1;
                    bool isFind = false;
                    string strTranslate = "";
                    foreach (var item in MyConfig.Dict.Keys)
                    {
                        isFind = currentLineText.IndexOf( item ) > 0;
                        if (isFind)
                        {
                            find = currentLineText.IndexOf( item );
                            strTranslate = MyConfig.Dict[item];
                            break;
                        };
                    }

                    if (find > -1)
                    {
                        //SnapshotSpan span = new SnapshotSpan( this.view.TextSnapshot,
                        //    Span.FromBounds( line.Start + find, line.Start + find + 1 ) );
                        //DrawHI( textViewLines, span ,strTranslate);

                        yield return (ITagSpan<MultiLangTag>)new TagSpan<MultiLangTag>(
                            new SnapshotSpan( span.Snapshot.TextBuffer.CurrentSnapshot, (Span)span ),
                            //1.2是邊距
                            new MultiLangTag( 0.0, (double)MyConfig.FontSize * 1.2, 0.0, 0.0, 0.0, PositionAffinity.Predecessor, (object)null, (object)null ) );
                    }
                }

            }
        }
    }

    [Export( typeof( IViewTaggerProvider ) )]
    [ContentType( "cshtml" )]
    [ContentType( "RazorCSharp" )]
    [ContentType( "csharp" )]
    [TextViewRole( "DOCUMENT" )]
    [TagType( typeof( SpaceNegotiatingAdornmentTag ) )]
    [TagType( typeof( MultiLangTag ) )]
    internal partial class TestTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
                return null;

            //provide highlighting only on the top-level buffer
            if (textView.TextBuffer != buffer)
                return null;

            return new MultiLangTagger( ) as ITagger<T>;

        }
    }

}
