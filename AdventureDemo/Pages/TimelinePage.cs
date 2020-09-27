using System;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class TimelinePage : WaywardEngine.ContentPage
    {
        Actor _observer;
        public Actor observer {
            get {
                return _observer;
            }
        }

        StackPanel events;

        public TimelinePage(Actor observer) : base()
        {
            _observer = observer;

            SetTitle("Timeline");
            FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("OverviewEvents");
            AddContent(panel);

            events = Utilities.FindNode<StackPanel>(panel, "Events");
        }

        public override void Clear()
        {
            events.Children.Clear();
        }

        public override void Update() {}
    }
}
