using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdventureDemo
{
    /// <summary>
    /// Verb for a container to place an object within its inventory.
    /// </summary>
    class PickupVerb : Verb
    {
        new Container self;

        public PickupVerb( Container self )
            : base(self)
        {
            _displayLabel = "Pick Up";
            this.self = self;
        }

        /// <summary>
        /// Determines if obj can be placed into self inventory.
        /// </summary>
        /// <param name="data">obj: GameObject</param>
        public override CheckResult Check( GameObject obj )
        {
            // TODO: Multiple levels of action validity:
            //          Not a valid object type - option does not appear in context menu
            //          Object fails criteria - greyed option in context menu
            //          Valid - enabled option in context menu
            //
            //  Example: PickupVerb should display a greyed out option if the container
            //      is presently too full for object

            if( obj == null || obj == self || obj.container == null ) { return CheckResult.INVALID; }

            if( obj.container == self.container ) {
                return CheckResult.VALID;
            } else {
                GameObject container = obj.container as GameObject;
                if( container != null ) {
                    if( Check(container) >= CheckResult.RESTRICTED ) {
                        return CheckResult.VALID;
                    }
                }
            }

            return CheckResult.INVALID;
        }

        /// <summary>
        /// Places obj into self inventory if possible.
        /// </summary>
        /// <param name="data">obj: GameObject</param>
        public override void Action( GameObject obj )
        {
            if( Check(obj) != CheckResult.VALID ) { return; }

            obj.SetContainer(self);
        }
    }
}
