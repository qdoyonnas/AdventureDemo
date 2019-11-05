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
        public BodyPart( Organism organism, string name, double volume, params KeyValuePair<Material, double>[] mats )
            : base( name, organism.bodyAttachmentPoint, volume, mats )
        {
            Construct();
        }
        void Construct()
        {
            attachmentTypes.Add(AttachmentType.BODY);

            _bodyParts = new AttachmentPoint(this, -1, AttachmentType.BODY);
        }

        public override List<Verb> CollectVerbs()
        {
            List<Verb> collectedVerbs = base.CollectVerbs();

            foreach( GameObject obj in _bodyParts.GetAttached() ) {
                collectedVerbs.AddRange( obj.CollectVerbs() );
            }

            return collectedVerbs;
        }
        public override void CollectVerbs( Actor actor, PossessionType possession )
        {
            base.CollectVerbs(actor, possession);

            foreach( GameObject obj in _bodyParts.GetAttached() ) {
                obj.CollectVerbs();
            }
        }

        public void AddBodyPart( BodyPart part )
        {
            _bodyParts.Attach(part);
        }
    }
}
