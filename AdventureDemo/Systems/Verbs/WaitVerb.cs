using System;
using System.Collections.Generic;
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
        }

        protected bool WaitAction()
        {
            // Create data dictionary to be passed to observers
            Dictionary<string, object> data = new Dictionary<string, object>();

            // Message for Verbose pages
            data["message"] = WaywardTextParser.ParseAsBlock($"[0] {displayLabel.ToLower()}.",
                () => { return self.GetData("name top").span; }
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

                WaitAction();
            }

            return true;
        }
    }
}
