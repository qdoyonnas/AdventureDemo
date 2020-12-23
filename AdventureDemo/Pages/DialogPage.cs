using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
	class DialogPage : WaywardEngine.ContentPage
	{
		StackPanel optionsPanel = null;
		TextBox inputBox = null;

		public DialogPage() : base()
        {
			SetTitle("Dialogue"); // This should always be replaced by the calling object
        }

		public void AddOptionsPanel()
		{
			FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("DialogPanel");
            AddContent(panel);

			optionsPanel = WaywardEngine.Utilities.FindNode<StackPanel>(panel, "Options");
		}

		public delegate void InputActionDelegate(string input);
		public void AddInputPanel(InputActionDelegate action)
		{
			FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("DialogInput");
			AddContent(panel);

			inputBox = WaywardEngine.Utilities.FindNode<TextBox>(panel, "InputBox");
			inputBox.KeyDown += (sender, e) => {
				if( e.Key != Key.Return ) { return; }

				action(inputBox.Text);
				CloseAction();
			};
		}

		public delegate void EntryActionDelegate();
		public void AddEntry(string text, EntryActionDelegate action)
		{
			if( optionsPanel == null ) {
				AddOptionsPanel();
			}

			FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("DialogEntry");
			optionsPanel.Children.Add(entry);

			TextBlock entryText = WaywardEngine.Utilities.FindNode<TextBlock>(entry, "Text");
			entryText.Text = text;

			entry.MouseLeftButtonUp += delegate { action(); CloseAction(); };
		}

		public override void Clear()
		{
		}

		public override void Update()
		{
		}
	}
}
