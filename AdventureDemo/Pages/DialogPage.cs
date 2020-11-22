using System;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
	class DialogPage : WaywardEngine.ContentPage
	{
		public DialogPage() : base()
        {
			SetTitle("Wait Time");

            FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("DialogPanel");
            AddContent(panel);
        }

		public override void Clear()
		{
		}

		public override void Update()
		{
		}
	}
}
