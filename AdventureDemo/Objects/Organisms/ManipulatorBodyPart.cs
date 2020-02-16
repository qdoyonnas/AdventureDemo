using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class ManipulatorBodyPart : BodyPart
    {
        PhysicalAttachmentPoint grasp;

        public ManipulatorBodyPart( Dictionary<string, object> data )
            : base( data )
        {
            Construct();
        }
        public ManipulatorBodyPart( Organism organism, string name, double volume, params KeyValuePair<Material, double>[] mats )
            : base( organism, name, volume, mats )
        {
            Construct();
        }
        void Construct()
        {
            grasp = new PhysicalAttachmentPoint(this, -1, 1, AttachmentType.ALL);
            AddAttachmentPoint(grasp);
            //AddVerb( PossessionType.EMBODIMENT, new GrabVerb(this, grasp) );
        }
    }
}
