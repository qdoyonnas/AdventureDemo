using System;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    class GrabVerb : Verb
    {
        int quantity = 1;
        double capacity = -1;

        PhysicalAttachmentPoint inventory;
        Physical physicalSelf;

        public GrabVerb() : base() { }
        public GrabVerb( GameObject self ) : base(self) {}
        public GrabVerb(int quantity, double capacity)
        {
            this.quantity = quantity;
            this.capacity = capacity;
        }

        protected override void Construct()
        {
            _displayLabel = "Grab";

            _validInputs = new string[] {
                "grab", "drop", "pickup", "take"
            };
        }
        protected override void OnAssign()
        {
            physicalSelf = self as Physical;
            if( physicalSelf == null ) { return; }

            inventory = new PhysicalAttachmentPoint(physicalSelf, capacity, quantity, AttachmentType.ALL);
            physicalSelf.AddAttachmentPoint(inventory);
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

        public override bool ParseInput( InputEventArgs e )
        {
            if( e.parsed ) { return true; }

            if( e.action == "drop" ) {
                return ParseDrop(e);
            } else {
                return ParseGrap(e);
            }
        }
        private bool ParseDrop( InputEventArgs e )
        {
            if( inventory.GetAttachedCount() == 0 ) {
                WaywardManager.instance.DisplayMessage($"You are not holding anything.");
                return true;
            }

            if( e.parameters.Length <= 0 ) {
                if( inventory.GetAttachedCount() == 1 ) {
                    Action(inventory.GetAttached(0));
                } else {
                    WaywardManager.instance.DisplayMessage($"Drop what?");
                }

                return true;
            } 

            GameObject foundObject = GetInputTarget(e);
            if( foundObject == null ) { return true; }
            if( foundObject == self ) {
                WaywardManager.instance.DisplayMessage($"You cannot grab yourself.");
                return true;
            }

            if( Check(foundObject) == CheckResult.VALID ) {
                Action(foundObject);
                return true;
            } else {
                WaywardManager.instance.DisplayMessage($"Could not grab {foundObject.GetData("name").text}.");
            }

            return true;
        }
        private bool ParseGrap( InputEventArgs e )
        {
            if( e.parameters.Length <= 0 ) {
                WaywardManager.instance.DisplayMessage($"Grab what?");
                return true; 
            }

            GameObject foundObject = GetInputTarget(e);
            if( foundObject == null ) { return true; }
            if( foundObject == self ) {
                WaywardManager.instance.DisplayMessage($"You cannot grab yourself.");
                return true;
            }

            if( Check(foundObject) == CheckResult.VALID ) {
                Physical physical = foundObject as Physical;
                if( physical != null && physicalSelf.Contains(physical) ) {
                    WaywardManager.instance.DisplayMessage($"You are already holding {foundObject.GetData("name").text}.");
                } else {
                    Action(foundObject);
                    return true;
                }
            } else {
                WaywardManager.instance.DisplayMessage($"Could not grab {foundObject.GetData("name").text}.");
            }

            return true;
        }

        private GameObject GetInputTarget( InputEventArgs e )
        {
            // XXX: Broken behaviour if target has a name with spaces
            GameObject[] foundObjects = GameManager.instance.world.FindObjects(self, e.parameters);
            if( foundObjects.Length <= 0 ) {
                string message = $"No such thing as ";
                for( int i = 0; i < e.parameters.Length; i++ ) {
                    message += $"'{e.parameters[i]}'";
                    if( i != e.parameters.Length - 1 ) {
                        message += " or ";
                    }
                }

                WaywardManager.instance.DisplayMessage(message);
                return null;
            }

            return foundObjects[0];
        }
    }
}
