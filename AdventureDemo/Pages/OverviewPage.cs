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
        
        /// <summary>
        /// Adds Object obj to the OverviewPage's content section.
        /// </summary>
        /// <param name="obj">Object to be displayed.</param>
        public void DisplayObject( GameObject obj )
        {
            AddContent( contents, obj );
        }
        /// <summary>
        /// Recursively Populates Stackpanel parent with entries relevant to Object obj.
        /// </summary>
        /// <param name="parent">Stackpanel being populated.</param>
        /// <param name="obj">Object to add.</param>
        private void AddContent( StackPanel parent, GameObject obj )
        {
            // Add separator from previous entry
            if( parent.Children.Count > 0 ) {
                Separator separator = new Separator();
                parent.Children.Add(separator);
            }

            if( parent != null ) {
                FrameworkElement entry = WaywardManager.instance.application.Resources["OverviewEntry"] as FrameworkElement;
                // Name
                TextBlock text = LogicalTreeHelper.FindLogicalNode( entry, "Data1") as TextBlock;
                if( text != null ) {
                    if( parent == contents ) {
                        text.FontSize = 24;
                    }
                    text.Text = obj.GetData("name");
                }

                parent.Children.Add(entry);

                // Populate subdata
                if( obj is IContainer ) {
                    IContainer container = obj as IContainer;
                    StackPanel objectContents = LogicalTreeHelper.FindLogicalNode( entry, "SubData" ) as StackPanel;
                    if( objectContents != null ) {
                        if( container.ContentCount() > 0 ) {
                            for( int i = 0; i < container.ContentCount(); i++ ) {
                                AddContent( objectContents, container.GetContent(i) );
                            }
                        } else {
                            objectContents.Visibility = Visibility.Hidden;
                        }
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
