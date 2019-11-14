using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            DisplayObject();
        }

        public void DisplayObject()
        {
            if( subject == null ) { return; }
            bool secondPerson = subject.GetData("name").text.ToLower() == "you";

            GameObject container = subject.container.GetParent();
            if( container == null ) { return; }

            descriptions.Children.Add(WaywardTextParser.ParseAsBlock(
                $@"[0] {(secondPerson ? "are" : "is")} in [1], [2].",
                new WaywardTextParser.ParseDelegate[] {
                    () => { return observer.Observe(subject).span; },
                    () => { return observer.Observe(container).span; },
                    () => { return observer.Observe(container, "description").span; }
                }
            ));
        }

        public override void Clear()
        {
            descriptions.Children.Clear();
        }

        public override void Update()
        {
            DisplayObject();
        }
    }
}
