using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class PhaseVerbData : VerbData
    {
        public PhaseVerbData() { }
        public PhaseVerbData(PhaseVerbData data)
            : base(data) { }

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            return new PhaseVerb();
        }
    }
}
