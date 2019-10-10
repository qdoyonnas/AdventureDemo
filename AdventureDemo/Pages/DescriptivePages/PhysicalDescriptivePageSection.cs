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

        public PhysicalDescriptivePageSection()
            : base("DescriptivePhysical")
        {
            weightText = Utilities.FindNode<TextBlock>( element, "Weight" );
            volumeText = Utilities.FindNode<TextBlock>( element, "Volume" );
        }

        public override void Clear()
        {
            weightText.Inlines.Clear();
            volumeText.Inlines.Clear();
        }
        public override void DisplayContents()
        {
            bool canObserve = observer.CanObserve(page.target);

            weightText.Inlines.Add( canObserve ? page.target.GetData("weight").span : WaywardTextParser.Parse("???") );
            volumeText.Inlines.Add( canObserve ? page.target.GetData("volume").span : WaywardTextParser.Parse("???") );
        }
    }
}
