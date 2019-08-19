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
    class OverviewPage : WaywardEngine.Page
    {
        public StackPanel contents;
        public StackPanel events;

        public OverviewPage( FrameworkElement element ) : base(element)
        {
            contents = LogicalTreeHelper.FindLogicalNode( element, "Contents") as StackPanel;
            events = LogicalTreeHelper.FindLogicalNode( element, "Events") as StackPanel;
        }
        
        public void DisplayObject( PlaceholderObject obj )
        {
            AddContent( contents, obj );
        }
        private void AddContent( StackPanel parent, PlaceholderObject obj )
        {
            if( parent.Children.Count > 0 ) {
                Separator separator = new Separator();
                parent.Children.Add(separator);
            }

            if( parent != null ) {
                FrameworkElement entry = WaywardManager.instance.application.Resources["OverviewEntry"] as FrameworkElement;
                TextBlock text = LogicalTreeHelper.FindLogicalNode( entry, "ObjectName") as TextBlock;
                if( text != null ) {
                    if( parent == contents ) {
                        text.FontSize = 24;
                    }
                    text.Text = obj.name;
                }
                text = LogicalTreeHelper.FindLogicalNode( entry, "ObjectHolding") as TextBlock;
                if( text != null ) {
                    text.Text = obj.holding;
                }
                text = LogicalTreeHelper.FindLogicalNode( entry, "ObjectAction") as TextBlock;
                if( text != null ) {
                    text.Text = obj.action;
                }

                parent.Children.Add(entry);

                StackPanel objectContents = LogicalTreeHelper.FindLogicalNode( entry, "ObjectContents" ) as StackPanel;
                if( objectContents != null ) {
                    if( obj.contents.Count > 0 ) {
                        foreach( PlaceholderObject child in obj.contents ) {
                            AddContent( objectContents, child );
                        }
                    } else {
                        objectContents.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        public void AddEvent( string description )
        {
            if( events != null ) {
                TextBlock text = new TextBlock();
                text.Text = description;
                events.Children.Add(text);
            }
        }
    }
}
