using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    class PhaseVerb : Verb
    {
        public PhaseVerb() : base() { }
        public PhaseVerb( GameObject self ) : base(self) {}

        protected override void Construct()
        {
            _displayLabel = "Phase";

            _validInputs = new string[] { "phase" };
        }
        protected override void OnAssign() {}

        public override bool Action( Dictionary<string, object> data )
        {
            GameObject target = null;
            if( data.ContainsKey("target") ) {
                target = data["target"] as GameObject;
            }
            if( target == null ) {
                return false;
            }

            bool success = false;

            Physical physical = target as Physical;
            if( physical != null ) {
                success = Action(physical.GetAttachmentPoints()[0]);
            }

            return success;
        }
        public bool Action( AttachmentPoint target )
        {
            Physical physicalSelf = self as Physical;
            if( physicalSelf != null ) {
                Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);
                target.Attach(parent);
            } else {
                target.Attach(self);
            }

            // Create data dictionary to be passed to observers
            Dictionary<string, object> data = new Dictionary<string, object>();

            // Message for Verbose pages
            data["message"] = new ObservableText($"[0] {displayLabel.ToLower()} into [1].", 
                new Tuple<GameObject, string>(self, "name top"),
                new Tuple<GameObject, string>(target.GetParent(), "name")
            );
            data["turnPage"] = true;
            data["displayAfter"] = true;

            self.OnAction(data);

            return true;
        }

        public override CheckResult Check( GameObject target )
        {
            CheckResult check = CheckResult.INVALID;

            if( self.container.GetParent() == target
                && target.container != null 
                && target.container != self.container )
            {
                check = target.container.CanAttach(self);
                if( check >= CheckResult.RESTRICTED ) {
                    return check;
                }
            }

            Physical physical = target as Physical;
            if( physical != null ) {

                Physical physicalSelf = self as Physical;
                if( physicalSelf != null
                    && physicalSelf.Contains(physical) )
                {
                    return CheckResult.INVALID;
                }

                foreach( AttachmentPoint point in physical.GetAttachmentPoints() ) {
                    if( point == self.container ) { continue; }

                    CheckResult pointCheck = point.CanAttach(self);
                    check = pointCheck > check ? pointCheck : check;
                    if( check >= CheckResult.RESTRICTED ) {
                        return check;
                    }
                }
            }

            return check;
        }

        public bool Register(AttachmentPoint point, bool fromPlayer)
        {
            bool success = TimelineManager.instance.RegisterEvent( () => { Action(point); }, self, this, actionTime );

            if( fromPlayer ) {
                if( success ) {
                    GameManager.instance.Update(actionTime);
                }
                WaywardManager.instance.Update();
            }

            return success;
        }

        public override void Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            if( target == self ) { return; }
            if( Check(target) < CheckResult.RESTRICTED ) { return; }

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
                Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();
                if( result == CheckResult.RESTRICTED ) {
                    items.Add( WaywardTextParser.ParseAsBlock($@"<gray>{actionLabel}</gray>") , null );
                } else {
                    items.Add( WaywardTextParser.ParseAsBlock(actionLabel) , delegate { return Register(point, true); } );
                }
                ContextMenuHelper.AddContextMenuHeader(span, new TextBlock(self.GetData("name upper").span), items, result != CheckResult.RESTRICTED);
            }
        }

        public override bool ParseInput( InputEventArgs e )
        {
            if( e.parsed ) { return true; }
            if( e.parameters.Length <= 0 ) {
                WaywardManager.instance.DisplayMessage("Phase where?");
                return true; 
            }

            if( CheckForOutInput(e) ) { return true; }

            GameObject foundObject = GetInputTarget(e);
            if( foundObject == null ) { return true; }
            if( foundObject == self ) {
                WaywardManager.instance.DisplayMessage($"You cannot phase into yourself.");
                return true;
            }

            if( Check(foundObject) == CheckResult.VALID ) {
                Action(new Dictionary<string, object>() {{ "target", foundObject }});
                return true;
            } else {
                WaywardManager.instance.DisplayMessage($"Could not phase into {foundObject.GetData("name").text}.");
            }

            return false;
        }
        private bool CheckForOutInput( InputEventArgs e )
        {
            if( e.parameters[0].ToLower() != "out" ) { return false; }

            if( self.container != null ) {
                GameObject container = self.container.GetParent();
                if( container.container != null
                    && Check(container.container.GetParent()) == CheckResult.VALID ) {
                    Action(new Dictionary<string, object>() {{ "target", container.container.GetParent() }});
                    return true;
                }
            }

            WaywardManager.instance.DisplayMessage("Could not phase out.");
            return true;
        }
        private GameObject GetInputTarget( InputEventArgs e )
        {
            if( e.parameterInput == "self" || e.parameterInput == "me" ) {
                WaywardManager.instance.DisplayMessage($"You cannot phase into yourself.");
                return null;
            }
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties["name"] = e.parameterInput;
            GameObject[] foundObjects = GameManager.instance.world.FindObjects( self, properties );
            if( foundObjects.Length <= 0 ) {
                string message = $"No such place as {e.parameterInput}";

                WaywardManager.instance.DisplayMessage(message);
                return null;
            }

            return foundObjects[0];
        }
    }
}
