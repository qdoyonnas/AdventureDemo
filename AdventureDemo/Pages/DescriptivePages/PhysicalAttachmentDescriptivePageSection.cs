using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class PhysicalAttachmentDescriptivePageSection : DescriptivePageSection
    {
        Physical physicalTarget;
        StackPanel attachmentsPanel;

        public PhysicalAttachmentDescriptivePageSection()
            : base("DescriptivePhysicalAttachments")
        {
            attachmentsPanel = Utilities.FindNode<StackPanel>(element, "Attachments");
        }

        public override void AssignPage( DescriptivePage page )
        {
            base.AssignPage(page);
            if( page != null ) {
                physicalTarget = page.target as Physical;
            }
        }

        public override void DisplayContents()
        {
            // XXX: This bypasses the observer rule
            foreach( AttachmentPoint point in physicalTarget.GetAttachmentPoints() ) {
                StackPanel entry = GameManager.instance.GetResource<StackPanel>("DescriptiveAttachmentEntry");
                attachmentsPanel.Children.Add( entry );
                TextBlock text = Utilities.FindNode<TextBlock>( entry, "AttachmentName" );
                if( text != null ) {
                    text.Text = char.ToUpper(point.name[0]) + point.name.Substring(1);
                }
                if( point.maxQuantity != -1 ) {
                    text = Utilities.FindNode<TextBlock>( entry, "Quantity" );
                    if( text != null ) {
                        if( observer.CanObserve(physicalTarget) ) {
                            text.Text = $"{point.GetAttached().Length.ToString()}/{point.maxQuantity.ToString()}";
                        } else {
                            text.Text = "???";
                        }
                    }
                }
                PhysicalAttachmentPoint physicalPoint = point as PhysicalAttachmentPoint;
                if( physicalPoint != null ) {
                    text = Utilities.FindNode<TextBlock>( entry, "VolumeRatio" );
                    if( text != null ) {
                        if( observer.CanObserve(physicalTarget) ) {
                            if( physicalPoint.capacity >= 0 ) {
                                text.Text = $"{physicalPoint.filledCapacity.ToString()} L/{physicalPoint.capacity.ToString()} L";
                            } else {
                                text.Text = $"{physicalPoint.filledCapacity.ToString()} L";
                            }
                        } else {
                            text.Text = "???";
                        }
                    }
                }

                DisplayAttachmentContents(entry, point);
            }

            if( attachmentsPanel.Children.Count == 0 ) {
                element.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
        private void DisplayAttachmentContents( StackPanel entry, AttachmentPoint point )
        {
            StackPanel contents = Utilities.FindNode<StackPanel>( entry, "Contents" );
            foreach( GameObject obj in point.GetAttached() ) {
                Grid objEntry = GameManager.instance.GetResource<Grid>("AttachmentObjectEntry");
                contents.Children.Add( objEntry );

                TextBlock text = Utilities.FindNode<TextBlock>( objEntry, "Data1" );
                if( text != null ) {
                    text.Inlines.Add( observer.Observe(obj, "name upper").span );
                }

                text = Utilities.FindNode<TextBlock>( objEntry, "Data2" );
                if( text != null ) {
                    text.Inlines.Add( observer.Observe(obj, "weight").span );
                }

                text = Utilities.FindNode<TextBlock>( objEntry, "Data3" );
                if( text != null ) {
                    text.Inlines.Add( observer.Observe(obj, "volume").span );
                }
            }

            if( contents.Children.Count == 0 ) {
                contents.Children.Add( WaywardTextParser.ParseAsBlock("<i>empty</i>") );
            }
        }

        public override void Clear()
        {
            attachmentsPanel.Children.Clear();
        }
    }
}
