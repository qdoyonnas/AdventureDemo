using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        bool _secondPerson = false;
        public bool secondPerson {
            get {
                return _secondPerson;
            }
        }

        GameObject _subject;
        public GameObject subject {
            get {
                return _subject;
            }
            set {
                _subject = value;
                Update();
            }
        }

        StackPanel descriptions;

        public VerbosePage(Actor observer, GameObject subject = null) : base()
        {
            _observer = observer;
            _subject = subject;

            SetTitle(". . .");
            FrameworkElement panel = GameManager.instance.GetResource<FrameworkElement>("VerbosePage");
            AddContent(panel);

            descriptions = Utilities.FindNode<StackPanel>(panel, "Descriptions");

            UpdateSecondPerson();
            Display();
        }

        public void UpdateSecondPerson()
        {
            if( subject == null ) { return; }
            _secondPerson = subject.GetData("name").text.ToLower() == "you";
        }

        public void Display()
        {
            if( subject == null ) { return; }

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
                new WaywardTextParser.ParseDelegate[] {
                    () => { return observer.Observe(subject).span; },
                    () => { return objData.span; },
                    () => { return observer.Observe(obj, "description").span; }
                }
            );
            text.TextWrapping = TextWrapping.Wrap;
            descriptions.Children.Add(text);
        }

        public override void Clear()
        {
            descriptions.Children.Clear();
        }

        public override void Update()
        {
            Display();
        }
    }
}
