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
    class DescriptivePage : WaywardEngine.Page
    {
        GameObject target;

        TextBlock descriptionText;

        public DescriptivePage( GameObject obj )
            : base()
        {
            target = obj;
            SetTitle(target.GetData("name").text);

            FrameworkElement content = GameManager.instance.GetResource<FrameworkElement>("DescriptiveDescription");
            AddContent(content);

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
    }
}
