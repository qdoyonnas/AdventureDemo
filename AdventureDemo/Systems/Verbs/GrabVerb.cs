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

        public GrabVerb( GameObject self, AttachmentPoint inventory )
            : base(self)
        {
            this.inventory = inventory;
            _displayLabel = "Grab";
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

            if( target.container.GetParent() == self ) {
                CheckResult check = self.container.CanAttach(target);
                if( check >= CheckResult.RESTRICTED ) {
                    return check;
                }
            } else if( target.container == self.container ) {
                CheckResult check = inventory.CanAttach(target);
                if( check >= CheckResult.RESTRICTED ) {
                    return check;
                }
            }

            return CheckResult.INVALID;
        }

        public override void Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            if( target.container == null || inventory == null ) { return; }

            if( target.container.GetParent() == self ) {
                _displayLabel = "Drop";
            }

            base.Display(actor, target, span);

            _displayLabel = "Pickup";
        }
    }
}
