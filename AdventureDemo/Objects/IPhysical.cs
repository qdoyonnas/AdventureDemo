using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    interface IPhysical
    {
        double GetVolume();
        double GetWeight();

        GameObjectData GetDescriptiveWeight( string[] parameters );
        GameObjectData GetDescriptiveVolume( string[] parameters );
    }
}
