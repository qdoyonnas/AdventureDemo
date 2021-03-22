using System;
using System.Collections.Generic;
using WaywardEngine;
using AdventureCore;

public static bool Construct(Verb verb)
{
    verb.displayLabel = "Grab";

    verb.blackboard["actionTime"] = (Double)100;
    verb.blackboard["quantity"] = 1;
    verb.blackboard["capacity"] = (Double)-1;

    verb.blackboard["inventory"] = (PhysicalAttachmentPoint)null;
    verb.blackboard["physicalSelf"] = (Physical)null;

    verb.AddValidInput("grab", "drop", "pickup", "take");

    return true;
}
public static bool OnAssign(Verb verb)
{
    Physical physicalSelf = verb.self as Physical;
    if( physicalSelf == null ) { 

        return false; 
    }
    verb.blackboard["physicalSelf"] = physicalSelf;

    try {
        Double capacity = (Double)verb.blackboard["capacity"];
        int quantity = (int)verb.blackboard["quantity"];
    } catch( SystemException e ) {
        WaywardManager.instance.Log($@"<red>GrabVerb of GameObject '{verb.self.GetName()}' failed in OnAssign:</red> {e}");
        return false;
    }

    PhysicalAttachmentPoint inventory = new PhysicalAttachmentPoint(physicalSelf, capacity, quantity, AttachmentType.ALL);
    verb.blackboard["inventory"] = inventory;
    physicalSelf.AddAttachmentPoint(inventory);

    return true;
}

public static bool Action( Verb verb, Dictionary<string, object> data )
{
    GameObject target = null;
    if( data.ContainsKey("target") ) {
        target = data["target"] as GameObject;
    }
    if( target == null ) {
        return false;
    }

    if( verb.Check(target) != CheckResult.VALID ) { return false; }

    bool drop = target.container.GetParent() == verb.self;
    if( drop ) {
        verb.self.container.Attach(target);
    } else {
        try {
            PhysicalAttachmentPoint inventory = (PhysicalAttachmentPoint)verb.blackboard["inventory"];
            inventory.Attach(target);
        } catch( SystemException e ) {
            WaywardManager.instance.Log($@"<red>GrabVerb of GameObject '{verb.self.GetName()}' failed in Action:</red> {e}");
            return false;
        }
    }

    // Message for Verbose pages
    string label = drop ? "drop" : "pickup";
    data["message"] = new ObservableText($"[0] { label } [1].", 
        new Tuple<GameObject, string>(verb.self, "name top"),
        new Tuple<GameObject, string>(target, "name")
    );
    data["displayAfter"] = false;

    TimelineManager.instance.OnAction(data);

    return true;
}

public static CheckResult Check( Verb verb, GameObject target )
{
    PhysicalAttachmentPoint inventory = null;
    Physical physicalSelf = null;
    try {
        inventory = (PhysicalAttachmentPoint)verb.blackboard["inventory"];
        physicalSelf = (Physical)verb.blackboard["physicalSelf"];
    } catch( SystemException e ) {
        WaywardManager.instance.Log($@"<red>GrabVerb of GameObject '{verb.self.GetName()}' failed in CheckResult:</red> {e}");
        return CheckResult.INVALID;
    }

    if( target.container == null || inventory == null ) { return CheckResult.INVALID; }

    Physical physical = target as Physical;
    if( physical == null || physical.attachedTo != null ) { return CheckResult.INVALID; }

    if( physical.Contains(physicalSelf) ) {
        return CheckResult.INVALID;
    }

    if( inventory.Contains(target) ) {
        CheckResult check = verb.self.container.CanAttach(target);
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

public static bool Display( Verb verb, Actor actor, GameObject target, FrameworkContentElement span )
{
    PhysicalAttachmentPoint inventory = null;
    try {
        inventory = (PhysicalAttachmentPoint)verb.blackboard["inventory"];
    } catch( SystemException e ) {
        WaywardManager.instance.Log($@"<red>GrabVerb of GameObject '{verb.self.GetName()}' failed in Display:</red> {e}");
        return false;
    }

    if( target.container == null || inventory == null ) { return; }

    if( target.container.GetParent() == verb.self ) {
        verb.displayLabel = "Drop";
    } else {
        verb.displayLabel = "Pickup";
    }

    verb.Display(actor, target, span);

    verb.displayLabel = "Grab";

    return true;
}

public static bool ParseInput( Verb verb, InputEventArgs e )
{
    if( e.parsed ) { return true; }

    if( e.action == "drop" ) {
        return ParseDrop(verb, e);
    } else {
        return ParseGrap(verb, e);
    }
}
static bool ParseDrop( Verb verb, InputEventArgs e )
{
    PhysicalAttachmentPoint inventory = null;
    try {
        inventory = (PhysicalAttachmentPoint)verb.blackboard["inventory"];
    } catch( SystemException e ) {
        WaywardManager.instance.Log($@"<red>GrabVerb of GameObject '{verb.self.GetName()}' failed in ParseDrop:</red> {e}");
        return true;
    }

    if( inventory.GetAttachedCount() == 0 ) {
        WaywardManager.instance.DisplayMessage($"You are not holding anything.");
        return true;
    }

    if( e.parameters.Length <= 0 ) {
        if( inventory.GetAttachedCount() == 1 ) {
            verb.Register(new Dictionary<string, object>() {{ "target", inventory.GetAttached(0)} }, true);
        } else {
            WaywardManager.instance.DisplayMessage($"Drop what?");
        }

        return true;
    } 

    GameObject foundObject = GetInputTarget(verb, e);
    if( foundObject == null ) { return true; }
    if( foundObject == verb.self ) {
        WaywardManager.instance.DisplayMessage($"You get a hold of yourself.");
        return true;
    }

    if( verb.Check(foundObject) == CheckResult.VALID ) {
        verb.Register(new Dictionary<string, object>() {{ "target", foundObject }}, true);
        return true;
    } else {
        WaywardManager.instance.DisplayMessage($"Could not grab {foundObject.GetData("name").text}.");
        return false;
    }
}
static bool ParseGrap( Verb verb, InputEventArgs e )
{
    Physical physicalSelf = null;
    try {
        physicalSelf = (Physical)verb.blackboard["physicalSelf"];
    } catch( SystemException e ) {
        WaywardManager.instance.Log($@"<red>GrabVerb of GameObject '{verb.self.GetName()}' failed in ParseGrab:</red> {e}");
        return true;
    }

    if( e.parameters.Length <= 0 ) {
        WaywardManager.instance.DisplayMessage($"Grab what?");
        return true; 
    }

    GameObject foundObject = GetInputTarget(verb, e);
    if( foundObject == null ) { return true; }
    if( foundObject == verb.self ) {
        WaywardManager.instance.DisplayMessage($"You get a hold of yourself.");
        return true;
    }

    if( verb.Check(foundObject) == CheckResult.VALID ) {
        Physical physical = foundObject as Physical;
        if( physical != null && physicalSelf.Contains(physical) ) {
            WaywardManager.instance.DisplayMessage($"You are already holding {foundObject.GetData("name").text}.");
            return true;
        } else {
            verb.Register(new Dictionary<string, object>() {{ "target", foundObject }}, true);
            return true;
        }
    } else {
        WaywardManager.instance.DisplayMessage($"Could not grab {foundObject.GetData("name").text}.");
        return true;
    }
}

static GameObject GetInputTarget( Verb verb, InputEventArgs e )
{
    if( e.parameterInput == "self" || e.parameterInput == "me" ) {
        return verb.self;
    }

    Dictionary<string, string> properties = new Dictionary<string, string>();
    properties["name"] = e.parameterInput;
    GameObject[] foundObjects = GameManager.instance.world.FindObjects( verb.self, properties );
    if( foundObjects.Length <= 0 ) {
        string message = $"No such thing as {e.parameterInput}";

        WaywardManager.instance.DisplayMessage(message);
        return null;
    }

    return foundObjects[0];
}
