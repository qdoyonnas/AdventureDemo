using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class WaywardWill : Physical
    {
        AttachmentPoint contents;

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

            contents = new AttachmentPoint( new Dictionary<string, object>() {
                { "parent", this }, { "name", "Grasp" }, { "quantity", -1 },
                { "types", new AttachmentType[] { AttachmentType.ALL } }
            });
            AddAttachmentPoint(contents);

            verbs.Add( PossessionType.EMBODIMENT, new Verb[] {
                new PhaseVerb(this),
                new GrabVerb(this, contents)
            } );
        }
    }
}
