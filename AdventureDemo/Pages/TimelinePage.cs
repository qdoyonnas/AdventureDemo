using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    public class TimelinePage : WaywardEngine.ContentPage
    {
        PlayerActor _observer;
        public PlayerActor observer {
            get {
                return _observer;
            }
        }

        StackPanel events;

        public TimelinePage(PlayerActor observer) : base()
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
            if (data == null) { return; }

            FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
            events.Children.Add(entry);

            TextBlock data1 = WaywardEngine.Utilities.FindNode<TextBlock>(entry, "Data1");
            if( data.ContainsKey("gameObject") ) {
                try {
                    GameObject obj = data["gameObject"] as GameObject;
                    data1.Inlines.Add( observer.Observe(obj, "name upper top").span );
                } catch( Exception e ) {
                    WaywardManager.instance.Log($@"<orange>WARNING: TimelinePage failed to resolve gameObject: {e}</orange>");
                    data1.Inlines.Add( WaywardTextParser.Parse($"<i>unknown</i>") );
                }
            } else {
                data1.Inlines.Add( WaywardTextParser.Parse($"<i>unknown</i>") );
            }

            TextBlock data2 = WaywardEngine.Utilities.FindNode<TextBlock>(entry, "Data2");
            if( data.ContainsKey("label") ) {
                try {
                    string label = data["label"] as string;
                    data2.Inlines.Add( WaywardTextParser.Parse(label) );
                } catch( Exception e ) {
                    WaywardManager.instance.Log($@"<orange>WARNING: TimelinePage failed to resolve label: {e}</orange>");
                    data2.Inlines.Add( WaywardTextParser.Parse($"<i>unknown</i>") );
                }
            } else {
                data2.Inlines.Add( WaywardTextParser.Parse($"<i>unknown</i>") );
            }

            TextBlock data3 = WaywardEngine.Utilities.FindNode<TextBlock>(entry, "Data3");
            if( data.ContainsKey("target") ) {
                try {
                    GameObject target = data["target"] as GameObject;
                    data3.Inlines.Add( observer.Observe(target, "name upper").span );
                } catch( Exception e ) {
                    WaywardManager.instance.Log($@"<orange>WARNING: TimelinePage failed to resolve target: {e}</orange>");
                }
            } 
        }
    }
}
