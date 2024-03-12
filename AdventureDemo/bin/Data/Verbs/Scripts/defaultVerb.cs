using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;
using AdventureCore;

public class DefaultVerb : IVerbScript {
    public virtual bool Construct(Verb verb, Dictionary<string, object> data)
    {
        if( data.ContainsKey("actionTime") ) {
            try {
                verb.blackboard["actionTime"] = (Double)data["actionTime"];
            } catch { }
        } else {
            verb.blackboard["actionTime"] = (Double)0;
        }

        return true;
    }
    public virtual bool OnAssign(Verb verb)
    {
         return true; 
    }

    public virtual bool Action( Verb verb, Dictionary<string, object> data )
    {
        WaywardEngine.WaywardManager.instance.Log($@"<yellow>Verb '{verb.displayLabel}' ran default action.</yellow>");
        return false;
    }

    public virtual CheckResult Check( Verb verb, GameObject target )
    {
        return new CheckResult(CheckValue.INVALID);
    }

    public virtual bool Register( Verb verb, Dictionary<string, object> data, bool fromPlayer = false )
    {
        if( !data.ContainsKey("gameObject") ) { data["gameObject"] = verb.self; }
        if( !data.ContainsKey("verb") ) { data["verb"] = verb; }
        if( !data.ContainsKey("label") ) { data["label"] = verb.displayLabel; }

        Double actionTime = 0;
        try {
            actionTime = (Double)verb.blackboard["actionTime"];
        } catch( SystemException e ) { }

        bool success = false;
        try {
            success = TimelineManager.instance.RegisterEvent( (d) => { verb.Action(d); }, data, actionTime);
        } catch( SystemException e ) {
            WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed registering action:</red> {e}");
            return false;
        }

        // XXX: Set the game objects current action here

        if( fromPlayer ) {
            if( success ) {
                try {
                    GameManager.instance.Update(actionTime);
                } catch( SystemException e ) {
                    WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed updating GameManager:</red> {e}");
                    return false;
                }
            }
            WaywardManager.instance.Update();
        }

        return success;
    }

    public virtual bool Display( Verb verb, Actor actor, GameObject target, FrameworkContentElement span )
    {
        CheckResult check = verb.Check(target);
		if( check.value >= CheckValue.RESTRICTED ) {
            Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();
            if( check.value == CheckValue.RESTRICTED ) {
                items.Add( WaywardTextParser.ParseAsBlock($@"<gray>{verb.displayLabel}</gray>") , null );
            } else {
                items.Add( WaywardTextParser.ParseAsBlock(verb.displayLabel) , delegate { return verb.Register(new Dictionary<string, object>(){{ "target", target }}, true); } );
            }
            ContextMenuHelper.AddContextMenuHeader(span, new TextBlock(verb.self.GetData("name upper").span), items, check.value != CheckValue.RESTRICTED);
        }

        return true;
    }

    public virtual bool AddVerb( Verb verb, Actor actor )
    {
        if( actor.HasVerb(verb.type) ) { return false; }
        actor.AddVerb(verb);

        return true;
    }

    public virtual bool ParseInput( Verb verb, InputEventArgs inputEventArgs )
    {
        if( inputEventArgs.parsed ) { return true; }

        string message = $"Verb Found:\n{verb.displayLabel} with params:";

        for( int i = 1; i < inputEventArgs.words.Length; i++ ) {
            message += $"\n{inputEventArgs.words[i]}";
        }

        WaywardManager.instance.DisplayMessage(message);

        return true;
    }
}
