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
            // Create data dictionary to be passed to observers
            Dictionary<string, object> data = new Dictionary<string, object>();

            // Message for Verbose pages
            data["message"] = WaywardTextParser.ParseAsBlock($"[0] {displayLabel.ToLower()}.",
                () => { return self.GetData("name top").span; },
                () => { return target.GetData("name").span; }
            );
            data["turnPage"] = true;
            data["displayAfter"] = true;

            self.OnAction(data);

            return true;
        }

        public override CheckResult Check(GameObject target) 
            { return CheckResult.VALID; }

        protected override void OnAssign() {}
    }
}
