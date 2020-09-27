using System;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class VerbosePage : WaywardEngine.ContentPage
    {
        Actor _observer;
        public Actor observer {
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

        public VerbosePage(Actor observer) : base()
        {
            _observer = observer;
            _observer.MessageVerbosePages += PrintMessage;
            _observer.TurnVerbosePages += TurnPage;
            _observer.DisplayVerbosePages += Display;

            SetTitle(". . .");
            FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("VerbosePage");
            AddContent(panel);

            descriptions = Utilities.FindNode<StackPanel>(panel, "Descriptions");

            Display();
        }

        public void Display()
        {
            if( subject == null || subject.container == null ) { return; }

            GameObject container = subject.container.GetParent();
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

        public void TurnPage(bool display)
        {
            descriptions.Children.Clear();
            if( display ) { Display(); }
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
