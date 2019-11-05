using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class BodyPart : Physical
    {
        List<BodyAttachmentPoint> _bodyParts;
        public List<BodyAttachmentPoint> bodyParts {
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
            : base( name, volume, mats )
        {
            Construct();
        }
        void Construct()
        {
            attachmentTypes.Add(AttachmentType.BODY);

            _bodyParts = new List<BodyAttachmentPoint>();
        }

        public override List<Verb> CollectVerbs()
        {
            List<Verb> collectedVerbs = base.CollectVerbs();

            foreach( BodyAttachmentPoint point in _bodyParts ) {
                foreach( GameObject obj in point.GetAttached() ) {
                    collectedVerbs.AddRange( obj.CollectVerbs() );
                }
            }

            return collectedVerbs;
        }
        public override void CollectVerbs( Actor actor, PossessionType possession )
        {
            base.CollectVerbs(actor, possession);

            foreach( BodyAttachmentPoint point in _bodyParts ) {
                foreach( GameObject obj in point.GetAttached() ) {
                    obj.CollectVerbs(actor, possession);
                }
            }
        }

        public BodyAttachmentPoint AddBodyAttachmentPoint()
        {
            BodyAttachmentPoint point = new BodyAttachmentPoint(this);
            _bodyParts.Add( point );

            return point;
        }
        public void AddBodyPart( BodyPart part )
        {
            foreach( BodyAttachmentPoint point in _bodyParts ) {
                if( point.CanAttach(part) == CheckResult.VALID ) {
                    point.Attach(part);
                }
            }
        }
    }
}
