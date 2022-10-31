using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

//css_include ../Data/Verbs/Scripts/defaultVerb.cs;

class WaitVerb : DefaultVerb
{
    public override bool Construct(Verb verb, Dictionary<string, object> data)
    {
        verb.SetType("WaitVerb");
        verb.displayLabel = "Wait";

        verb.blackboard["actionTime"] = 500.0;

        verb.AddValidInput("wait");

        return true;
    }

    bool DisplayDialog(Verb verb)
    {
        // Open dialog for choosing time
        DialogPage dialogPage =  new DialogPage();
        dialogPage.SetTitle("Wait Time");

        dialogPage.AddInputPanel((input) => {
            double amount = -1;
            if( !double.TryParse(input, out amount) ) {
                return;
            }

            verb.Register(new Dictionary<string, object>() {{ "duration", amount }}, true);
        });

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["gameObject"] = verb.self;
        data["verb"] = verb;
        data["label"] = verb.displayLabel;

        data["duration"] = 10.0;
        dialogPage.AddEntry("10", () => { verb.Register(data, true); });
        data["duration"] = 20.0;
        dialogPage.AddEntry("20", () => { verb.Register(data, true); });
        data["duration"] = 100.0;
        dialogPage.AddEntry("100", () => { verb.Register(data, true); });
        data["duration"] = 250.0;
        dialogPage.AddEntry("250", () => { verb.Register(data, true); });
        data["duration"] = 500.0;
        dialogPage.AddEntry("500", () => { verb.Register(data, true); });
        data["duration"] = 1000.0;
        dialogPage.AddEntry("1000", () => { verb.Register(data, true); });
        data["duration"] = 2000.0;
        dialogPage.AddEntry("2000", () => { verb.Register(data, true); });
        data["duration"] = 4000.0;
        dialogPage.AddEntry("4000", () => { verb.Register(data, true); });
        data["duration"] = 10000.0;
        dialogPage.AddEntry("10000", () => { verb.Register(data, true); });

        WaywardManager.instance.AddPage(dialogPage, WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));

        return true;
    }
        
    public override bool Action( Verb verb, Dictionary<string, object> data )
    {
        double duration = -1;
        if( data.ContainsKey("duration") ) {
            try {
                duration = (double)data["duration"];
            } catch { }
        }
        if( duration == -1 ) { return false; }

        verb.blackboard["actionTime"] = duration;

        // Message for Verbose pages
        data["message"] = new ObservableText($"[0] {verb.displayLabel.ToLower()} for {duration.ToString()}.",
            new Tuple<GameObject, string>(verb.self, "name top"));
        data["displayAfter"] = false;

        TimelineManager.instance.OnAction(data);

        return true;
    }

    public override CheckResult Check(Verb verb, GameObject target) 
    {
        if( target == verb.self ) {
            return CheckResult.VALID;
        } else {
            return CheckResult.INVALID;
        }
    }

    public override bool ParseInput(Verb verb, InputEventArgs inputEventArgs)
    {
        if( inputEventArgs.parsed ) { return true; }

        if( inputEventArgs.parameters.Length == 0 ) {
            return DisplayDialog(verb);
        } else {
            double waitTime;
            bool success = double.TryParse(inputEventArgs.parameters[0], out waitTime);
            if( !success ) { return false; }

            verb.Register(new Dictionary<string, object>() {{ "duration", waitTime }}, true);
        }

        return true;
    }

    public override bool Display(Verb verb, Actor actor, GameObject target, FrameworkContentElement span)
    {
        CheckResult check = verb.Check(target);
		if( check >= CheckResult.VALID ) {
            Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["gameObject"] = verb.self;
            data["verb"] = verb;
            data["label"] = verb.displayLabel;

            items.Add( WaywardTextParser.ParseAsBlock("Custom...") , delegate { return DisplayDialog(verb); } );
            data["duration"] = 10.0;
            items.Add( WaywardTextParser.ParseAsBlock("10") , delegate { return verb.Register(data, true); } );
            data["duration"] = 100.0;
            items.Add( WaywardTextParser.ParseAsBlock("100") , delegate { return verb.Register(data, true); } );
            data["duration"] = 500.0;
            items.Add( WaywardTextParser.ParseAsBlock("500") , delegate { return verb.Register(data, true); } );
            data["duration"] = 1000.0;
            items.Add( WaywardTextParser.ParseAsBlock("1000") , delegate { return verb.Register(data, true); } );

            ContextMenuHelper.AddContextMenuHeader( span, WaywardTextParser.ParseAsBlock(verb.displayLabel), items, true );
        }

        return true;
    }
}