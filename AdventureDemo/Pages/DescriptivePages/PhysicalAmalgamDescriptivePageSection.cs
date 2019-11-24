using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class PhysicalAmalgamDescriptivePageSection : DescriptivePageSection
    {
        PhysicalAmalgam amalgam;
        StackPanel parts;

        public PhysicalAmalgamDescriptivePageSection()
            : base("DescriptiveAmalgam")
        {
            parts = Utilities.FindNode<StackPanel>(element, "Parts");
        }

        public override void AssignPage( DescriptivePage page )
        {
            base.AssignPage(page);
            if( page != null ) {
                amalgam = page.target as PhysicalAmalgam;
            }
        }

        public override void Clear()
        {
            parts.Children.Clear();
        }

        public override void DisplayContents()
        {
            foreach( Physical part in amalgam.GetParts() ) {
                DisplayPart(part);
            }

            if( parts.Children.Count == 0 ) {
                element.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void DisplayPart( Physical part )
        {
            Grid entry = GameManager.instance.GetResource<Grid>("AttachmentObjectEntry");
            if( entry == null ) { throw new System.NullReferenceException("Resource 'AttachmentObjectEntry' could not be found"); }
            parts.Children.Add(entry);

            TextBlock text = Utilities.FindNode<TextBlock>(entry, "Data1");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(part, "name upper").span );
            }

            text = Utilities.FindNode<TextBlock>(entry, "Data2");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(part, "weight partial").span );
            }

            text = Utilities.FindNode<TextBlock>(entry, "Data3");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(part, "volume partial").span );
            }

            StackPanel subData = Utilities.FindNode<StackPanel>(entry, "SubData");
            if( subData.Children.Count == 0 ) {
                subData.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
