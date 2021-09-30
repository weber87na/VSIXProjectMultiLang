using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Resources;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;

namespace VSIXProjectMultiLang
{
    static class TextSpanExtensions
    {
        /// <summary>
        /// Convert a <see cref="TextSpan"/> instance to a <see cref="TextSpan"/>.
        /// </summary>
        public static Span ToSpan(this TextSpan textSpan)
            => new Span(textSpan.Start, textSpan.Length);

        /// <summary>
        /// Add an offset to a <see cref="TextSpan"/>.
        /// </summary>
        public static TextSpan MoveTo(this TextSpan textSpan, int offset)
            => new TextSpan(textSpan.Start + offset, textSpan.Length);

        /// <summary>
        /// Convert a <see cref="TextSpan"/> to a <see cref="SnapshotSpan"/> on the given <see cref="ITextSnapshot"/> instance
        /// </summary>
        public static SnapshotSpan ToSnapshotSpan(this TextSpan textSpan, ITextSnapshot snapshot)
        {
            Debug.Assert(snapshot != null);
            var span = textSpan.ToSpan();
            return new SnapshotSpan(snapshot, span);
        }
    }








    /// <summary>
    /// TextAdornmentMultiLang places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class TextAdornmentMultiLang
    {
        /// <summary>
        /// The layer of the adornment.
        /// </summary>
        private readonly IAdornmentLayer layer;

        /// <summary>
        /// Text view where the adornment is created.
        /// </summary>
        private readonly IWpfTextView view;

        /// <summary>
        /// Adornment brush.
        /// </summary>
        private readonly Brush brush;

        /// <summary>
        /// Adornment pen.
        /// </summary>
        private readonly Pen pen;


        /// <summary>
        /// Initializes a new instance of the <see cref="TextAdornmentMultiLang"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public TextAdornmentMultiLang(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException( "view" );
            }

            this.layer = view.GetAdornmentLayer( "TextAdornmentMultiLang" );

            this.view = view;
            this.view.LayoutChanged += this.OnLayoutChanged;

            // Create the pen and brush to color the box behind the a's
            this.brush = new SolidColorBrush( Color.FromArgb( 0x20, 0x00, 0x00, 0xff ) );
            this.brush.Freeze( );

            var penBrush = new SolidColorBrush( Colors.Red );
            penBrush.Freeze( );
            this.pen = new Pen( penBrush, 0.5 );
            this.pen.Freeze( );
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (MyConfig.IsEnable == false) return;

            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                this.CreateVisuals( line );
            }
        }


        /// <summary>
        /// Adds the scarlet box behind the 'a' characters within the given line
        /// </summary>
        /// <param name="line">Line to add the adornments</param>
        private void CreateVisuals(ITextViewLine line)
        {
            IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;
            var currentLineText = line.Extent.GetText( );
            
            int find = -1;
            bool isFind = false;
            string strTranslate = "";
            foreach (var item in MyConfig.Dict.Keys)
            {
                isFind = currentLineText.IndexOf(item  ) > 0;
                if (isFind) {
                    find = currentLineText.IndexOf( item );
                    strTranslate = MyConfig.Dict[item];
                    break;
                };
            }
            
            if (find > -1)
            {
                SnapshotSpan span = new SnapshotSpan( this.view.TextSnapshot,
                    Span.FromBounds( line.Start + find, line.Start + find + 1 ) );
                DrawLang( textViewLines, span ,strTranslate);
            }

        }

        private void DrawLang(IWpfTextViewLineCollection textViewLines, SnapshotSpan span , string text)
        {
            Geometry geometry = textViewLines.GetMarkerGeometry( span );
            if (geometry != null)
            {
                var brush = new SolidColorBrush( MyConfig.Color );

                var textBlock = new TextBlock
                {
                    Text = text,
                    Foreground = brush,
                    FontSize = MyConfig.FontSize 
                };

                //設定多語系變數中文在左上角
                Canvas.SetLeft( textBlock, geometry.Bounds.Left );
                Canvas.SetTop( textBlock, geometry.Bounds.Top - MyConfig.FontSize );

                this.layer.AddAdornment( AdornmentPositioningBehavior.TextRelative, span, null, textBlock, null );
            }
        }
    }
}
