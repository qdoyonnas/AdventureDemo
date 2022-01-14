using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

//css_include ../Data/Verbs/Scripts/defaultVerb.cs;

class PhaseVerb : DefaultVerb
{
    public override bool Construct(Verb verb, Dictionary<string, object> data)
    {
        verb.displayLabel = "Phase";

        verb.AddValidInput("phase");

        return true;
    }

    public override bool Action( Verb verb, Dictionary<string, object> data )
    {
        bool success = false;

        GameObject target = null;
        if( data.ContainsKey("target") ) {
            target = data["target"] as GameObject;
            success = true;
        }
        if( !success ) {
            success = false;
            if( data.ContainsKey("point") ) {
                AttachmentPoint point = data["point"] as AttachmentPoint;
                if( point != null ) {
                    success = Action(verb, point);
                }
            }
        }

        if( !success ) { return false; }

        Physical physical = target as Physical;
        if( physical != null ) {
            success = Action(verb, physical.GetAttachmentPoints()[0]);
        }

        return success;
    }
    public bool Action( Verb verb, AttachmentPoint target )
    {
        Physical physicalSelf = verb.self as Physical;
        if( physicalSelf != null ) {
            Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);
            target.Attach(parent);
        } else {
            target.Attach(verb.self);
        }

        // Create data dictionary to be passed to observers
        Dictionary<string, object> data = new Dictionary<string, object>();

        // Message for Verbose pages
        data["message"] = new ObservableText($"[0] {verb.displayLabel.ToLower()} into [1].", 
            new Tuple<GameObject, string>(verb.self, "name top"),
            new Tuple<GameObject, string>(target.GetParent(), "name")
        );
        data["turnPage"] = true;
        data["displayAfter"] = true;

        TimelineManager.instance.OnAction(data);

        return true;
    }

    public override CheckResult Check( Verb verb, GameObject target )
    {
        CheckResult check = CheckResult.INVALID;

        if( verb.self.container.GetParent() == target
            && target.container != null 
            && target.container != verb.self.container )
        {
            check = target.container.CanAttach(verb.self);
            if( check >= CheckResult.RESTRICTED ) {
                return check;
            }
        }

        Physical physical = target as Physical;
        if( physical != null ) {

            Physical physicalSelf = verb.self as Physical;
            if( physicalSelf != null
                && physicalSelf.Contains(physical) )
            {
                return CheckResult.INVALID;
            }

            foreach( AttachmentPoint point in physical.GetAttachmentPoints() ) {
                if( point == verb.self.container ) { continue; }

                CheckResult pointCheck = point.CanAttach(verb.self);
                check = pointCheck > check ? pointCheck : check;
                if( check >= CheckResult.RESTRICTED ) {
                    return check;
                }
            }
        }

        return check;
    }

    public override bool Display( Verb verb, Actor actor, GameObject target, FrameworkContentElement span )
    {
        if( target == verb.self ) { return; }
        if( Check(target) < CheckResult.RESTRICTED ) { return; }

        string actionLabel = verb.displayLabel;

        if( verb.self.container.GetParent() == target && target.container != null ) {
            actionLabel = verb.displayLabel + " out";
            DisplayForPoint(verb, actionLabel, target.container, span);
        }

        Physical physical = target as Physical;
        if( physical != null ) {
            foreach( AttachmentPoint point in physical.GetAttachmentPoints() ) {
                actionLabel = verb.displayLabel + " into " + point.name;
                DisplayForPoint(verb, actionLabel, point, span);
            }
        }

        return true;
    }
    private void DisplayForPoint( Verb verb, string actionLabel, AttachmentPoint point, FrameworkContentElement span )
    {
        if( point == verb.self.container ) { return; }

        CheckResult result = point.CanAttach(verb.self);
        if( result >= CheckResult.RESTRICTED ) {
            Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();
            if( result == CheckResult.RESTRICTED ) {
                items.Add( WaywardTextParser.ParseAsBlock($@"<gray>{actionLabel}</gray>") , null );
            } else {
                items.Add( WaywardTextParser.ParseAsBlock(actionLabel) , delegate { return verb.Register(point, true); } );
            }
            ContextMenuHelper.AddContextMenuHeader(span, new TextBlock(verb.self.GetData("name upper").span), items, result != CheckResult.RESTRICTED);
        }
    }

    public override bool ParseInput( Verb verb, InputEventArgs inputEventArgs )
    {
        if( inputEventArgs.parsed ) { return true; }
        if( inputEventArgs.parameters.Length <= 0 ) {
            WaywardManager.instance.DisplayMessage("Phase where?");
            return true; 
        }

        if( CheckForOutInput(verb, inputEventArgs) ) { return true; }

        GameObject foundObject = GetInputTarget(verb, inputEventArgs);
        if( foundObject == null ) { return true; }
        if( foundObject == verb.self ) {
            WaywardManager.instance.DisplayMessage($"You cannot phase into yourself.");
            return true;
        }

        if( verb.Check(foundObject) == CheckResult.VALID ) {
            verb.Action(new Dictionary<string, object>() {{ "target", foundObject }});
            return true;
        } else {
            WaywardManager.instance.DisplayMessage($"Could not phase into {foundObject.GetData("name").text}.");
        }

        return false;
    }
    private bool CheckForOutInput( Verb verb, InputEventArgs inputEventArgs )
    {
        if( inputEventArgs.parameters[0].ToLower() != "out" ) { return false; }

        if( verb.self.container != null ) {
            GameObject container = verb.self.container.GetParent();
            if( container.container != null
                && verb.Check(container.container.GetParent()) == CheckResult.VALID ) {
                verb.Action(new Dictionary<string, object>() {{ "target", container.container.GetParent() }});
                return true;
            }
        }

        WaywardManager.instance.DisplayMessage("Could not phase out.");
        return true;
    }
    private GameObject GetInputTarget( Verb verb, InputEventArgs inputEventArgs )
    {
        if( inputEventArgs.parameterInput == "self" || inputEventArgs.parameterInput == "me" ) {
            WaywardManager.instance.DisplayMessage($"You cannot phase into yourself.");
            return null;
        }
        Dictionary<string, string> properties = new Dictionary<string, string>();
        properties["name"] = inputEventArgs.parameterInput;
        GameObject[] foundObjects = GameManager.instance.world.FindObjects( verb.self, properties );
        if( foundObjects.Length <= 0 ) {
            string message = $"No such place as {inputEventArgs.parameterInput}";

            WaywardManager.instance.DisplayMessage(message);
            return null;
        }

        return foundObjects[0];
    }
}