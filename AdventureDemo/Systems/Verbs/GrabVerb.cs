using System;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    class GrabVerb : Verb
    {
        AttachmentPoint inventory;
        Physical physicalSelf;

        public GrabVerb() : base() { }
        public GrabVerb( GameObject self ) : base(self) {}

        protected override void Construct()
        {
            _displayLabel = "Grab";

            _validInputs = new string[] {
                "grab", "drop", "pickup", "take"
            };
        }
        protected override void InitVerb()
        {
            this.inventory = inventory;
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

            return base.Action(target);
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

        public override bool ParseInput( InputEventArgs e )
        {
            if( e.parsed ) { return true; }
            if( e.words.Length <= 1 ) { return false; }

            WaywardManager.instance.DisplayMessage($"Grab {e.words[1]}");

            return true;
        }
    }
}
