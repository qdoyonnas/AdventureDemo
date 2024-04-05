using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

//css_include ../Data/Verbs/Scripts/defaultVerb.cs;

class PossessVerb : DefaultVerb
{
    public override bool Construct(Verb verb, Dictionary<string, object> data)
    {
        verb.SetType("PossessVerb");
        verb.displayLabel = "Possess";

        verb.AddValidInput("possess", "control");

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

        CheckResult result = Check(verb, target)
        if ( result.value != CheckValue.VALID ) {
            WaywardManager.instance.DisplayMessage(result.messages[0]);
            return null;
        }


        // Message for Verbose pages
        data["message"] = new ObservableText($"[0] {verb.displayLabel.ToLower()} [1].",
            new Tuple<GameObject, string>(verb.self, "name top"),
            new Tuple<GameObject, string>(target, "name")
        );
        data["turnPage"] = true;
        data["displayAfter"] = true;

        verb.self.actor.Control(target);

        return data;
    }

    public override CheckResult Check( Verb verb, GameObject target )
    {
        if( target == verb.self 
            || target.CollectVerbs().Count <= 0 ) 
        {
            return new CheckResult(CheckValue.INVALID);
        }

        return new CheckResult(CheckValue.VALID);
    }

    public override bool ParseInput( Verb verb, InputEventArgs e )
    {
        if( e.parsed ) { return true; }
        if( e.words.Length <= 1 ) { return false; }

        WaywardManager.instance.DisplayMessage($"Take control of {e.words[1]}");

        return true;
    }
}