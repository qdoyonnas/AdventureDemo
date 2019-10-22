using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;

namespace WaywardEngine
{
    public static class WaywardTextParser
    {
        public delegate Span ParseDelegate();
        /// <summary>
        /// Parse a marked-up string into stylized text. Returned as a TextBlock.
        /// Format: <i>...</i> for italics <red></red> for color and [0] to insert passed in spans.
        /// NOTE: 'Consumes' the Spans, multiple distinct instances of a Span must be provided if located in multiple
        /// positions. 
        /// </summary>
        /// <param name="text">String to be parsed.</param>
        /// <param name="spans">Additional spans to be swapped in.</param>
        /// <returns></returns>
        public static TextBlock ParseAsBlock( string text, params ParseDelegate[] spans )
        {
            return new TextBlock( Parse(text, spans) );
        }
        /// <summary>
        /// Parse a marked-up string into stylized text.
        /// Format: <i>...</i> for italics <red></red> for color and [0] to insert passed in spans.
        /// NOTE: 'Consumes' the Spans, multiple distinct instances of a Span must be provided if located in multiple
        /// positions. 
        /// </summary>
        /// <param name="text">String to be parsed.</param>
        /// <param name="spans">Additional spans to be swapped in.</param>
        /// <returns></returns>
        public static Span Parse( string text, params ParseDelegate[] spans )
        {
            Regex parser = new Regex(@"(?'open'<(?'id'\w+)>)(?!</\k'id'>).*(?'content-open'</\k'id'>)");
            MatchCollection matches = parser.Matches(text);

            Span span = new Span();
            int index = 0;
            foreach( Match match in matches ) {
                int lengthOfContent = match.Index - index;
                string preContent = text.Substring(index, lengthOfContent);
                span.Inlines.Add( SubstituteSpans(preContent, spans) );

                Span internalSpan = GetSpan(match.Groups["id"].Value);
                internalSpan.Inlines.Add( Parse(match.Groups["content"].Value, spans) );
                span.Inlines.Add(internalSpan);

                index = match.Index + match.Length;
            }

            string postContent = text.Substring(index);
            span.Inlines.Add( SubstituteSpans(postContent, spans) );

            return span;
        }

        private static Span GetSpan( string id )
        {
            switch( id ) {
                case "i":
                    return new Italic();
                case "b":
                    return new Bold();
                case "u":
                    return new Underline();
                default:
                    return ColorSpan(id);
            }
        }
        private static Span ColorSpan( string id )
        {
            Span span = new Span();

            switch( id.ToLower() ) {
                case "red":
                    span.Foreground = Brushes.Red;
                    break;
                case "green":
                    span.Foreground = Brushes.Green;
                    break;
                case "blue":
                    span.Foreground = Brushes.Blue;
                    break;
                case "yellow":
                    span.Foreground = Brushes.Yellow;
                    break;
                case "purple":
                    span.Foreground = Brushes.Purple;
                    break;
                case "brown":
                    span.Foreground = Brushes.Brown;
                    break;
                case "black":
                    span.Foreground = Brushes.Black;
                    break;
                case "gray":
                    span.Foreground = Brushes.Gray;
                    break;
                case "white":
                    span.Foreground = Brushes.White;
                    break;
            }

            return span;
        }

        /// <summary>
        /// Scans the input for '[#]' patterns replacing them with the matching index from the passed in Spans.
        /// NOTE: 'Consumes' the Spans, multiple distinct instances of a Span must be provided if located in multiple
        /// positions.
        /// </summary>
        /// <param name="text">String to be parsed.</param>
        /// <param name="spans">Additional spans to be swapped in.</param>
        /// <returns></returns>
        private static Span SubstituteSpans( string text, ParseDelegate[] spans )
        {
            Regex parser = new Regex(@"\[(?'index'[0-9]+)\]");
            MatchCollection matches = parser.Matches(text);

            Span span = new Span();
            int index = 0;
            foreach( Match match in matches ) {
                int lengthOfContent = match.Index - index;
                string preContent = text.Substring(index, lengthOfContent);
                span.Inlines.Add( preContent );
                
                int spanIndex;
                if( int.TryParse(match.Groups["index"].Value, out spanIndex) && spanIndex < spans.Length ) {
                    span.Inlines.Add( spans[spanIndex]() ); // XXX: Will retarget the span removing it fom other locations
                                                            //  For this to work Spans must be deep copied, recursively
                }

                index = match.Index + match.Length;
            }

            string postContent = text.Substring(index);
            span.Inlines.Add( postContent );

            return span;
        }
    }
}
