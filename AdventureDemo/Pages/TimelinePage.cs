using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
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
            observer.ObservedActionTaken += OnObservedActionTaken;

            SetTitle("Timeline");
            FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("OverviewEvents");
            AddContent(panel);

            events = Utilities.FindNode<StackPanel>(panel, "Events");
        }

        public override void Clear() {}

        public override void Update() {}

        protected void OnObservedActionTaken(Dictionary<string, object> data)
        {
            if( data.ContainsKey("message") ) {
                ObservableText observableText = data["message"] as ObservableText;
                TextBlock block = observableText.Observe(observer);
                events.Children.Insert(0, block);
            }
        }
    }
}
