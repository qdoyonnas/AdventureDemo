using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
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

        public OverviewPage(Actor observer) : base()
        {
            _observer = observer;

            overviewContentPanels = new Dictionary<GameObject, StackPanel>();
            foreach( GameObject obj in observer.GetSubjectObjects() ) {
                if( obj != null ) {
                    DisplayObject(obj);
                }
            }

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
        
        /// <summary>
        /// Adds Object obj to the OverviewPage's content section.
        /// </summary>
        /// <param name="obj">Object to be displayed.</param>
        public void DisplayObject( GameObject obj )
        {
            if( obj.container == null ) { return; }
            GameObject objContainer = obj.container.GetParent();

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
                text.Inlines.Add( observer.Observe(obj, "name upper").span );
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

            if( isMainObject) { FetchContents(entry, obj); } 
        }
        protected virtual void FetchContents( FrameworkElement entry, GameObject obj )
        {
            Physical physicalObj = obj as Physical;
            if( physicalObj == null ) { return; }

            StackPanel subData = Utilities.FindNode<StackPanel>( entry, "SubData" );
            if( subData == null ) { return; }

            foreach( AttachmentPoint point in physicalObj.GetAttachmentPoints() ) {
                if( point.GetAttached().Length == 0 ) { continue; }

                FrameworkElement pointEntry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
                subData.Children.Add( pointEntry );
                TextBlock text = Utilities.FindNode<TextBlock>( pointEntry, "Data1");
                if( text != null ) {
                    text.Inlines.Add( char.ToUpper(point.name[0]) + point.name.Substring(1) );
                }
                StackPanel pointContents = Utilities.FindNode<StackPanel>( pointEntry, "SubData" );
                foreach( GameObject child in point.GetAttached() ) {
                    DisplayObject( pointContents, child );
                }
            }

            FetchConnections(subData, physicalObj);

            if( subData.Children.Count == 0 ) {
                subData.Visibility = Visibility.Hidden;
            }
        }
        protected virtual void FetchConnections( StackPanel subData, Physical physicalObj )
        {
            Container container = physicalObj as Container;
            if( container == null ) { return; }

            foreach( Connection connection in container.GetConnections() ) {
                FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
                subData.Children.Add( entry );

                TextBlock text = Utilities.FindNode<TextBlock>( entry, "Data1");
                if( text != null ) {
                    text.Inlines.Add( observer.Observe(connection, "name upper").span );
                }

                text = Utilities.FindNode<TextBlock>( entry, "Data2");
                if( text != null ) {
                    GameObject connected = connection.secondContainer.GetParent();
                    text.Inlines.Add( observer.Observe( connected, "name upper").span );
                }

                text = Utilities.FindNode<TextBlock>( entry, "Data3");
                if( text != null ) {
                    text.Inlines.Add( connection.throughput.ToString() + " L" );
                }
            }
        }

        public override void Clear()
        {
            base.Clear();
            overviewContentPanels.Clear();
        }

        public override void Update()
        {
            foreach( GameObject obj in observer.GetSubjectObjects() ) {
                DisplayObject(obj);
            }
        }
    }
}
