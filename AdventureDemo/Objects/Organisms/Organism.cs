using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class Organism : Physical
    {
        BodyAttachmentPoint _body;
        public BodyAttachmentPoint bodyAttachmentPoint {
             get {
                return _body;
            }
        }

        public Organism( Dictionary<string, object> data )
            : base(data)
        {
            Construct();
        }
        public Organism( string name )
            : base( name )
        {
            Construct();
        }
        public Organism( string name, params KeyValuePair<Material, double>[] mats )
            : base( name, 0, mats )
        {
            Construct();
        }
        void Construct()
        {
            _body = new BodyAttachmentPoint(this);
        }

        public override bool SetContainer( AttachmentPoint newContainer )
        {
            bool result = base.SetContainer(newContainer);
            if( !result ) { return false; }

            if( bodyAttachmentPoint.isExternal ) {
                foreach( Physical obj in bodyAttachmentPoint.GetAttachedAsPhysical() ) {
                    obj.SetContainer(newContainer);
                }
            }

            return true;
        }

        public override double GetVolume(bool total = true)
        {
            double totalVolume = base.GetVolume(total);

            if( total && _body != null ) {
                Physical body = _body.GetAttachedAsPhysical(0);
                if( body != null ) {
                    totalVolume += body.GetVolume();
                }
            }

            return totalVolume;
        }
        public override double GetWeight(bool total = true)
        {
            double totalWeight = base.GetWeight(total);

            if( total ) {
                if( _body != null && _body.GetAttachedCount() != 0 ) {
                    totalWeight += GetBody().GetWeight();
                }
            }

            return totalWeight;
        }

        public override List<Verb> CollectVerbs()
        {
            List<Verb> collectedVerbs = base.CollectVerbs();

            foreach( GameObject part in _body.GetAttached() ) {
                collectedVerbs.AddRange( part.CollectVerbs() );
            }

            return collectedVerbs;
        }
        public override void CollectVerbs( Actor actor, PossessionType possession )
        {
            base.CollectVerbs(actor, possession);

            foreach( GameObject part in _body.GetAttached() ) {
                part.CollectVerbs( actor, possession );
            }
        }

        public BodyPart GetBody()
        {
            if( _body == null || _body.GetAttachedCount() == 0 ) { return null; }
            return _body.GetAttached(0) as BodyPart;
        }
        public BodyPart GetBody(string path)
        {
            if( _body == null ) { return null; }
            if( string.IsNullOrEmpty(path) ) { return GetBody(); }

            return GetBody().GetBodyPart(path);
        }
        public BodyPart AddBodyPart( BodyPart parent, BodyPart part )
        {
            if( _body == null ) { return null; }

            if( parent == null ) {
                _body.Attach( part );
            } else {
                parent.AddBodyPart( part );
            }

            return part;
        }
        public BodyPart AddBodyPart( string path, BodyPart part )
        {
            BodyPart parent = GetBody(path);

            return AddBodyPart(parent, part);
        }

        public override bool Contains( Physical obj )
        {
            bool result = base.Contains(obj);
            if( result ) { return true; }

            if( _body != null && _body.Contains(obj) ) {
                return true;
            }

            return false;
        }
        public override bool Externalize( Physical obj )
        {
            bool result = base.Externalize(obj);
            if( result ) { return true; }

            if( _body != null && _body.Contains(obj) ) {
                return true;
            }

            return false;
        }

        public override List<DescriptivePageSection> DisplayDescriptivePage()
        {
            List<DescriptivePageSection> sections = base.DisplayDescriptivePage();

            sections.Add( new OrganismDescriptivePageSection() );

            return sections;
        }
    }
}
