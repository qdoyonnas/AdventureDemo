using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class WaywardWill : Physical
    {
        PhysicalAttachmentPoint contents;
        
        public WaywardWill()
            : base( "Wayward Will" )
        {
            Construct();
        }
        private void Construct()
        {
            description = "the will of the Wayward Engine";

            attachmentTypes.Add( AttachmentType.ALL );

            AddVerb( PossessionType.EMBODIMENT, new PhaseVerb(this) );
            GrabVerb grab = new GrabVerb(-1, -1);
            grab.self = this;
            AddVerb( PossessionType.EMBODIMENT, grab );
            AddVerb( PossessionType.EMBODIMENT, new PossessVerb(this) );
            AddVerb(PossessionType.EMBODIMENT, new CreateVerb(this));
        }
    }
}
