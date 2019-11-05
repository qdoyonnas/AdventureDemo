using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaywardEngine;

namespace AdventureDemo
{
    class Human : Organism
    {
        public Human( Dictionary<string, object> data )
            : base(data)
        {
            Construct();
        }
        public Human( string name )
            : base(name)
        {
            Construct();
        }

        void Construct()
        {
            Material flesh = GameManager.instance.world.GetMaterial("flesh");
            if( flesh == null ) { return; }

            AddBodyPart(null, new BodyPart(this, "head", 2, Utilities.Pair<Material, double>(flesh, 1)) )
                .AddBodyAttachmentPoint();
                
            AddBodyPart("head", new BodyPart(this, "torso", 40, Utilities.Pair<Material, double>(flesh, 1)) );
        }
    }
}
