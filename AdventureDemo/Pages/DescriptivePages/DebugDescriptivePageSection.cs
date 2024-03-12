using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
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

            DisplayType();
            DisplayContainer();
            DisplayAttachmentTypes();
            DisplayTags();
        }
        private void DisplayType()
        {
            string typeString = $"Type: {page.target.GetType().ToString()}";
            TextBlock typeBlock = WaywardTextParser.ParseAsBlock(typeString);
            content.Children.Add(typeBlock);
        }
        private void DisplayContainer()
        {
            try {
                string containerContent = "Container: [0]";
                AttachmentPoint container = page.target.attachPoint;
                TextBlock containerBlock;
                if( container != null ) {
                    containerBlock = WaywardTextParser.ParseAsBlock(containerContent,
                        () => { return page.observer.Observe(container.GetParent(), "name upper").span; }
                    );
                } else {
                    containerBlock = WaywardTextParser.ParseAsBlock("Container: <i>null</i>");
                }
                Grid.SetRow(containerBlock, 2);
                content.Children.Add(containerBlock);
            } catch( NullReferenceException e ) {
                TextBlock containerBlock = WaywardTextParser.ParseAsBlock("Container: <i>null</i>");
                Grid.SetRow(containerBlock, 2);
                content.Children.Add(containerBlock);
            }

        }
        private void DisplayAttachmentTypes()
        {
            string attachmentString = "Attachment Types: ";
            if( page.target.attachmentTypes.Count > 0 ) {
                foreach( AttachmentType type in page.target.attachmentTypes ) {
                    attachmentString += type.ToString() + " ";
                }
            } else {
                attachmentString += "<i>none</i>";
            }

            TextBlock attachmentBlock = WaywardTextParser.ParseAsBlock(attachmentString);
            Grid.SetRow(attachmentBlock, 2);
            Grid.SetColumn(attachmentBlock, 1);
            content.Children.Add(attachmentBlock);
        }
        private void DisplayTags()
        {
            string tagsString = "Tags: ";
            string[] tags = page.target.tags.ToArray();
            if( tags.Length > 0 ) {
                for( int i = 0; i < tags.Length; i++ ) {
                    tagsString += tags[i];
                    if( i != tags.Length - 1 ) {
                        tagsString += ", ";
                    }
                }
            } else {
                tagsString += "<i>none</i>";
            }

            TextBlock tagsBlock = WaywardTextParser.ParseAsBlock(tagsString);
            Grid.SetRow(tagsBlock, 3);
            content.Children.Add(tagsBlock);
        }
    }
}
