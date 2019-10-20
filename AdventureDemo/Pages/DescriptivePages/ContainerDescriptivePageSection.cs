using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WaywardEngine;

namespace AdventureDemo
{
    class ContainerDescriptivePageSection : DescriptivePageSection
    {
        StackPanel contents;
        TextBlock volumeRatio;
        StackPanel connections;

        IContainer container;

        public ContainerDescriptivePageSection()
            : base("DescriptiveContainer")
        {
            contents = Utilities.FindNode<StackPanel>( element, "Contents" );
            volumeRatio = Utilities.FindNode<TextBlock>( element, "VolumeRatio" );
            connections = Utilities.FindNode<StackPanel>( element, "Connections" );

            if( page != null ) {
                container = page.target as IContainer;
            }
        }

        public override void AssignPage( DescriptivePage page )
        {
            base.AssignPage(page);
            if( page != null ) {
                container = page.target as IContainer;
            }
        }

        public override void Clear()
        {
            volumeRatio.Inlines.Clear();
            contents.Children.Clear();
            connections.Children.Clear();

            if( page != null ) {
                container = page.target as IContainer;
            }
        }

        public override void DisplayContents()
        {
            if( observer.CanObserve(page.target) ) {
                volumeRatio.Inlines.Add( page.target.GetData("volumeratio").span );
            }

            if( container == null ) {
                throw new System.NullReferenceException("DescriptivePage tried to display the contents of a GameObject that is not an IContainer.");
            }

            if( container.ContentCount() > 0 ) {
                for( int i = 0; i < container.ContentCount(); i++ ) {
                    DisplayContent( container.GetContent(i) );
                }
            }

            if( contents.Children.Count == 0 ) {
                contents.Children.Add( WaywardTextParser.ParseAsBlock("<i>empty</i>") );
            }

            List<Connection> containerConnections = container.GetConnections();
            foreach( Connection connection in containerConnections ) {
                DisplayConnection( connection );
            }

            if( connections.Children.Count == 0 ) {
                connections.Children.Add( WaywardTextParser.ParseAsBlock("<i>none</i>") );
            }
        }
        
        private void DisplayContent( GameObject obj )
        {
            if( !observer.CanObserve(obj) ) { return; }

            // Add separator from previous entry
            if( contents.Children.Count > 0 ) {
                Separator separator = new Separator();
                contents.Children.Add(separator);
            }

            FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
            if( entry == null ) { return; }
            contents.Children.Add(entry);

            TextBlock text = Utilities.FindNode<TextBlock>(entry, "Data1");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj).span );
            }
            
            text = Utilities.FindNode<TextBlock>( entry, "Data2" );
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj, "data 0").span );
            }
            text = Utilities.FindNode<TextBlock>( entry, "Data3" );
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj, "data 1").span );
            }
        }
        private void DisplayConnection( Connection connection )
        {
            if( !observer.CanObserve(connection) ) { return; }

            // Add separator from previous entry
            if( connections.Children.Count > 0 ) {
                Separator separator = new Separator();
                connections.Children.Add(separator);
            }

            FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
            if( entry == null ) { return; }
            connections.Children.Add(entry);

            TextBlock text = Utilities.FindNode<TextBlock>(entry, "Data1");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(connection).span );
            }
            
            text = Utilities.FindNode<TextBlock>( entry, "Data2" );
            if( text != null ) {
                GameObject connected;
                if( connection.container == container ) {
                    connected = connection.secondContainer as GameObject;
                } else {
                    connected = connection.container as GameObject;
                }
                if( connected == null ) { return; }

                text.Inlines.Add( observer.Observe(connected).span );
            }

            PhysicalConnection physicalConnection = connection as PhysicalConnection;
            if( physicalConnection != null ) {
                text = Utilities.FindNode<TextBlock>( entry, "Data3" );
                if( text != null ) {
                    text.Inlines.Add( observer.Observe(connection, "volume").span );
                }
            }
        }
    }
}
