using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class CreateVerbData : VerbData
    {
        protected override object CreateInstance()
        {
            return new CreateVerb();
        }
    }
}
