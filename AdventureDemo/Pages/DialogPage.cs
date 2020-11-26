using System;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
	class DialogPage : WaywardEngine.ContentPage
	{
		StackPanel optionsPanel;

		public DialogPage() : base()
        {
			SetTitle("Dialogue"); // This should always be replaced by the calling object

            FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("DialogPanel");
            AddContent(panel);

			optionsPanel = WaywardEngine.Utilities.FindNode<StackPanel>(panel, "Options");
        }

		public delegate void EntryActionDelegate();
		public void AddEntry(string text, EntryActionDelegate action)
		{
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
