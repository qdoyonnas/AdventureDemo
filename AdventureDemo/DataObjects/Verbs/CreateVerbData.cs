using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class CreateVerbData : VerbData
    {
        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            return new CreateVerb();
        }
    }
}
