using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace WaywardEngine
{
	public class FormattedText
	{
		protected string text;
		protected TextBlock content;

		public FormattedText()
		{
			text = "";
			content = null;
		}
		public FormattedText( string text )
		{
			content = WaywardTextParser.ParseAsBlock(text);
			this.text = ParseBlockForTextContent(content);
		}
		public FormattedText( TextBlock block )
		{
			content = block;
			text = ParseBlockForTextContent(block);
		}
		public FormattedText( Inline inline )
		{
			text = ParseInline(inline);
			content = new TextBlock(inline);
		}

		public string GetText()
		{
			return text;
		}
		/// <summary>
		/// Creates and returns a deep copy of the content TextBlock.
		/// </summary>
		/// <returns></returns>
		public TextBlock GetContent()
		{
			TextBlock block = new TextBlock();
			block.Text = content.Text;

			Inline[] inlines = new Inline[content.Inlines.Count];
			content.Inlines.CopyTo(inlines, 0);

			block.Inlines.AddRange(inlines);

			return block;
		}

		/// <summary>
		/// Parses and sets the input text.
		/// WARNING: Will lose all previouse formatting.
		/// </summary>
		/// <param name="text">Input string</param>
		/// <returns></returns>
		public void SetText( string text )
		{
			content = WaywardTextParser.ParseAsBlock(text);
			this.text = ParseBlockForTextContent(content);
		}
		public void SetContent( TextBlock block )
		{
			content = block;
			text = ParseBlockForTextContent(block);
		}

		/// <summary>
		/// Recursively collects and returns a string that is the flattened combination of all text contained in the content's inlines.
		/// </summary>
		/// <param name="block">TextBlock to parse</param>
		/// <returns></returns>
		public static string ParseBlockForTextContent( TextBlock block )
		{
			string text = "";

			text += block.Text;
			foreach( Inline inline in block.Inlines ) {
				text += ParseInline(inline);
			}

			return text;
		}
		protected static string ParseInline( Inline inline )
		{
			string text = "";

			Type type = inline.GetType();
			if( type == typeof(Span) ) {
				Span span = inline as Span;
				foreach( Inline subInline in span.Inlines ) {
					text += ParseInline(subInline);
				}
			} else if( type == typeof(Run) ) {
				Run run = inline as Run;
				text += run.Text;
			}

			return text;
		}
	}
}
