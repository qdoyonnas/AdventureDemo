using AdventureCore;
using System.Collections.Generic;

class WaitVerb : Verb
{
    public WaitVerb() : base() { }
    public WaitVerb( GameObject self ) : base(self) { }

    protected override void Construct()
    {
        _displayLabel = "Wait";

        actionTime = 500;

        _validInputs = new string[] { "wait" };
    }

    bool DisplayDialog()
    {
        // Open dialog for choosing time
        DialogPage dialogPage =  new DialogPage();
        dialogPage.SetTitle("Wait Time");

        dialogPage.AddInputPanel((input) => {
            double amount = -1;
            if( !double.TryParse(input, out amount) ) {
                return;
            }

            Register(new Dictionary<string, object>() {{ "duration", amount }}, true);
        });

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["gameObject"] = self;
        data["verb"] = this;
        data["label"] = displayLabel;

        data["duration"] = 10.0;
        dialogPage.AddEntry("10", () => { Register(data, true); });
        data["duration"] = 20.0;
        dialogPage.AddEntry("20", () => { Register(data, true); });
        data["duration"] = 100.0;
        dialogPage.AddEntry("100", () => { Register(data, true); });
        data["duration"] = 250.0;
        dialogPage.AddEntry("250", () => { Register(data, true); });
        data["duration"] = 500.0;
        dialogPage.AddEntry("500", () => { Register(data, true); });
        data["duration"] = 1000.0;
        dialogPage.AddEntry("1000", () => { Register(data, true); });
        data["duration"] = 2000.0;
        dialogPage.AddEntry("2000", () => { Register(data, true); });
        data["duration"] = 4000.0;
        dialogPage.AddEntry("4000", () => { Register(data, true); });
        data["duration"] = 10000.0;
        dialogPage.AddEntry("10000", () => { Register(data, true); });

        WaywardManager.instance.AddPage(dialogPage, WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));

        return true;
    }
        
    public override bool Action( Dictionary<string, object> data )
    {
        double duration = -1;
        if( data.ContainsKey("duration") ) {
            try {
                duration = (double)data["duration"];
            } catch { }
        }
        if( duration == -1 ) { return false; }

        actionTime = duration;

        // Message for Verbose pages
        data["message"] = new ObservableText($"[0] {displayLabel.ToLower()} for {actionTime.ToString()}.",
            new Tuple<GameObject, string>(self, "name top"));
        data["displayAfter"] = false;

        TimelineManager.instance.OnAction(data);

        return true;
    }

    public override CheckResult Check(GameObject target) 
    {
        if( target == self ) {
            return CheckResult.VALID;
        } else {
            return CheckResult.INVALID;
        }
    }

    protected override void OnAssign() {}

    public override bool ParseInput(InputEventArgs e)
    {
        if( e.parsed ) { return true; }

        if( e.parameters.Length == 0 ) {
            return DisplayDialog();
        } else {
            double waitTime;
            bool success = double.TryParse(e.parameters[0], out waitTime);
            if( !success ) { return false; }

            Register(new Dictionary<string, object>() {{ "duration", waitTime }}, true);
        }

        return true;
    }

    public override void Display(Actor actor, GameObject target, FrameworkContentElement span)
    {
        CheckResult check = Check(target);
		if( check >= CheckResult.VALID ) {
            Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();

            Dictionary<string, object> data = new Dictionary<string, object>();
            data["gameObject"] = self;
            data["verb"] = this;
            data["label"] = displayLabel;

            items.Add( WaywardTextParser.ParseAsBlock("Custom...") , delegate { return DisplayDialog(); } );
            data["duration"] = 10.0;
            items.Add( WaywardTextParser.ParseAsBlock("10") , delegate { return Register(data, true); } );
            data["duration"] = 100.0;
            items.Add( WaywardTextParser.ParseAsBlock("100") , delegate { return Register(data, true); } );
            data["duration"] = 500.0;
            items.Add( WaywardTextParser.ParseAsBlock("500") , delegate { return Register(data, true); } );
            data["duration"] = 1000.0;
            items.Add( WaywardTextParser.ParseAsBlock("1000") , delegate { return Register(data, true); } );

            ContextMenuHelper.AddContextMenuHeader( span, WaywardTextParser.ParseAsBlock(displayLabel), items, true );
        }
    }
}