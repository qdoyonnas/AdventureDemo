using AdventureCore;
using System.Collections.Generic;

class TraversalVerb : Verb
{
    Physical physicalSelf;

    public TraversalVerb(): base() { }
    public TraversalVerb(Dictionary<string, object> data)
        :base(data) {}
    public TraversalVerb( GameObject self ) : base(self) {}

    protected override void Construct()
    {
        _displayLabel = "Walk";

        actionTime = 500;

        _validInputs = new string[] {
            "walk", "move", "go"
        };
    }
    protected override void OnAssign()
    {
        physicalSelf = self as Physical;
    }

    public override bool Action( Dictionary<string, object> data )
    {
        GameObject target = null;
        if( data.ContainsKey("target") ) {
            target = data["target"] as GameObject;
        }
        if( target == null ) {
            return false;
        }

        if( Check(target) != CheckResult.VALID ) { return false; }

        bool success = false;
        Container container = target as Container;
        if( container != null ) {
            success = EnterContainer(container, data);
        }

        if( !success ) {
            Connection connection = target as Connection;
            if( connection != null ) {
                success = EnterConnection(connection, data);
            }
        }

        return success;
    }
    bool EnterContainer( Container container, Dictionary<string, object> data = null )
    {
        Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);

        bool success = container.GetContents().Attach(parent);

        if( success ) {
            SendMessage( container, data );
        }

        return success;
    }
    bool EnterConnection( Connection connection, Dictionary<string, object> data = null )
    {
        Physical parent = PhysicalUtilities.FindParentPhysical(physicalSelf);

        bool success = connection.secondContainer.Attach(parent);

        if( success ) {
            SendMessage( connection.secondContainer.GetParent(), data );
        }

        return success;
    }

    private void SendMessage( GameObject target, Dictionary<string, object> data = null )
    {
        if( data == null ) {
            data = new Dictionary<string, object>();
        }

        // Message for Verbose pages
        data["message"] = new ObservableText($"[0] {displayLabel.ToLower()} into [1].",
            new Tuple<GameObject, string>(self, "name top"),
            new Tuple<GameObject, string>(target, "name")
        );
        data["displayAfter"] = true;

        TimelineManager.instance.OnAction(data);
    }

    public override CheckResult Check( GameObject target )
    {
        Container container = target as Container;
        if( container == null ) { 
            Connection connection = target as Connection;
            if( connection == null ) { return CheckResult.INVALID; }
                
            return CheckConnection(connection);
        }

        return CheckContainer(container);
    }
    CheckResult CheckConnection( Connection connection )
    {
        if( physicalSelf.GetVolume() > connection.throughput ) { return CheckResult.INVALID; }

        return connection.secondContainer.CanAttach(self);
    }
    CheckResult CheckContainer( Container container )
    {
        if( container.Contains(physicalSelf) ) { return CheckResult.INVALID; }
        return container.CanContain(physicalSelf);
    }

    public override void Display( Actor actor, GameObject target, FrameworkContentElement span )
    {
        CheckResult check = Check(target);
        if( check >= CheckResult.RESTRICTED ) {
            if( check == CheckResult.RESTRICTED ) {
                ContextMenuHelper.AddContextMenuItem( span, WaywardTextParser.ParseAsBlock($@"<gray>{displayLabel}</gray>") , null, false );
            } else {
                ContextMenuHelper.AddContextMenuItem( span, WaywardTextParser.ParseAsBlock(displayLabel) , delegate { return Register(new Dictionary<string, object>(){{ "target", target }}, true); } );
            }
        }
    }

    public override bool ParseInput( InputEventArgs e )
    {
        if( e.parsed ) { return true; }
        if( e.words.Length <= 1 ) { return false; }

        WaywardManager.instance.DisplayMessage($"Move to {e.words[1]}");

        return false;
    }
}