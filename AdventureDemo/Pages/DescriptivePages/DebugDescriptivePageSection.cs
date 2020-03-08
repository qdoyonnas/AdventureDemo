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

            string typeString = $"Type: {page.target.GetType().ToString()}";
            TextBlock typeBlock = WaywardTextParser.ParseAsBlock(typeString);
            content.Children.Add(typeBlock);

            try {
                string containerContent = "Container: [0]";
                TextBlock containerBlock = WaywardTextParser.ParseAsBlock(containerContent,
                    () => { return page.observer.Observe(page.target.container.GetParent(), "name upper").span; }
                );
                Grid.SetRow(containerBlock, 2);
                content.Children.Add(containerBlock);
            } catch( NullReferenceException e ) {
                TextBlock containerBlock = WaywardTextParser.ParseAsBlock("Container: <i>null</i>");
                Grid.SetRow(containerBlock, 2);
                content.Children.Add(containerBlock);
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
