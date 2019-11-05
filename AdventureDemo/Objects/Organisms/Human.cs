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
        public Human( string name, Container container )
            : base(name, container.GetContents())
        {
            Construct();
        }

        void Construct()
        {
            Material flesh = GameManager.instance.world.GetMaterial("Flesh");
            AddBodyPart(null, new BodyPart(this, "Torso", 40, Utilities.Pair<Material, double>(flesh, 1)) );
        }
    }
}
