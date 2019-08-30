using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class DescriptivePage : WaywardEngine.ContentPage
    {
        GameObject _target;
        public GameObject target {
            get {
                return _target;
            }
            set {
                _target = value;
                DisplayTarget();
            }
        }

        TextBlock descriptionText;

        public DescriptivePage( GameObject obj )
            : base()
        {
            target = obj;
        }

        private void DisplayTarget()
        {
            FrameworkElement content = GameManager.instance.GetResource<FrameworkElement>("DescriptiveDescription");
            AddContent(content);

            SetTitle(target.GetData("name").text);

            descriptionText = Utilities.FindNode<TextBlock>(content, "Description");
            descriptionText.Inlines.Add( target.GetData("description").span );

            FetchObjectContents();
        }
        private void FetchObjectContents()
        {
            IContainer container = target as Container;
            if( container == null ) { return; }

            FrameworkElement content = GameManager.instance.GetResource<FrameworkElement>("DescriptiveContents");
            AddContent(content);

            StackPanel inventory = Utilities.FindNode<StackPanel>(content, "Contents");
            if( inventory != null ) {
                if( container.ContentCount() > 0 ) {
                    for( int i = 0; i < container.ContentCount(); i++ ) {
                        DisplayContent( inventory, container.GetContent(i) );
                    }
                } else {
                    inventory.Children.Add( new TextBlock(new Italic(new Run("empty"))) ); // XXX: Hate this - WaywardManager text parser needed
                }
            }
        }
        private void DisplayContent(StackPanel inventory, GameObject child)
        {
            // Add separator from previous entry
            if( inventory.Children.Count > 0 ) {
                Separator separator = new Separator();
                inventory.Children.Add(separator);
            }

            FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
            if( entry == null ) { return; }
            inventory.Children.Add(entry);

            TextBlock nameText = Utilities.FindNode<TextBlock>(entry, "Data1");
            GameObjectData data = child.GetData("name");
            nameText.Inlines.Add(data.span);
        }

        public void SetDescription( string text )
        {
            descriptionText.Text = text;
        }

        public override void Update()
        {
            Clear();
            DisplayTarget();
        }
    }
}
