using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

//css_include ../Data/Verbs/Scripts/defaultVerb.cs;

class TraversalVerb : DefaultVerb
{
    public override bool Construct(Verb verb, Dictionary<string, object> data)
    {
        verb.SetType("TraversalVerb");
        verb.displayLabel = "Walk";

        verb.blackboard["actionTime"] = 500.0;

        verb.AddValidInput("walk", "move", "go");

        return true;
    }
    public override bool OnAssign(Verb verb)
    {
        Physical physicalSelf = verb.self as Physical;
        if( physicalSelf == null ) {
            return false;
        }
        verb.blackboard["physicalSelf"] = physicalSelf;

        return true;
    }

    public override Dictionary<string, object> Action( Verb verb, Dictionary<string, object> data )
    {
        GameObject target = null;
        if( data.ContainsKey("target") ) {
            target = data["target"] as GameObject;
        }
        if( target == null ) {
            return null;
        }

        //XXX: Always displaying a message on failed action is dangerous when it comes to NPCs
        //      theoretically they should never take an action that is not valid, but this is risky
        CheckResult result = verb.Check(target);
        if (result.value != CheckValue.VALID ) {
            WaywardManager.instance.DisplayMessage(result.messages[0]);
            return null;
        } 

        bool success = false;
        Container container = target as Container;
        if( container != null ) {
            success = EnterContainer(verb, container, data);
        }

        if( !success ) {
            Connection connection = target as Connection;
            if( connection != null ) {
                success = EnterConnection(verb, connection, data);
            }
        }

        return data;
    }
    bool EnterContainer( Verb verb, Container container, Dictionary<string, object> data = null )
    {
        Physical physicalSelf = null;
        try {
            physicalSelf = (Physical)verb.blackboard["physicalSelf"];
        } catch( SystemException e ) {
            WaywardManager.instance.Log($@"<red>TraversalVerb of GameObject '{verb.self.GetName()}' failed in EnterContainer(Container):</red> {e}");
            return false;
        }

        Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);

        bool success = container.GetContents().Attach(parent);

        if( success ) {
            SendMessage( verb, container, data );
        }

        return success;
    }
    bool EnterConnection( Verb verb, Connection connection, Dictionary<string, object> data = null )
    {
        Physical physicalSelf = null;
        try {
            physicalSelf = (Physical)verb.blackboard["physicalSelf"];
        } catch( SystemException e ) {
            WaywardManager.instance.Log($@"<red>TraversalVerb of GameObject '{verb.self.GetName()}' failed in EnterContainer(Connection):</red> {e}");
            return false;
        }

        Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);

        bool success = connection.connectedAttachmentPoint.Attach(parent);

        if( success ) {
            SendMessage( verb, connection.connectedObject, data );
        }

        return success;
    }

    private void SendMessage( Verb verb, GameObject target, Dictionary<string, object> data = null )
    {
        if( data == null ) {
            data = new Dictionary<string, object>();
        }

        // Message for Verbose pages
        data["message"] = new ObservableText($"[0] {verb.displayLabel.ToLower()} into [1].",
            new Tuple<GameObject, string>(verb.self, "name top"),
            new Tuple<GameObject, string>(target, "name")
        );
        data["displayAfter"] = true;

        TimelineManager.instance.OnAction(data);
    }

    public override CheckResult Check( Verb verb, GameObject target )
    {
        Container container = target as Container;
        if( container == null ) { 
            Connection connection = target as Connection;
            if( connection == null ) {
                return new CheckResult(CheckValue.INVALID, $@"Cannot enter that");
            }
                
            return CheckConnection(verb, connection);
        }

        return CheckContainer(verb, container);
    }
    CheckResult CheckConnection( Verb verb, Connection connection )
    {
        Physical physicalSelf = null;
        try {
            physicalSelf = (Physical)verb.blackboard["physicalSelf"];
        } catch( SystemException e ) {
            WaywardManager.instance.Log($@"<red>TraversalVerb of GameObject '{verb.self.GetName()}' failed in CheckConnection(Connection):</red> {e}");
            return new CheckResult(CheckValue.INVALID);
        }

        CheckResult canAttachResult = connection.connectedAttachmentPoint.CanAttach(verb.self);
        if (canAttachResult.value == CheckValue.INVALID) { return canAttachResult; }

        if (physicalSelf.GetVolume() > connection.naturalThroughput) {
            return new CheckResult(CheckValue.INVALID, $@"Cannot fit through there");
        }
        if (physicalSelf.GetVolume() > connection.actualThroughput) {
            return new CheckResult(CheckValue.RESTRICTED, $@"{connection.GetBlockingObject().GetName("upper")} is blocking the way");
        }

        return canAttachResult;
    }
    CheckResult CheckContainer( Verb verb, Container container )
    {
        Physical physicalSelf = null;
        try {
            physicalSelf = (Physical)verb.blackboard["physicalSelf"];
        } catch( SystemException e ) {
            WaywardManager.instance.Log($@"<red>TraversalVerb of GameObject '{verb.self.GetName()}' failed in CheckConnection(Container):</red> {e}");
            return new CheckResult(CheckValue.INVALID);
        }

        if( container.Contains(physicalSelf) ) {
            return new CheckResult(CheckValue.INVALID, $@"Already inside");
        }
        return container.CanContain(physicalSelf);
    }

    public override bool Display( Verb verb, Actor actor, GameObject target, FrameworkContentElement span )
    {
        CheckResult check = verb.Check(target);
        if( check.value >= CheckValue.RESTRICTED ) {
            if( check.value == CheckValue.RESTRICTED ) {
                ContextMenuHelper.AddContextMenuItem( span, WaywardTextParser.ParseAsBlock($@"<gray>{verb.displayLabel}</gray>") , null, false );
            } else {
                ContextMenuHelper.AddContextMenuItem( span, WaywardTextParser.ParseAsBlock(verb.displayLabel) , delegate { return verb.Register(new Dictionary<string, object>(){{ "target", target }}, true); } );
            }
        }

        return true;
    }

    public override bool ParseInput( Verb verb, InputEventArgs inputEventArgs )
    {
        if( inputEventArgs.parsed ) { return true; }
        if( inputEventArgs.words.Length <= 1 ) { return false; }

        WaywardManager.instance.DisplayMessage($"Move to {inputEventArgs.words[1]}");

        return false;
    }
}