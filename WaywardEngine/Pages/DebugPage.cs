using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls;

namespace WaywardEngine
{
	public class DebugPage : Page
	{
		public bool visible = false;

		List<TextBlock> _log = new List<TextBlock>();
        StackPanel log;

		TextBox input;

		public DebugPage() 
			: base("DebugPage")
		{
			log = Utilities.FindNode<StackPanel>(element, "Log");
			input = Utilities.FindNode<TextBox>(element, "InputBox");

			ContextMenuHelper.AddContextMenuItem(element, "Close", CloseAction);
		}

		public override void Update()
		{
			foreach( TextBlock entry in _log ) {
				log.Children.Add(entry);
			}
		}
		public override void Clear() 
		{
			log.Children.Clear();
		}

		public void ClearLog()
		{
			_log.Clear();
		}

		public override bool CloseAction()
		{
			visible = false;
			return base.CloseAction();
		}

		public void DisplayMessage(string message, params WaywardTextParser.ParseDelegate[] spans)
		{
			DisplayMessage(WaywardTextParser.ParseAsBlock(message, spans));
		}
		public void DisplayMessage(TextBlock text)
		{
			_log.Add(text);
			if( visible ) {
				Clear();
				Update();
			}
		}
	}
}
