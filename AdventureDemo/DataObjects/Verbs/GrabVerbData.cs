using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class GrabVerbData : VerbData
    {
        public override object Create()
        {
            GrabVerb verb = new GrabVerb();
        }
    }
}
