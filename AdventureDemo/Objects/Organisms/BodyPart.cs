using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class BodyPart : Physical
    {
        AttachmentPoint _bodyParts;
        public AttachmentPoint bodyParts {
            get {
                return _bodyParts;
            }
        }

        public BodyPart( Dictionary<string, object> data )
            : base( data )
        {
            Construct();
        }
        public BodyPart( Organism organism, string name, double volume, double weight )
            : base( name, organism.body, volume, weight )
        {
            Construct();
        }
        void Construct()
        {
            attachmentTypes.Add(AttachmentType.BODY);

            _bodyParts = new AttachmentPoint(this, AttachmentType.BODY);
        }


    }
}
