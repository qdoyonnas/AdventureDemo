using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

//css_include ../Data/Verbs/Scripts/defaultVerb.cs;

class EmoteVerb : DefaultVerb
{
	public override bool Construct(Verb verb, Dictionary<string, object> data)
    {
        verb.SetType("EmoteVerb");
        verb.displayLabel = "Emote";

        verb.blackboard["actionTime"] = 10.0;

        verb.AddValidInput("do", "emote");

        return true;
    }

	bool DisplayDialog(Verb verb)
	{
        // Open dialog for choosing time
        DialogPage dialogPage =  new DialogPage();
        dialogPage.SetTitle("Action");

        dialogPage.AddInputPanel((input) => {
            verb.Register(new Dictionary<string, object>() {{ "message", input }}, true);
        });

        WaywardManager.instance.AddPage(dialogPage, WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));

        return true;
	}

	public override Dictionary<string, object> Action(Verb verb, Dictionary<string, object> data)
	{
		string message = null;
		if( data.ContainsKey("message") ) {
			message = data["message"] as string;
		}
		if( message == null ) { return null; }

        // Message for Verbose pages
        data["message"] = new ObservableText($"[0] { message }.", 
            new Tuple<GameObject, string>(verb.self, "name top")
        );
        data["turnPage"] = false;
        data["displayAfter"] = false;

        return data;
	}

	public override CheckResult Check(Verb verb, GameObject target)
	{
		return new CheckResult(CheckValue.VALID);
	}

	public override bool ParseInput(Verb verb, InputEventArgs inputEventArgs)
    {
        if( inputEventArgs.parsed ) { return true; }

        if( inputEventArgs.parameters.Length == 0 ) {
            return DisplayDialog(verb);
        } else {
            string message = inputEventArgs.parameterInput;

            verb.Register(new Dictionary<string, object>() {{ "message", message }}, true);
        }

        return true;
    }

    public override bool Display(Verb verb, Actor actor, GameObject target, FrameworkContentElement span)
    {
        CheckResult check = verb.Check(target);
		if( check.value >= CheckValue.VALID ) {
            ContextMenuHelper.AddContextMenuItem( span, verb.displayLabel, delegate { DisplayDialog(verb); return false; } );
        }

        return true;
    }
}