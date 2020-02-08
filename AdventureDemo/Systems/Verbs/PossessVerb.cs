﻿using System;
using WaywardEngine;

namespace AdventureDemo
{
    class PossessVerb : Verb
    {
        public PossessVerb( GameObject self )
            : base(self)
        {
            _displayLabel = "Possess";

            validInputs = new string[] {
                "possess", "control"
            };
        }

        public override bool Action( GameObject target )
        {
            if( Check(target) != CheckResult.VALID ) { return false; }

            self.actor.Control(target);

            return true;
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
