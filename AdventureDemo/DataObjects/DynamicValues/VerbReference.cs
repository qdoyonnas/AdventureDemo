using System;
using System.Collections.Generic;

namespace AdventureCore
{
    class VerbReference
    {
        public DataReference verb;
        public PossessionType possession;

        public VerbReference() { }
        public VerbReference( VerbReference value )
        {
            verb = new DataReference(value.verb.value);
            possession = value.possession;
        }

        public KeyValuePair<Verb, PossessionType> GetValue(Dictionary<string, object> context = null)
        {
            Verb v = verb.LoadData<Verb>(typeof(VerbData), context);
            if( v == null ) {
                WaywardEngine.WaywardManager.instance.Log($@"<red>ERROR: Verb failed creating VerbData object with reference {verb.value}</red>");
            }

            return new KeyValuePair<Verb, PossessionType>(v, possession);
        }
    }
}
