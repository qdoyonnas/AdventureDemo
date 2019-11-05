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
    class PhysicalDescriptivePageSection : DescriptivePageSection
    {
        TextBlock weightText;
        TextBlock volumeText;
        TextBlock materials;

        public PhysicalDescriptivePageSection()
            : base("DescriptivePhysical")
        {
            weightText = Utilities.FindNode<TextBlock>( element, "Weight" );
            volumeText = Utilities.FindNode<TextBlock>( element, "Volume" );
            materials = Utilities.FindNode<TextBlock>( element, "Materials" );
        }

        public override void Clear()
        {
            weightText.Inlines.Clear();
            volumeText.Inlines.Clear();
            materials.Inlines.Clear();
        }
        public override void DisplayContents()
        {
            bool canObserve = observer.CanObserve(page.target);

            // TODO: Route this through observer knowledge instead
            weightText.Inlines.Add( canObserve ? page.target.GetData("weight").span : WaywardTextParser.Parse("???") );
            volumeText.Inlines.Add( canObserve ? page.target.GetData("volume").span : WaywardTextParser.Parse("???") );

            FetchMaterials();
        }
        void FetchMaterials()
        {
            GameObjectData data = observer.Observe(page.target, "materials");
            if( data.span.Inlines.Count > 0 ) {
                materials.Inlines.Add( data.span );
            } else {
                materials.Visibility = Visibility.Collapsed;
            }
        }
    }
}
