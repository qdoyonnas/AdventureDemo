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
    class OverviewPage : WaywardEngine.ContentPage
    {
        Actor _observer;
        public Actor observer {
            get {
                return _observer;
            }
        }

        Dictionary<GameObject, StackPanel> overviewContentPanels;
        Dictionary<string, StackPanel> overviewEventPanels;

        public OverviewPage(Actor observer) : base()
        {
            _observer = observer;

            overviewContentPanels = new Dictionary<GameObject, StackPanel>();
            overviewEventPanels = new Dictionary<string, StackPanel>();

            SetTitle("Overview");
        }
        
        private StackPanel AddContentPanel( GameObject key )
        {
            FrameworkElement overviewContents = GameManager.instance.GetResource<FrameworkElement>("OverviewContents");
            AddContent(overviewContents);

            StackPanel contents = Utilities.FindNode<StackPanel>(overviewContents, "Contents");
            overviewContentPanels.Add(key, contents);

            return contents;
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
        public void DisplayObject( GameObject obj )
        {
            GameObject objContainer = obj.container as GameObject;
            if( objContainer == null ) { return; }

            StackPanel parent;
            if( overviewContentPanels.ContainsKey(obj) ) {
                parent = overviewContentPanels[obj];
            } else {
                parent = AddContentPanel(obj);
            }


            DisplayObject( parent, objContainer );
        }
        /// <summary>
        /// Recursively populates Stackpanel parent with entries relevant to Object obj.
        /// </summary>
        /// <param name="parent">Stackpanel being populated.</param>
        /// <param name="obj">Object to add.</param>
        private void DisplayObject( StackPanel parent, GameObject obj )
        {
            // OverviewPage is responsible for fetching the objects and organizing the content
            // Observer is responsible for validating, styling, and attaching actions to objects

            if( parent == null || observer == null || obj == null ) { return; }
            
            if( !observer.CanObserve(obj) ) { return; }

            // Entry styling
            bool isMainObject = false;
            if( parent.Children.Count > 0 ) {
                Separator separator = new Separator();
                parent.Children.Add(separator);
            } else if( overviewContentPanels.ContainsValue(parent) ) {
                isMainObject = true;
            }

            FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
            parent.Children.Add(entry);
            
            TextBlock text = Utilities.FindNode<TextBlock>( entry, "Data1");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj, "name").span );
                if( isMainObject ) {
                    text.Style = GameManager.instance.GetResource<Style>( "Header" );
                }
            }
            text = Utilities.FindNode<TextBlock>( entry, "Data2" );
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj, "data 0").span );
            }
            text = Utilities.FindNode<TextBlock>( entry, "Data3" );
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj, "data 1").span );
            }

            FetchObjectContents(entry, obj);
        }
        private void FetchObjectContents( FrameworkElement entry, GameObject obj )
        {
            IContainer container = obj as IContainer;
            if( container == null ) { return; }

            StackPanel objectContents = Utilities.FindNode<StackPanel>( entry, "SubData" );
            if( objectContents != null ) {
                List<Connection> connections = container.GetConnections();
                if( container.ContentCount() > 0 ) {
                    for( int i = 0; i < container.ContentCount(); i++ ) {
                        DisplayObject( objectContents, container.GetContent(i) );
                    }
                }
                foreach( Connection connection in connections ) {
                    DisplayConnection(objectContents, container, connection);
                }

                if( objectContents.Children.Count == 0 ) {
                    objectContents.Visibility = Visibility.Hidden;
                }
            }
        }
        private void DisplayConnection( StackPanel objectContents, IContainer container, Connection connection )
        {
            FrameworkElement newEntry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
            objectContents.Children.Add(newEntry);
            TextBlock text = Utilities.FindNode<TextBlock>( newEntry, "Data1");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(connection, "name").span );
            }

            text = Utilities.FindNode<TextBlock>( newEntry, "Data2" );
            if( text != null ) {
                GameObject connected;
                if( connection.container == container ) {
                    connected = connection.containerB as GameObject;
                } else {
                    connected = connection.container as GameObject;
                }
                if( connected == null ) { return; }
                text.Inlines.Add( observer.Observe(connected).span );
            }

            PhysicalConnection physicalConnection = connection as PhysicalConnection;
            if( physicalConnection != null ) {
                text = Utilities.FindNode<TextBlock>( newEntry, "Data3" );
                if( text != null ) {
                    text.Inlines.Add( observer.Observe(connection, "volume").span );
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

        public override void Clear()
        {
            foreach( StackPanel panel in overviewContentPanels.Values ) {
                panel.Children.Clear();
            }
            /*foreach( StackPanel panel in overviewEventPanels.Values ) {
                panel.Children.Clear();
            }*/
        }

        public override void Update()
        {
            Clear();
            foreach( GameObject obj in overviewContentPanels.Keys ) {
                DisplayObject(obj);
            }
        }
    }
}
