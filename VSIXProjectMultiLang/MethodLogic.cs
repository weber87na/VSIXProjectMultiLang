using EnvDTE;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VSIXProjectMultiLang
{
class NumberToWords  
{  
    private static String[] units = { "Zero", "One", "Two", "Three",  
    "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",  
    "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",  
    "Seventeen", "Eighteen", "Nineteen" };  
    private static String[] tens = { "", "", "Twenty", "Thirty", "Forty",  
    "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };  
  
    public static String ConvertAmount(double amount)  
    {  
        try  
        {  
            Int64 amount_int = (Int64)amount;  
            Int64 amount_dec = (Int64)Math.Round((amount - (double)(amount_int)) * 100);  
            if (amount_dec == 0)  
            {  
                return Convert(amount_int) + " Only.";  
            }  
            else  
            {  
                return Convert(amount_int) + " Point " + Convert(amount_dec) + " Only.";  
            }  
        }  
        catch (Exception e)  
        {  
            // TODO: handle exception  
        }  
        return "";  
    }  
  
    public static String Convert(Int64 i)  
    {  
        if (i < 20)  
        {  
            return units[i];  
        }  
        if (i < 100)  
        {  
            return tens[i / 10] + ((i % 10 > 0) ? " " + Convert(i % 10) : "");  
        }  
        if (i < 1000)  
        {  
            return units[i / 100] + " Hundred"  
                    + ((i % 100 > 0) ? " And " + Convert(i % 100) : "");  
        }  
        if (i < 100000)  
        {           return Convert(i / 1000) + " Thousand "  
                    + ((i % 1000 > 0) ? " " + Convert(i % 1000) : "");  
        }  
        if (i < 10000000)  
        {  
            return Convert(i / 100000) + " Lakh "  
                    + ((i % 100000 > 0) ? " " + Convert(i % 100000) : "");  
        }  
        if (i < 1000000000)  
        {  
            return Convert(i / 10000000) + " Crore "  
                    + ((i % 10000000 > 0) ? " " + Convert(i % 10000000) : "");  
        }  
        return Convert(i / 1000000000) + " Arab "  
                + ((i % 1000000000 > 0) ? " " + Convert(i % 1000000000) : "");  
    }  
} 



    public class MethodLogic
    {
        public void AddJsonPropertyName(IWpfTextView wpfTextView, DTE dte)
        {
            var span = wpfTextView.Caret.ContainingTextViewLine.Extent;
            var tree = CSharpSyntaxTree.ParseText( wpfTextView.TextSnapshot.GetText( ) );
            var root = tree.GetRoot( );


            // Get mnemonic content from editor.
            SnapshotPoint caretPosition = wpfTextView.Caret.Position.BufferPosition;
            ITextSnapshotLine line = wpfTextView.Caret.Position.BufferPosition.GetContainingLine( );
            string currentLineText = line.GetText( );


            var  currentTree = CSharpSyntaxTree.ParseText(currentLineText);
            var mem = currentTree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>( );
            var prop = mem.FirstOrDefault( ) as PropertyDeclarationSyntax;
            if (prop != null)
            {
                var attrName = SyntaxFactory.ParseName( "JsonPropertyName" );
                var arguments = SyntaxFactory.ParseAttributeArgumentList( $"(\"{prop.Identifier.Text}\")" );
                var attribute = SyntaxFactory.Attribute( attrName, arguments ); //MyAttribute("some_param")

                var attributeList = new SeparatedSyntaxList<AttributeSyntax>( );
                attributeList = attributeList.Add( attribute );
                //[MyAttribute("some_param")]
                var list = SyntaxFactory.AttributeList( attributeList );
                var newP = prop.AddAttributeLists( list );
                Console.WriteLine(newP);


                var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                //textEdit.Insert( line.Start.Position - 1, Environment.NewLine );
                textEdit.Insert( line.Start.Position - 1, Environment.NewLine + list.ToString( ) );
                textEdit.Apply( );

                dte.ExecuteCommand( "Edit.FormatDocument" );
            }

        }

        //借鏡 emacs 大師
        //https://github.com/redguardtoo/vscode-matchit/blob/master/src/extension.ts
        //注意不能直接 mapping 原本的 key %
        //需要用 leader + % 才有用
        public void Matchit(IWpfTextView wpfTextView, DTE dte)
        {
            var span = wpfTextView.Caret.ContainingTextViewLine.Extent;
            var tree = CSharpSyntaxTree.ParseText( wpfTextView.TextSnapshot.GetText( ) );
            var root = tree.GetRoot( );


            // Get mnemonic content from editor.
            SnapshotPoint caretPosition = wpfTextView.Caret.Position.BufferPosition;
            ITextSnapshotLine line = wpfTextView.Caret.Position.BufferPosition.GetContainingLine( );
            string lineText = line.GetText( );

            if ("{}[]()".IndexOf( caretPosition.GetChar( ) ) >= 0)
            {
                dte.ExecuteCommand( "Edit.GotoBrace" );
                return;
            }

            void Vat()
            {
                System.Windows.Forms.SendKeys.SendWait( "vat" );
            }


            Regex regex = new Regex( @"^[ \t]*(<[A-Za-z]|<\/[A-Za-z][a-zA-Z0-9]*)" );
            var isMatch = regex.IsMatch( lineText );

            if (isMatch)
            {
                //目前只有多行有用
                //確認是否為關閉標籤
                if (lineText.Trim( )[1].ToString( ) == "/")
                {
                    //標籤尾
                    Vat( );
                    System.Windows.Forms.SendKeys.Send( "o" );
                    System.Windows.Forms.SendKeys.Send( "{Esc}" );
                }
                else
                {
                    //標籤頭
                    Vat( );
                    System.Windows.Forms.SendKeys.Send( "{Esc}" );
                }

            }
        }


        public void MoveToEnd(IWpfTextView wpfTextView)
        {
            var span = wpfTextView.Caret.ContainingTextViewLine.Extent;
            var tree = CSharpSyntaxTree.ParseText( wpfTextView.TextSnapshot.GetText( ) );
            var members = tree.GetRoot( ).DescendantNodes( ).OfType<MemberDeclarationSyntax>( );
            //SnapshotSpan span = new SnapshotSpan( this.wpfTextView.TextSnapshot,
            //    Span.FromBounds( line.Start + find, line.Start + find + 1 ) );
            foreach (var member in members)
            {
                var method = member as MethodDeclarationSyntax;
                if (method != null)
                {
                    if (method.HasLeadingTrivia)
                    {
                        var methodSpan = method.FullSpan.ToSpan( );
                        var isContains = methodSpan.Contains( span );
                        if (isContains)
                        {
                            int spanStart = method.Body.CloseBraceToken.SpanStart;
                            //Debug.WriteLine( "IN METHOD:" );
                            //Debug.WriteLine(method.GetLeadingTrivia().Span);
                            //Debug.WriteLine( "SpanStart:" + spanStart);
                            //Debug.WriteLine("Method: " + method.Identifier);

                            SnapshotPoint sp = new SnapshotPoint( wpfTextView.TextSnapshot, spanStart + 1 );
                            wpfTextView.Caret.MoveTo( sp );
                        }
                    }
                }
            }
        }

        public void MoveToBegin(IWpfTextView wpfTextView)
        {
            var span = wpfTextView.Caret.ContainingTextViewLine.Extent;
            var tree = CSharpSyntaxTree.ParseText( wpfTextView.TextSnapshot.GetText( ) );
            var members = tree.GetRoot( ).DescendantNodes( ).OfType<MemberDeclarationSyntax>( );
            //SnapshotSpan span = new SnapshotSpan( this.wpfTextView.TextSnapshot,
            //    Span.FromBounds( line.Start + find, line.Start + find + 1 ) );
            foreach (var member in members)
            {
                var method = member as MethodDeclarationSyntax;
                if (method != null)
                {
                    if (method.HasLeadingTrivia)
                    {
                        var methodSpan = method.FullSpan.ToSpan( );
                        var isContains = methodSpan.Contains( span );
                        if (isContains)
                        {
                            int moveTo = method.Identifier.Span.Start;
                            SnapshotPoint sp = new SnapshotPoint( wpfTextView.TextSnapshot, moveTo );
                            wpfTextView.Caret.MoveTo( sp );

                            //var findFirstText = method.ToFullString( );
                            //int index = 0;
                            //foreach (var c in findFirstText)
                            //{
                            //    //防止開頭是空白 tab 換行
                            //    if (char.IsWhiteSpace( c ) || c == '\t' || c == '\n')
                            //        index = index + 1;
                            //    else
                            //        break;
                            //}
                            //int moveTo = methodSpan.Start + index;
                            //SnapshotPoint sp = new SnapshotPoint( wpfTextView.TextSnapshot, moveTo );
                            //wpfTextView.Caret.MoveTo( sp );


                            //var findFirstText = method.ToFullString( );
                            //int index = 0;
                            //foreach (var c in findFirstText)
                            //{
                            //    //防止開頭是空白 tab 換行
                            //    if (char.IsWhiteSpace( c ) || c == '\t' || c == '\n')
                            //        index = index + 1;
                            //    else
                            //        break;
                            //}
                            //int moveTo = methodSpan.Start + index;
                            //SnapshotPoint sp = new SnapshotPoint( wpfTextView.TextSnapshot, moveTo );
                            //wpfTextView.Caret.MoveTo( sp );

                            //if(method.Modifiers.Count > 0)
                            //{
                            //    int spanStart = method.Identifier.SpanStart;
                            //    SnapshotPoint sp = new SnapshotPoint( wpfTextView.TextSnapshot, spanStart );
                            //    wpfTextView.Caret.MoveTo( sp );
                            //}
                            //else
                            //{
                            //}
                        }
                    }
                }
            }
        }


        public void MoveToIfBegin(IWpfTextView wpfTextView)
        {
            var span = wpfTextView.Caret.ContainingTextViewLine.Extent;
            var tree = CSharpSyntaxTree.ParseText( wpfTextView.TextSnapshot.GetText( ) );
            var members = tree.GetRoot( ).DescendantNodes( ).OfType<IfStatementSyntax>( );
            //SnapshotSpan span = new SnapshotSpan( this.wpfTextView.TextSnapshot,
            //    Span.FromBounds( line.Start + find, line.Start + find + 1 ) );
            foreach (var member in members)
            {
                var ifStatementSyntax = member as IfStatementSyntax;
                if (ifStatementSyntax != null)
                {
                    if (ifStatementSyntax.HasLeadingTrivia)
                    {
                        var methodSpan = ifStatementSyntax.FullSpan.ToSpan( );
                        var isContains = methodSpan.Contains( span );
                        if (isContains)
                        {
                            int moveTo = ifStatementSyntax.Condition.Span.Start;
                            SnapshotPoint sp = new SnapshotPoint( wpfTextView.TextSnapshot, moveTo );
                            wpfTextView.Caret.MoveTo( sp );
                        }
                    }
                }
            }
        }





        public void SelectCurrentMethod(IWpfTextView wpfTextView)
        {
            var span = wpfTextView.Caret.ContainingTextViewLine.Extent;
            var tree = CSharpSyntaxTree.ParseText( wpfTextView.TextSnapshot.GetText( ) );
            var members = tree.GetRoot( ).DescendantNodes( ).OfType<MemberDeclarationSyntax>( );
            //SnapshotSpan span = new SnapshotSpan( this.wpfTextView.TextSnapshot,
            //    Span.FromBounds( line.Start + find, line.Start + find + 1 ) );
            foreach (var member in members)
            {
                var method = member as MethodDeclarationSyntax;
                if (method != null)
                {
                    if (method.HasLeadingTrivia)
                    {
                        var methodSpan = method.FullSpan.ToSpan( );
                        var isContains = methodSpan.Contains( span );
                        if (isContains)
                        {
                            var selectFullMethod = method.FullSpan.ToSnapshotSpan( wpfTextView.TextSnapshot );
                            wpfTextView.Selection.Select( selectFullMethod, false );
                        }
                    }
                }
            }
        }

        public void GoToFile(IWpfTextView wpfTextView , DTE dte)
        {
            var spans = wpfTextView.Selection.SelectedSpans;
            foreach (var span in wpfTextView.Selection.SelectedSpans)
            {
                string path = span.GetText( );

                //打開目前絕對路徑的檔案
                if(File.Exists(path) == true) dte.ItemOperations.OpenFile( path , Constants.vsViewKindAny );

                //mvc root
                if(path.StartsWith( "~/" ))
                {
                    var slnDir = Path.GetDirectoryName(dte.Solution.FullName);
                    string currentDocumentPath = dte.ActiveDocument.FullName;
                    string substractPath = currentDocumentPath.Replace( slnDir, "" );
                    var match = Regex.Match(substractPath, @"^(\\)(?<first>[\w\-]+)");
                    var first = match.Groups["first"];
                    string dirName = Path.GetDirectoryName( currentDocumentPath );
                    path = path.Replace( "~/", "" );
                    path = path.Replace( @"/", @"\" );
                    var result = Path.Combine(slnDir, first.Value, path);

                    if(File.Exists(result) == true) 
                        dte.ItemOperations.OpenFile( result , Constants.vsViewKindAny );
                }

                //取得目前資料夾位置
                if(path.StartsWith( "./" ))
                {
                    string currentDocumentPath = dte.ActiveDocument.FullName;
                    string dirName = Path.GetDirectoryName( currentDocumentPath );
                    path = path.Replace( "./", "" );
                    path = path.Replace( @"/", @"\" );
                    var result = Path.Combine( dirName, path);

                    if(File.Exists(result) == true) 
                        dte.ItemOperations.OpenFile( result , Constants.vsViewKindAny );
                }

                //相對
                if (path.StartsWith( "../" ))
                {
                    string currentDocumentPath = dte.ActiveDocument.FullName;
                    string dirName = Path.GetDirectoryName( currentDocumentPath );
                    var result = Path.Combine( dirName, path);

                    if(File.Exists(result) == true) 
                        dte.ItemOperations.OpenFile( result , Constants.vsViewKindAny );
                }
            }
        }

        public void Toggle(IWpfTextView wpfTextView)
        {
            var spans = wpfTextView.Selection.SelectedSpans;
            foreach (var span in wpfTextView.Selection.SelectedSpans)
            {
                Debug.WriteLine( span.GetText( ) );
                if (span.GetText( ) == "0")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "1" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "1")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "0" );
                    textEdit.Apply( );
                    continue;
                }


                if (span.GetText( ) == "public")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "private" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "private")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "public" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "true")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "false" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "false")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "true" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "close")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "open" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "open")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "close" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "on")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "off" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "off")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "on" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "真")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "假" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "假")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "真" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "開")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "關" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "關")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "開" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "正")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "反" );
                    textEdit.Apply( );
                    continue;
                }

                if (span.GetText( ) == "反")
                {
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, "正" );
                    textEdit.Apply( );
                    continue;
                }



            }
        }

        public void ToBool(IWpfTextView wpfTextView)
        {
            To( wpfTextView, "bool" );
        }

        public void ToInt(IWpfTextView wpfTextView)
        {
            To( wpfTextView, "int" );
        }

        public void ToDecimal(IWpfTextView wpfTextView)
        {
            To( wpfTextView, "decimal" );
        }

        public void ToLong(IWpfTextView wpfTextView)
        {
            To( wpfTextView, "long" );
        }

        public void ToDouble(IWpfTextView wpfTextView)
        {
            To( wpfTextView, "double" );
        }
        public void ToVar(IWpfTextView wpfTextView)
        {
            To( wpfTextView, "var" );
        }

        public void ToString(IWpfTextView wpfTextView)
        {
            To( wpfTextView, "string" );
        }

        private void To(IWpfTextView wpfTextView, string to)
        {
            var spans = wpfTextView.Selection.SelectedSpans;
            foreach (var span in wpfTextView.Selection.SelectedSpans)
            {
                var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                textEdit.Replace( span, to );
                textEdit.Apply( );
            }
        }

        public void ToEng(IWpfTextView wpfTextView)
        {
            var spans = wpfTextView.Selection.SelectedSpans;
            foreach (var span in wpfTextView.Selection.SelectedSpans)
            {
                var text = span.GetText( );
                if (Microsoft.VisualBasic.Information.IsNumeric(text))
                {
                    string result = NumberToWords.Convert(Convert.ToInt64( text ));
                    var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                    textEdit.Replace( span, result );
                    textEdit.Apply( );
                }
            }

        }


        public void ChineseAdd(IWpfTextView wpfTextView)
        {
            var spans = wpfTextView.Selection.SelectedSpans;
            foreach (var span in wpfTextView.Selection.SelectedSpans)
            {
                var text = span.GetText( );
                int number = ConvChineseInt.ConvChineseNumber( text );
                number += 1;
                string result = ConvChineseInt.NumToChinese( number.ToString( ) );
                var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                textEdit.Replace( span, result);
                textEdit.Apply( );
            }

        }


        public void ChineseMinus(IWpfTextView wpfTextView)
        {
            var spans = wpfTextView.Selection.SelectedSpans;
            foreach (var span in wpfTextView.Selection.SelectedSpans)
            {
                var text = span.GetText( );
                int number = ConvChineseInt.ConvChineseNumber( text );
                number += 1;
                string result = ConvChineseInt.NumToChinese( number.ToString( ) );
                var textEdit = wpfTextView.TextBuffer.CreateEdit( );
                textEdit.Replace( span, result);
                textEdit.Apply( );
            }
        }
    }
}
