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
    class OverviewPage : WaywardEngine.Page
    {
        public Dictionary<string, StackPanel> overviewContentPanels;
        public Dictionary<string, StackPanel> overviewEventPanels;

        public OverviewPage() : base()
        {
            overviewContentPanels = new Dictionary<string, StackPanel>();
            overviewEventPanels = new Dictionary<string, StackPanel>();

            SetTitle("Overview");
        }
        
        /// <summary>
        /// Add a new Overview Contents panel to the page.
        /// </summary>
        /// <param name="key">String used to reference this panel.</param>
        public void AddOverview( string key )
        {
            FrameworkElement overviewContent = GameManager.instance.GetResource<FrameworkElement>("OverviewContents");
            AddContent(overviewContent);

            StackPanel contents = Utilities.FindNode<StackPanel>(overviewContent, "Contents");
            overviewContentPanels.Add(key, contents);
        }
        public void AddEventPanel( string key )
        {
            FrameworkElement overviewEvents = GameManager.instance.GetResource<FrameworkElement>("OverviewEvents");
            AddContent(overviewEvents);

            StackPanel events = Utilities.FindNode<StackPanel>(overviewEvents, "Events");
            overviewEventPanels.Add(key, events);
        }

        /// <summary>
        /// Adds Object obj to the OverviewPage's content section.
        /// </summary>
        /// <param name="obj">Object to be displayed.</param>
        public void DisplayObject( string key, GameObject obj )
        {
            StackPanel parent = overviewContentPanels[key];
            if( parent == null ) {
                Console.WriteLine($"ERROR: OverviewPage does not contain content panel with key '{key}'");
                return;
            }
            DisplayObject( parent, obj );
        }
        /// <summary>
        /// Recursively Populates Stackpanel parent with entries relevant to Object obj.
        /// </summary>
        /// <param name="parent">Stackpanel being populated.</param>
        /// <param name="obj">Object to add.</param>
        private void DisplayObject( StackPanel parent, GameObject obj )
        {
            if( parent == null || obj == null ) { return; }

            // Add separator from previous entry
            if( parent.Children.Count > 0 ) {
                Separator separator = new Separator();
                parent.Children.Add(separator);
            }

            FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
            parent.Children.Add(entry);
            
            TextBlock text = Utilities.FindNode<TextBlock>( entry, "Data1");
            if( text != null ) {
                Span objectData = obj.GetData("name").span;
                text.Inlines.Add(objectData);
            }

            FetchObjectContents(entry, obj);
        }
        private void FetchObjectContents( FrameworkElement entry, GameObject obj )
        {
            IContainer container = obj as IContainer;
            if( container == null ) { return; }

            StackPanel objectContents = Utilities.FindNode<StackPanel>( entry, "SubData" );
            if( objectContents != null ) {
                if( container.ContentCount() > 0 ) {
                    for( int i = 0; i < container.ContentCount(); i++ ) {
                        DisplayObject( objectContents, container.GetContent(i) );
                    }
                } else {
                    objectContents.Visibility = Visibility.Hidden;
                }
            }
        }

        public void DisplayEvent( string key, string e )
        {
            StackPanel panel = overviewEventPanels[key];
            if( panel == null ) {
                Console.WriteLine($"ERROR: OverviewPage does not contain events panel with key '{key}'");
                return;
            }
            TextBlock text = new TextBlock();
            text.Text = e;
            panel.Children.Add( text );
        }
    }
}
