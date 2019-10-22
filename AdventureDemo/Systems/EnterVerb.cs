using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdventureDemo
{

    /// Verb for characters to pass through a connection.
    /// </summary>
    class EnterVerb: Verb
    {
        public EnterVerb( GameObject self )
            : base(self)
        {
            _displayLabel = "Enter";
        }

        public override CheckResult Check( GameObject obj )
        {
            // TODO: Same as in PickupVerb. EnterVerb should be greyed out if self cannot fit

            Connection connection = obj as Connection;
            if( connection == null ) { return CheckResult.INVALID; }

            if( connection.CanContain( self ) ) {
                return CheckResult.VALID;
            }

            return CheckResult.INVALID;
        }

        public override void Action( GameObject obj )
        {
            if( Check(obj) != CheckResult.VALID ) { return; }

            Connection connection = obj as Connection;

            connection.Pass(self, self.container);
        }
    }
}
