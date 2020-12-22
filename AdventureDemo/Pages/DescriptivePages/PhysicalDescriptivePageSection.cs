using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
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

            GameObjectData weightData = page.target.GetData("weight");
            GameObjectData partialWeightData = page.target.GetData("weight partial");
            GameObjectData volumeData = page.target.GetData("volume");
            GameObjectData partialVolumeData = page.target.GetData("volume partial");

            if( weightData.text != partialWeightData.text ) {
                weightText.Inlines.Add( WaywardTextParser.Parse("[0] ([1])",
                    () => { return weightData.span; },
                    () => { return partialWeightData.span; }) );
            } else {
                weightText.Inlines.Add( weightData.span );
            }
            if( volumeData.text != partialVolumeData.text ) {
                volumeText.Inlines.Add( WaywardTextParser.Parse("[0] ([1])",
                    () => { return volumeData.span; },
                    () => { return partialVolumeData.span; }) );
            } else {
                volumeText.Inlines.Add( volumeData.span );
            }

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
