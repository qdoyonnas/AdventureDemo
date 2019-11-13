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

            contents = new PhysicalAttachmentPoint( new Dictionary<string, object>() {
                { "parent", this }, { "name", "Grasp" }, { "quantity", -1 },
                { "types", new AttachmentType[] { AttachmentType.ALL } }
            });
            AddAttachmentPoint(contents);

            AddVerb( PossessionType.EMBODIMENT, new PhaseVerb(this) );
            AddVerb( PossessionType.EMBODIMENT, new GrabVerb(this, contents) );
            AddVerb( PossessionType.EMBODIMENT, new PossessVerb(this) );
        }
    }
}
