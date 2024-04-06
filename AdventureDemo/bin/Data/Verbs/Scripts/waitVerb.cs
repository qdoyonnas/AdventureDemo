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
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["gameObject"] = verb.self;
            data["verb"] = verb;
            data["label"] = verb.displayLabel;
            data["duration"] = amount;

            verb.Register(data, true);
        });

        AddDialogEntry(verb, dialogPage, "10", 10.0);
        AddDialogEntry(verb, dialogPage, "20", 20.0);
        AddDialogEntry(verb, dialogPage, "100", 100.0);
        AddDialogEntry(verb, dialogPage, "250", 250.0);
        AddDialogEntry(verb, dialogPage, "500", 500.0);
        AddDialogEntry(verb, dialogPage, "1000", 1000.0);
        AddDialogEntry(verb, dialogPage, "2000", 2000.0);
        AddDialogEntry(verb, dialogPage, "4000", 4000.0);
        AddDialogEntry(verb, dialogPage, "10000", 10000.0);

        WaywardManager.instance.AddPage(dialogPage, WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));

        return true;
    }

    public void AddDialogEntry(Verb verb, DialogPage dialogPage, string label, double duration)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["gameObject"] = verb.self;
        data["verb"] = verb;
        data["label"] = verb.displayLabel;
        data["duration"] = duration;

        dialogPage.AddEntry(label, () => { verb.Register(data, true); });
    }

    public override Dictionary<string, object> Action( Verb verb, Dictionary<string, object> data )
    {
        double duration = -1;
        if( data.ContainsKey("duration") ) {
            try {
                duration = (double)data["duration"];
            } catch { }
        }
        if( duration == -1 ) { return null; }

        verb.blackboard["actionTime"] = duration;

        // Message for Verbose pages
        data["message"] = new ObservableText($"[0] {verb.displayLabel.ToLower()} for {duration.ToString()}.",
            new Tuple<GameObject, string>(verb.self, "name top"));
        data["displayAfter"] = false;

        return data;
    }

    public override CheckResult Check(Verb verb, GameObject target) 
    {
        if( target == verb.self ) {
            return new CheckResult(CheckValue.VALID);
        } else {
            return new CheckResult(CheckValue.INVALID);
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
		if( check.value >= CheckValue.VALID ) {
            Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();

            items.Add(WaywardTextParser.ParseAsBlock("Custom..."), delegate { return DisplayDialog(verb); });
            AddDisplayItem(verb, items, "10", 10.0);
            AddDisplayItem(verb, items, "100", 100.0);
            AddDisplayItem(verb, items, "500", 500.0);
            AddDisplayItem(verb, items, "1000", 1000.0);

            ContextMenuHelper.AddContextMenuHeader( span, WaywardTextParser.ParseAsBlock(verb.displayLabel), items, true );
        }

        return true;
    }

    public void AddDisplayItem(Verb verb, Dictionary<TextBlock, ContextMenuAction> items, string label, double duration)
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data["gameObject"] = verb.self;
        data["verb"] = verb;
        data["label"] = verb.displayLabel;
        data["duration"] = duration;

        items.Add(WaywardTextParser.ParseAsBlock(label), delegate { return verb.Register(data, true); });
    }
}