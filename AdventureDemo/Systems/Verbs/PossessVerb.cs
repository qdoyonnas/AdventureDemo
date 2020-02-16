using System;
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
        protected override void InitVerb() {}

        public override bool Action( GameObject target )
        {
            if( Check(target) != CheckResult.VALID ) { return false; }

            self.actor.Control(target);

            return base.Action(target);
        }

        public override CheckResult Check( GameObject target )
        {
            if( target.CollectVerbs().Count > 0 ) {
                return CheckResult.VALID;
            }

            return CheckResult.INVALID;
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
