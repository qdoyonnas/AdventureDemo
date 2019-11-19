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

        public override bool SetContainer( AttachmentPoint newContainer )
        {
            bool result = base.SetContainer(newContainer);
            if( !result ) { return false; }

            foreach( BodyAttachmentPoint point in bodyParts ) {
                if( point.isExternal ) {
                    foreach( Physical obj in point.GetAttachedAsPhysical() ) {
                        obj.SetContainer(newContainer);
                    }
                }
            }

            return true;
        }

        public override double GetVolume(bool total = true)
        {
            double totalVolume = base.GetVolume(total);

            if( total ) {
                foreach( BodyAttachmentPoint point in _bodyParts ) {
                    foreach( Physical part in point.GetAttachedAsPhysical() ) {
                        totalVolume += part.GetVolume();
                    }
                }
            }

            return totalVolume;
        }
        public override double GetWeight(bool total = true)
        {
            double totalWeight = base.GetWeight();

            if( total ) {
                foreach( BodyAttachmentPoint point in _bodyParts ) {
                    foreach( Physical part in point.GetAttachedAsPhysical() ) {
                        totalWeight += part.GetWeight();
                    }
                }
            }

            return totalWeight;
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

        public BodyPart AddBodyAttachmentPoint()
        {
            BodyAttachmentPoint point = new BodyAttachmentPoint(this);
            _bodyParts.Add( point );

            return this;
        }
        public void AddBodyPart( BodyPart part )
        {
            foreach( BodyAttachmentPoint point in _bodyParts ) {
                if( point.CanAttach(part) == CheckResult.VALID ) {
                    point.Attach(part);
                    return;
                }
            }
        }

        public BodyPart GetBodyPart( string path )
        {
            if( path == name ) {
                return this;
            }

            int indexOfStep = path.IndexOf('/');
            string nextStep = indexOfStep != -1 ? path.Substring(0, indexOfStep) : path;
            string nextPath = path.Substring(indexOfStep+1);

            foreach( BodyAttachmentPoint point in _bodyParts ) {
                foreach( GameObject obj in point.GetAttached() ) {
                    if( obj.GetData("name").text == nextStep ) {
                        BodyPart part = obj as BodyPart;
                        if( part != null ) {
                            BodyPart found = part.GetBodyPart(nextPath);
                            if( found != null ) { return found; }
                        }
                    }
                }
            }

            return null;
        }
        public override bool Contains( Physical obj )
        {
            bool result = base.Contains(obj);
            if( result ) { return true; }

            foreach( BodyAttachmentPoint point in bodyParts ) {
                if( point.Contains(obj) ) {
                    return true;
                }
            }

            return false;
        }
        public override bool Externalize( Physical obj )
        {
            bool result = base.Externalize(obj);
            if( result ) { return true; }

            foreach( BodyAttachmentPoint point in bodyParts ) {
                if( point.Contains(obj) ) {
                    return true;
                }
            }

            return false;
        }
    }
}
