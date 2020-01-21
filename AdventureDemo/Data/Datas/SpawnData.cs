using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class SpawnData : BasicData
    {
        public SpawnList Create()
        {
            return new SpawnList();
        }
    }
}
