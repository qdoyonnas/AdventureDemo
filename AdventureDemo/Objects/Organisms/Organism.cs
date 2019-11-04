using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class Organism : Physical
    {
        AttachmentPoint _body;
        public AttachmentPoint body {
            get {
                return body;
            }
        }

        public Organism( Dictionary<string, object> data )
            : base(data)
        {
            Construct();
        }
        public Organism( string name, AttachmentPoint container )
            : base( name, container )
        {
            Construct();
        }
        public Organism( string name, AttachmentPoint container, double volume )
            : base( name, container, volume )
        {
            Construct();
        }
        public Organism( string name, AttachmentPoint container, double volume, double weight )
            : base( name, container, volume, weight )
        {
            Construct();
        }
        void Construct()
        {
            _body = new AttachmentPoint(this, 1, AttachmentType.BODY);
        }

        public BodyPart AddBodyPart( BodyPart parent, BodyPart part )
        {
            if( parent == null && _body == null ) {
                _body.Attach( part );
            } else {
                parent.AddBodyPart( part );
            }

            return part;
        }
    }
}
