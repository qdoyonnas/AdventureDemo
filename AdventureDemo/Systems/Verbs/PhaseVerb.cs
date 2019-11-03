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
    class PhaseVerb : Verb
    {
        public PhaseVerb( GameObject self )
            : base(self)
        {
            _displayLabel = "Phase";
        }

        public override void Action( GameObject target )
        {
            Physical physical = target as Physical;
            if( physical != null ) {
                Action(physical.GetAttachmentPoints()[0]);
            }
        }
        public void Action( AttachmentPoint target )
        {
            self.SetContainer(target);
        }

        public override CheckResult Check( GameObject target )
        {
            CheckResult check = CheckResult.VALID;
            if( self.container.GetParent() == target
                && target.container != null 
                && target.container != self.container )
            {
                check = target.container.CanAttach(self);
                if( check == CheckResult.VALID ) {
                    return check;
                }
            }

            Physical physical = target as Physical;
            if( physical != null ) {
                foreach( AttachmentPoint point in physical.GetAttachmentPoints() ) {
                    if( point == self.container ) { continue; }

                    CheckResult pointCheck = point.CanAttach(self);
                    check = pointCheck > check ? pointCheck : check;
                    if( check == CheckResult.VALID ) {
                        return check;
                    }
                }
            }

            return check;
        }

        public override void Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            if( target == self ) { return; }

            string actionLabel = displayLabel;

            if( self.container.GetParent() == target && target.container != null ) {
                actionLabel = displayLabel + " out";
                DisplayForPoint(actionLabel, target.container, span);
            }

            Physical physical = target as Physical;
            if( physical != null ) {
                foreach( AttachmentPoint point in physical.GetAttachmentPoints() ) {
                    actionLabel = displayLabel + " into " + point.name;
                    DisplayForPoint(actionLabel, point, span);
                }
            }
        }
        private void DisplayForPoint( string actionLabel, AttachmentPoint point, FrameworkContentElement span )
        {
            if( point == self.container ) { return; }

            CheckResult result = point.CanAttach(self);
            if( result >= CheckResult.RESTRICTED ) {
                Dictionary<TextBlock, RoutedEventHandler> items = new Dictionary<TextBlock, RoutedEventHandler>();
                if( result == CheckResult.RESTRICTED ) {
                    items.Add( WaywardTextParser.ParseAsBlock($@"<gray>{actionLabel}</gray>") , null );
                } else {
                    items.Add( WaywardTextParser.ParseAsBlock(actionLabel) , delegate { Action(point); } );
                }
                ContextMenuHelper.AddContextMenuHeader(span, new TextBlock(self.GetData("name upper").span), items, result != CheckResult.RESTRICTED);
            }
        }
    }
}
