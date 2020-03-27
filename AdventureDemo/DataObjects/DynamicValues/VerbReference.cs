using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
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

            return new KeyValuePair<Verb, PossessionType>(v, possession);
        }
    }
}
