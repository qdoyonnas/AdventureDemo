using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class DebugDescriptivePageSection : DescriptivePageSection
    {
        Grid content;

        public DebugDescriptivePageSection()
            : base("DescriptiveDebug")
        {
            content = Utilities.FindNode<Grid>(element, "Content");
        }

        public override void Clear()
        {
            content.Children.Clear();
        }

        public override void DisplayContents()
        {
            string containerContent = "Container: [0]";
            TextBlock block = WaywardTextParser.ParseAsBlock(containerContent, 
                () => { return page.observer.Observe(page.target.container.GetParent(), "name upper").span; }
            );
            content.Children.Add(block);
        }
    }
}
