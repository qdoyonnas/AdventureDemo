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
        public Label location;
        public StackPanel contents;
        public StackPanel events;

        public OverviewPage( FrameworkElement element ) : base(element)
        {
            location = LogicalTreeHelper.FindLogicalNode( element, "Location" ) as Label;
            contents = LogicalTreeHelper.FindLogicalNode( element, "Contents") as StackPanel;
            events = LogicalTreeHelper.FindLogicalNode( element, "Events") as StackPanel;
        }

        public void SetLocation( string locationName )
        {
            if( location != null ) {
                location.Content = "Dim Room";
            }
        }
        public void AddContent( string name, string holding, string action )
        {
            if( contents.Children.Count > 1 ) {
                Separator separator = new Separator();
                contents.Children.Add(separator);
            }

            if( contents != null ) {
                FrameworkElement entry = WaywardManager.instance.application.Resources["OverviewEntry"] as FrameworkElement;
                TextBlock text = LogicalTreeHelper.FindLogicalNode( entry, "ObjectName") as TextBlock;
                if( text != null ) {
                    text.Text = name;
                }
                text = LogicalTreeHelper.FindLogicalNode( entry, "ObjectHolding") as TextBlock;
                if( text != null ) {
                    text.Text = holding;
                }
                text = LogicalTreeHelper.FindLogicalNode( entry, "ObjectAction") as TextBlock;
                if( text != null ) {
                    text.Text = action;
                }
                contents.Children.Add(entry);
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
