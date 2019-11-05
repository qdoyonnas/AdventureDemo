using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PhysicalAttachmentPoint : AttachmentPoint
    {
        protected double _capacity;
        public double capacity {
            get {
                return _capacity;
            }
        }

        public double filledCapacity {
            get {
                double total = 0;
                foreach( GameObject obj in attachedObjects ) {
                    Physical physical = obj as Physical;
                    if( physical == null ) { throw new System.NullReferenceException("PhysicalAttachmentPoint contains non-physical object"); }

                    total += physical.GetVolume();
                }

                return total;
            }
        }
        public double remainingCapacity {
            get {
                return capacity - filledCapacity;
            }
        }
        
        protected bool isLoose = false; // used to calculate certain physics rules (objects falling off tables when moved etc)

        public PhysicalAttachmentPoint( Dictionary<string, object> data )
            : base( data )
        {
            isLoose = data.ContainsKey("loose") ? (bool)data["loose"] : false;

            _capacity = data.ContainsKey("capacity") ? (double)data["capacity"] : 0;
        }
        public PhysicalAttachmentPoint( GameObject parent, double capacity, params AttachmentType[] types )
            : base( parent, types )
        {
            _capacity = capacity;
        }
        public PhysicalAttachmentPoint( GameObject parent, double capacity, int quantity, params AttachmentType[] types )
            : base(parent, quantity, types)
        {
            _capacity = capacity;
        }

        public Physical GetAttachedAsPhysical( int i )
        {
            if( i < 0 || i >= attachedObjects.Count ) { return null; }

            return attachedObjects[i] as Physical;
        }

        public override CheckResult CanAttach( GameObject obj )
        {
            Physical physical = obj as Physical;
            if( physical == null ) { return CheckResult.INVALID; }

            CheckResult result = base.CanAttach(obj);
            if( result != CheckResult.VALID ) { return result; }

            if( capacity >= 0  && physical.GetVolume() > remainingCapacity ) { return CheckResult.RESTRICTED; }

            return CheckResult.VALID;
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
