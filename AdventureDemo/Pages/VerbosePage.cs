using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    public class VerbosePage : WaywardEngine.ContentPage
    {
        PlayerActor _observer;
        public PlayerActor observer {
            get {
                return _observer;
            }
        }

        public bool secondPerson {
            get {
                return observer == GameManager.instance.world.player;
            }
        }
        public GameObject subject {
            get {
                return observer.GetControlled();
            }
        }

        StackPanel descriptions;

        public VerbosePage(PlayerActor observer) : base()
        {
            _observer = observer;
            observer.ObservedActionTaken += OnObservedActionTaken;
            //TimelineManager.instance.onTimeAdvanceStart += TurnPage;

            SetTitle(". . .");
            FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("VerbosePage");
            AddContent(panel);

            descriptions = Utilities.FindNode<StackPanel>(panel, "Descriptions");

            Display();
        }

        public void Display()
        {
            if( subject == null || subject.attachPoint == null ) { return; }

            GameObject container = subject.attachPoint.GetParent();
            if( container != null ) {
                DisplayContainer(container);
            }
        }

        private void DisplayContainer(GameObject obj)
        {
            GameObjectData objData = observer.Observe(obj);

            bool unique = Char.IsUpper(objData.text[0]);

            TextBlock text = WaywardTextParser.ParseAsBlock(
                $@"[0] {(secondPerson ? "are" : "is")} in {(unique ? "" : "a ")}[1], [2].",
                    () => { return observer.Observe(subject).span; },
                    () => { return objData.span; },
                    () => { return observer.Observe(obj, "description").span; }
            );

            text.TextWrapping = TextWrapping.Wrap;
            descriptions.Children.Add(text);
        }

        protected void OnObservedActionTaken( Dictionary<string, object> data )
        {
            if (data == null) { return; }

            if( data.ContainsKey("message") ) {
                ObservableText observableText = data["message"] as ObservableText;
                if (observableText == null) {
                    WaywardManager.instance.Log($"<red>VerbosePage found none ObservableText message:</red> {data["message"]}");
                    return;
                }
                TextBlock block = observableText.Observed(observer);
                descriptions.Children.Add(block);
            }

            if( data.ContainsKey("displayAfter") && (bool)data["displayAfter"] ) {
                Display();
            }
        }

        public void TurnPage()
        {
            descriptions.Children.Clear();
        }

        public override void Clear() {}

        public override void Update() {}

        public void PrintMessage(string message)
        {
            TextBlock text = WaywardTextParser.ParseAsBlock(message);
            descriptions.Children.Add(text);
        }
        public void PrintMessage(TextBlock text)
        {
            descriptions.Children.Add(text);
        }
    }
}
