using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PhaseVerbData : VerbData
    {
        public PhaseVerbData() { }
        public PhaseVerbData(PhaseVerbData data)
            : base(data) { }

        public override object Create()
        {
            return new PhaseVerb();
        }
    }
}
