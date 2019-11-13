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
    class GameObjectVerbsDescriptivePageSection : DescriptivePageSection
    {
        Grid verbsGrid;

        public GameObjectVerbsDescriptivePageSection()
            : base( "DescriptiveGameObjectVerbs" )
        {
            verbsGrid = Utilities.FindNode<Grid>(element, "Verbs");
        }

        public override void Clear()
        {
            verbsGrid.Children.Clear();
        }

        public override void DisplayContents()
        {
            int r = 0; int c = 0;
            foreach( Verb verb in page.target.CollectVerbs() ) {
                Span text = WaywardTextParser.Parse(verb.displayLabel);
                text.Style = GameManager.instance.GetResource<Style>("Link");
                // TODO: Add descriptive page for Verbs to explain function, behaviour and stats

                TextBlock block = new TextBlock(text);
                Grid.SetRow(block, r);
                Grid.SetColumn(block, c);
                verbsGrid.Children.Add(block);

                c++;
                if( c >= verbsGrid.ColumnDefinitions.Count ) {
                    c = 0;
                    r++;
                }
            }

            if( verbsGrid.Children.Count == 0 ) {
                element.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
