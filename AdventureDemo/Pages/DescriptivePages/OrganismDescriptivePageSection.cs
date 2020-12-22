using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    class OrganismDescriptivePageSection : DescriptivePageSection
    {
        Organism organism;
        StackPanel bodyParts;

        public OrganismDescriptivePageSection()
            : base( "DescriptiveOrganism" )
        {
            bodyParts = Utilities.FindNode<StackPanel>( element, "BodyParts" );
        }

        public override void AssignPage( DescriptivePage page )
        {
            base.AssignPage(page);
            if( page != null ) {
                organism = page.target as Organism;
            }
        }

        public override void Clear()
        {
            bodyParts.Children.Clear();
        }
        public override void DisplayContents()
        {
            DisplayBody();

            if( bodyParts.Children.Count == 0 ) {
                element.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        protected virtual void DisplayBody()
        {
            if( !observer.CanObserve(organism) ) { return; }
            
        }
        protected virtual void DisplayBodyPart(GameObject obj, StackPanel stack)
        {
            Grid entry = GameManager.instance.GetResource<Grid>("AttachmentObjectEntry");
            if( entry == null ) { throw new System.NullReferenceException("Resource 'AttachmentObjectEntry' could not be found"); }
            stack.Children.Add(entry);

            TextBlock text = Utilities.FindNode<TextBlock>(entry, "Data1");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj, "name upper").span );
            }

            text = Utilities.FindNode<TextBlock>(entry, "Data2");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj, "weight partial").span );
            }

            text = Utilities.FindNode<TextBlock>(entry, "Data3");
            if( text != null ) {
                text.Inlines.Add( observer.Observe(obj, "volume partial").span );
            }

            StackPanel subData = Utilities.FindNode<StackPanel>(entry, "SubData");
            PhysicalAmalgam part = obj as PhysicalAmalgam;
            if( part != null ) {
                foreach( Physical child in part.GetParts() ) {
                    DisplayBodyPart(child, subData);
                }
            }

            if( subData.Children.Count == 0 ) {
                subData.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
