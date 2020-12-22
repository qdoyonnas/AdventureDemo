using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class GrabVerbData : VerbData
    {
        public int quantity = 1;
        public double capacity = -1;

        public GrabVerbData() { }
        public GrabVerbData( GrabVerbData data )
        {
            quantity = data.quantity;
            capacity = data.capacity;
        }

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            return new GrabVerb(quantity, capacity);
        }
    }
}
