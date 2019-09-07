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

        public ContainerDescriptivePageSection()
            : base("DescriptiveContainer")
        {
            contents = Utilities.FindNode<StackPanel>( element, "Contents" );
            volumeRatio = Utilities.FindNode<TextBlock>( element, "VolumeRatio" );
        }

        public override void Clear()
        {
            contents.Children.Clear();
        }

        public override void DisplayContents()
        {
            volumeRatio.Inlines.Add( page.target.GetData("volumeratio").span );

            IContainer container = page.target as IContainer;
            if( container == null ) {
                throw new System.NullReferenceException("DescriptivePage tried to display the contents of a GameObject that is not an IContainer.");
            }

            if( container.ContentCount() > 0 ) {
                for( int i = 0; i < container.ContentCount(); i++ ) {
                    DisplayContent( container.GetContent(i) );
                }
            } else {
                contents.Children.Add( new TextBlock(new Italic(new Run("empty"))) ); // XXX: Hate this - WaywardManager text parser needed
            }
        }
        
        private void DisplayContent( GameObject child )
        {
            // Add separator from previous entry
            if( contents.Children.Count > 0 ) {
                Separator separator = new Separator();
                contents.Children.Add(separator);
            }

            FrameworkElement entry = GameManager.instance.GetResource<FrameworkElement>("OverviewEntry");
            if( entry == null ) { return; }
            contents.Children.Add(entry);

            TextBlock nameText = Utilities.FindNode<TextBlock>(entry, "Data1");
            GameObjectData data = child.GetData("name");
            nameText.Inlines.Add(data.span);
        }
    }
}
