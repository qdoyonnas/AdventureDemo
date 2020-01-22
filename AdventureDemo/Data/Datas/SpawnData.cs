using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class SpawnData : BasicData
    {
        public override object Create()
        {
            return new SpawnList();
        }
    }
}
