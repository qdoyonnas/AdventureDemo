using System;
using System.Collections.Generic;

namespace AdventureCore
{
    // XXX: This feel unnecessary and causes 
    //      "behaviours": [
    //          {
    //              "behaviour": {...}
    //          }
    //      ]
	class BehaviourReference
	{
        public DataReference behaviour;

		public BehaviourReference() { }
        public BehaviourReference( BehaviourReference value )
        {
            behaviour = new DataReference(value.behaviour.value);
        }

        public Behaviour GetValue(Dictionary<string, object> context = null)
        {
            Behaviour b = behaviour.LoadData<Behaviour>(typeof(BehaviourData), context);
            if (b == null) {
                WaywardEngine.WaywardManager.instance.Log($@"<red>ERROR: Behaviour failed creating BehaviourData object with reference {behaviour.value}</red>");
            }

            return b;
        }
	}
}
