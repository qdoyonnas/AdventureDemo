using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PossessVerbData : VerbData
    {
        public PossessVerbData() { }
        public PossessVerbData(PossessVerbData data) 
            :base(data) { }

        public override object Create()
        {
            return new PossessVerb();
        }
    }
}
