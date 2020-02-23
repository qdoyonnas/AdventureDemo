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
            if( page == null ) {
                content.Children.Add(WaywardTextParser.ParseAsBlock("<i>Page is Null</i>"));
                return;
            }
            if( page.observer == null ) {
                content.Children.Add(WaywardTextParser.ParseAsBlock("<i>Page Observer is Null</i>"));
                return;
            }
            if( page.target == null ) {
                content.Children.Add(WaywardTextParser.ParseAsBlock("<i>Page Target is Null</i>"));
                return;
            }

            try {
                string containerContent = "Container: [0]";
                TextBlock block = WaywardTextParser.ParseAsBlock(containerContent,
                    () => { return page.observer.Observe(page.target.container.GetParent(), "name upper").span; }
                );
                content.Children.Add(block);
            } catch( NullReferenceException e ) {
                content.Children.Add(WaywardTextParser.ParseAsBlock("Container: <i>null</i>"));
            }

            string attachmentString = "Attachment Types: ";
            if( page.target.attachmentTypes.Count > 0 ) {
                foreach( AttachmentType type in page.target.attachmentTypes ) {
                    attachmentString += type.ToString() + " ";
                }
            } else {
                attachmentString += "<i>none</i>";
            }
            TextBlock attachmentBlock = WaywardTextParser.ParseAsBlock(attachmentString);
            Grid.SetColumn(attachmentBlock, 1);
            content.Children.Add(attachmentBlock);
        }
    }
}
