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

        public override bool CanAttach( GameObject obj )
        {
            Physical physical = obj as Physical;
            if( physical == null ) { return false; }

            if( physical.GetVolume() > remainingCapacity ) { return false; }

            return base.CanAttach(obj);
        }
    }
}
