using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class LocomotionBodyPart : BodyPart
    {
        public LocomotionBodyPart( Organism organism, string name, double volume, params KeyValuePair<Material, double>[] mats )
            : base( organism, name, volume, mats )
        {
            AddVerb(PossessionType.EMBODIMENT, );
        }
    }
}
