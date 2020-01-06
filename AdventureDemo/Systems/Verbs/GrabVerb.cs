using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdventureDemo
{
    class GrabVerb : Verb
    {
        AttachmentPoint inventory;
        Physical physicalSelf;

        public GrabVerb( GameObject self, AttachmentPoint inventory )
            : base(self)
        {
            this.inventory = inventory;
            _displayLabel = "Grab";

            physicalSelf = self as Physical;
        }

        public override bool Action( GameObject target )
        {
            if( Check(target) != CheckResult.VALID ) { return false; }

            if( target.container.GetParent() == self ) {
                self.container.Attach(target);
            } else {
                inventory.Attach(target);
            }

            return true;
        }

        public override CheckResult Check( GameObject target )
        {
            if( target.container == null || inventory == null ) { return CheckResult.INVALID; }

            Physical physical = target as Physical;
            if( physical == null || physical.attachedTo != null ) { return CheckResult.INVALID; }

            if( physical.Contains(physicalSelf) ) {
                return CheckResult.INVALID;
            }

            if( inventory.Contains(target) ) {
                CheckResult check = self.container.CanAttach(target);
                if( check >= CheckResult.RESTRICTED ) {
                    return check;
                }
            } else {
                Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);
                if( parent.container.Contains(target) ) {
                    CheckResult check = inventory.CanAttach(target);
                    if( check >= CheckResult.RESTRICTED ) {
                        return check;
                    }
                }
            }

            return CheckResult.INVALID;
        }

        public override void Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            if( target.container == null || inventory == null ) { return; }

            if( target.container.GetParent() == self ) {
                _displayLabel = "Drop";
            } else {
                _displayLabel = "Pickup";
            }

            base.Display(actor, target, span);

            _displayLabel = "Grab";
        }
    }
}
