using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class TraversalVerbData : VerbData
    {
        public override object Create()
        {
            return new TraversalVerb();
        }
    }
}
