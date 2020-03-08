using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class TraversalVerbData : VerbData
    {
        public TraversalVerbData() { }
        public TraversalVerbData( TraversalVerbData data )
            : base(data) { }

        protected override object CreateInstance()
        {
            return new TraversalVerb();
        }
    }
}
