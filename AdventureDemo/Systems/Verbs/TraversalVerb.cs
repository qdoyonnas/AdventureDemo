using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class TraversalVerb : Verb
    {
        Physical physicalSelf;

        public TraversalVerb( GameObject self )
            : base(self)
        {
            _displayLabel = "Walk";

            physicalSelf = self as Physical;
        }

        public override bool Action( GameObject target )
        {
            return false;
        }

        public override CheckResult Check( GameObject target )
        {
            Container container = target as Container;
            if( container == null ) { 
                Connection connection = target as Connection;
                if( connection == null ) { return CheckResult.INVALID; }
                
                Console.WriteLine("is a connection");
                return CheckConnection(connection);
            }

            return CheckContainer(container);
        }
        CheckResult CheckConnection( Connection connection )
        {
            if( physicalSelf.GetVolume() > connection.throughput ) { return CheckResult.INVALID; }

            return connection.secondContainer.CanAttach(self);
        }
        CheckResult CheckContainer( Container container )
        {
            return container.CanContain(physicalSelf);
        }
    }
}
