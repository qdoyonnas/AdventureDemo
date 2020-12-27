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

        public override Dictionary<string, object> GenerateData(Dictionary<string, object> context = null)
        {
            Dictionary<string, object> data = base.GenerateData(context);

            data["quantity"] = quantity;
            data["capacity"] = capacity;

            return data;
        }

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            return new GrabVerb(GenerateData(context));
        }
    }
}
