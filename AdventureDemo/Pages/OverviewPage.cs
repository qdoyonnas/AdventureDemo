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
            FrameworkElement overviewContent = GameManager.instance.application.Resources["OverviewContents"] as FrameworkElement;
            if( overviewContent == null ) {
                throw new System.NullReferenceException("OverviewPage could not find 'OverviewContents' resource");
            }
            AddContent(overviewContent);

            StackPanel contents = LogicalTreeHelper.FindLogicalNode(overviewContent, "Contents") as StackPanel;
            if( overviewContent == null ) {
                throw new System.NullReferenceException("OverviewPage could not find StackPanel 'Contents'");
            }
            overviewContentPanels.Add(key, contents);
        }
        public void AddEventPanel( string key )
        {
            FrameworkElement overviewEvents = GameManager.instance.application.Resources["OverviewEvents"] as FrameworkElement;
            if( overviewEvents == null ) {
                throw new System.NullReferenceException("OverviewPage could not find 'OverviewEvents' resource");
            }
            AddContent(overviewEvents);

            StackPanel events = LogicalTreeHelper.FindLogicalNode(overviewEvents, "Events") as StackPanel;
            if( overviewEvents == null ) {
                throw new System.NullReferenceException("OverviewPage could not find StackPanel 'Events'");
            }
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

            FrameworkElement entry = WaywardManager.instance.application.Resources["OverviewEntry"] as FrameworkElement;
            if( entry == null ) {
                throw new System.NullReferenceException("OverviewPage could not find 'OverviewEntry' resource");
            }

            TextBlock text = LogicalTreeHelper.FindLogicalNode( entry, "Data1") as TextBlock;
            if( text != null ) {
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
                            DisplayObject( objectContents, container.GetContent(i) );
                        }
                    } else {
                        objectContents.Visibility = Visibility.Hidden;
                    }
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
