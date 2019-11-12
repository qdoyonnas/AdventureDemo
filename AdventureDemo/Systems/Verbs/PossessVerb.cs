using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PossessVerb : Verb
    {
        public PossessVerb( GameObject self )
            : base(self)
        {
            _displayLabel = "Possess";
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
    }
}
