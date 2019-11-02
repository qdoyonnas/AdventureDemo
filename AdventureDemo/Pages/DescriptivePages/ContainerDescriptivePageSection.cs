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
        StackPanel connections;

        Container container;

        public ContainerDescriptivePageSection()
            : base("DescriptiveContainer")
        {
            connections = Utilities.FindNode<StackPanel>( element, "Connections" );

            if( page != null ) {
                container = page.target as Container;
            }
        }

        public override void AssignPage( DescriptivePage page )
        {
            base.AssignPage(page);
            if( page != null ) {
                container = page.target as Container;
            }
        }

        public override void DisplayContents()
        {
            if( connections.Children.Count == 0 ) {
                connections.Children.Add( WaywardTextParser.ParseAsBlock("<i>none</i>") );
            }
        }

        public override void Clear()
        {
            connections.Children.Clear();

            if( page != null ) {
                container = page.target as Container;
            }
        }
    }
}
