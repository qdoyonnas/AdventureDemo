using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
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

            dialogPage.AddEntry("10", () => { Register(new Dictionary<string, object>() {{ "duration", 10.0 }}, true); });
            dialogPage.AddEntry("20", () => { Register(new Dictionary<string, object>() {{ "duration", 20.0 }}, true); });
            dialogPage.AddEntry("100", () => { Register(new Dictionary<string, object>() {{ "duration", 100.0 }}, true); });
            dialogPage.AddEntry("250", () => { Register(new Dictionary<string, object>() {{ "duration", 250.0 }}, true); });
            dialogPage.AddEntry("500", () => { Register(new Dictionary<string, object>() {{ "duration", 500.0 }}, true); });
            dialogPage.AddEntry("1000", () => { Register(new Dictionary<string, object>() {{ "duration", 1000.0 }}, true); });
            dialogPage.AddEntry("2000", () => { Register(new Dictionary<string, object>() {{ "duration", 2000.0 }}, true); });
            dialogPage.AddEntry("4000", () => { Register(new Dictionary<string, object>() {{ "duration", 4000.0 }}, true); });
            dialogPage.AddEntry("10000", () => { Register(new Dictionary<string, object>() {{ "duration", 10000.0 }}, true); });

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

            // Create data dictionary to be passed to observers
            Dictionary<string, object> actionData = new Dictionary<string, object>();

            actionTime = duration;

            // Message for Verbose pages
            actionData["message"] = new ObservableText($"[0] {displayLabel.ToLower()} for {actionTime.ToString()}.",
                new Tuple<GameObject, string>(self, "name top"));
            actionData["turnPage"] = true;
            actionData["displayAfter"] = true;

            TimelineManager.instance.OnAction(actionData);

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

                items.Add( WaywardTextParser.ParseAsBlock("Custom...") , delegate { return DisplayDialog(); } );
                items.Add( WaywardTextParser.ParseAsBlock("10") , delegate { return Register(new Dictionary<string, object>() {{ "duration", 10.0 }}, true); } );
                items.Add( WaywardTextParser.ParseAsBlock("100") , delegate { return Register(new Dictionary<string, object>() {{ "duration", 100.0 }}, true); } );
                items.Add( WaywardTextParser.ParseAsBlock("500") , delegate { return Register(new Dictionary<string, object>() {{ "duration", 500.0 }}, true); } );
                items.Add( WaywardTextParser.ParseAsBlock("1000") , delegate { return Register(new Dictionary<string, object>() {{ "duration", 1000.0 }}, true); } );

                ContextMenuHelper.AddContextMenuHeader( span, WaywardTextParser.ParseAsBlock(displayLabel), items, true );
            }
        }
    }
}
