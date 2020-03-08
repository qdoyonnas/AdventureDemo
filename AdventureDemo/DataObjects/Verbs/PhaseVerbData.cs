﻿using System;
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

        protected override object CreateInstance()
        {
            return new PhaseVerb();
        }
    }
}