﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class Organism : PhysicalAmalgam
    {
        public Organism( Dictionary<string, object> data )
            : base(data)
        {
        }
        public Organism( string name )
            : base( name )
        {
        }
    }
}