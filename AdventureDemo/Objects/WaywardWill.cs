using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class WaywardWill : Physical
    {
        public WaywardWill( Container container )
            : base( "Wayward Will", container )
        {
            Construct();
        }
        public WaywardWill( AttachmentPoint container )
            : base( "Wayward Will", container )
        {
            Construct();
        }
        private void Construct()
        {
            description = "the will of the Wayward Engine";

            attachmentTypes.Add( AttachmentType.ALL );

            verbs.Add( PossessionType.EMBODIMENT, new Verb[] {
                new PhaseVerb(this)
            } );
        }
    }
}
