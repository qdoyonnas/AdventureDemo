using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{

    /// Verb for characters to pass through a connection.
    /// </summary>
    class EnterVerb: Verb
    {
        public EnterVerb( GameObject self )
            : base(self)
        { }

        public override bool Check( GameObject obj )
        {
            // TODO: Same as in PickupVerb. EnterVerb should be greyed out if self cannot fit

            Connection connection = obj as Connection;

            if( connection == null ) { return false; }
            return connection.CanContain( self );
        }

        public override void Action( GameObject obj )
        {
            if( !Check(obj) ) { return; }

            Connection connection = obj as Connection;

            connection.Pass(self, self.container);
        }
    }
}
