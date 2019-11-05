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

        public override double GetVolume()
        {
            if( _body == null ) { return 0; }
            Physical body = _body.GetAttachedAsPhysical(0);

            if( body != null ) {
                return body.GetVolume();
            }

            return 0;
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
            return _body.GetAttached(0) as BodyPart;
        }
        public BodyPart GetBody(string path)
        {
            if( _body == null ) { return null; }
            if( string.IsNullOrEmpty(path) ) { return GetBody(); }

            return null;
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
            if( parent == null ) { return null; }

            return AddBodyPart(parent, part);
        }


        public override List<DescriptivePageSection> DisplayDescriptivePage()
        {
            List<DescriptivePageSection> sections = base.DisplayDescriptivePage();

            sections.Add( new OrganismDescriptivePageSection() );

            return sections;
        }
    }
}
