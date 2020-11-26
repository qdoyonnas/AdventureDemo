using System;
using System.Collections.Generic;
using System.Windows.Documents;
using WaywardEngine;

namespace AdventureDemo
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
        
        public override bool Action(GameObject target)
        {
            // Open dialog for choosing time
            DialogPage dialogPage =  new DialogPage();
            dialogPage.SetTitle("Wait Time");

            dialogPage.AddEntry("100", () => { WaitAction(100); });
            dialogPage.AddEntry("500", () => { WaitAction(500); });
            dialogPage.AddEntry("1000", () => { WaitAction(1000); });

            WaywardManager.instance.AddPage(dialogPage, WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));
            return true;
        }

        protected bool WaitAction(double duration)
        {
            // Create data dictionary to be passed to observers
            Dictionary<string, object> data = new Dictionary<string, object>();

            actionTime = duration;

            // Message for Verbose pages
            data["message"] = WaywardTextParser.ParseAsBlock($"[0] {displayLabel.ToLower()} for [1].",
                () => { return self.GetData("name top").span; },
                () => { return WaywardTextParser.Parse(actionTime.ToString()); }
            );
            data["turnPage"] = true;
            data["displayAfter"] = true;

            self.OnAction(data);

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
                return Action(null);
            } else {
                double waitTime;
                bool success = double.TryParse(e.parameters[0], out waitTime);
                if( !success ) { return false; }

                WaitAction(waitTime);
            }

            return true;
        }
    }
}
