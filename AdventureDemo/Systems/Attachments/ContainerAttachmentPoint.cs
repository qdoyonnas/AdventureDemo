using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class ContainerAttachmentPoint : PhysicalAttachmentPoint
    {
        protected List<AttachmentPoint> connections;

        public ContainerAttachmentPoint( Dictionary<string, object> data )
            : base( data )
        {
            Construct();
        }
        public ContainerAttachmentPoint( GameObject parent, double capacity )
            : base( parent, capacity, AttachmentType.ALL )
        {
            Construct();
        }
        private void Construct()
        {
            _maxQuantity = -1;
            connections = new List<AttachmentPoint>();
        }

        public virtual Physical[] GetAttachedPhysicals()
        {
             Physical[] physicals = new Physical[attachedObjects.Count];

            for( int i = 0; i < attachedObjects.Count; i++ ) {
                Physical physical = attachedObjects[i] as Physical;
                if( physical == null ) { throw new System.InvalidCastException("ContainerAttachmentPoint contains non-physical object"); }

                physicals[i] = physical;
            }

            return physicals;
        }
    }
}
