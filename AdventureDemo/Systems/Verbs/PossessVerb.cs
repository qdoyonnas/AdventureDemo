using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class PossessVerb : Verb
    {
        public PossessVerb() : base() { }
        public PossessVerb( GameObject self )
            : base(self) {}

        protected override void Construct()
        {
            _displayLabel = "Possess";

            _validInputs = new string[] {
                "possess", "control"
            };
        }
        protected override void OnAssign() {}

        public override bool Action( GameObject target )
        {
            if( Check(target) != CheckResult.VALID ) { return false; }

            // Create data dictionary to be passed to observers
            Dictionary<string, object> data = new Dictionary<string, object>();

            // Message for Verbose pages
            data["message"] = new ObservableText($"[0] {displayLabel.ToLower()} [1].",
                new Tuple<GameObject, string>(self, "name top"),
                new Tuple<GameObject, string>(target, "name")
            );
            data["turnPage"] = true;
            data["displayAfter"] = true;

            self.OnAction(data);
            self.actor.Control(target);

            return true;
        }

        public override CheckResult Check( GameObject target )
        {
            if( target == self 
                || target.CollectVerbs().Count <= 0 ) 
            {
                return CheckResult.INVALID;
            }

            return CheckResult.VALID;
        }

        public override bool ParseInput( InputEventArgs e )
        {
            if( e.parsed ) { return true; }
            if( e.words.Length <= 1 ) { return false; }

            WaywardManager.instance.DisplayMessage($"Take control of {e.words[1]}");

            return true;
        }
    }
}
