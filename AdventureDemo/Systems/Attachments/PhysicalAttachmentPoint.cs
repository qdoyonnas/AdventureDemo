using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    public class PhysicalAttachmentPoint : AttachmentPoint
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
        
        protected bool _isExternal = false;
        public bool isExternal {
            get {
                return _isExternal;
            }
        }

        public PhysicalAttachmentPoint( Dictionary<string, object> data )
            : base( data )
        {
            _isExternal = data.ContainsKey("external") ? (bool)data["external"] : false;

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

        public bool SetCapacity( double capacity )
        {
            if( capacity < filledCapacity ) { return false; }

            _capacity = capacity;

            return true;
        }

        public Physical GetAttachedAsPhysical( int i )
        {
            if( i < 0 || i >= attachedObjects.Count ) { return null; }

            return attachedObjects[i] as Physical;
        }
        public Physical[] GetAttachedAsPhysical()
        {
            Physical[] physicals = new Physical[attachedObjects.Count];
            for( int i = 0; i < attachedObjects.Count; i++ ) {
                physicals[i] = attachedObjects[i] as Physical;
            }

            return physicals;
        }

        public override bool Contains( GameObject obj )
        {
            bool result = attachedObjects.Contains(obj);

            if( result ) { return true; }

            Physical physical = obj as Physical;
            if( physical == null ) { return false; }
            foreach( GameObject attached in attachedObjects ) {
                PhysicalAmalgam amalgam = attached as PhysicalAmalgam;
                if( amalgam == null ) { continue; }

                if( amalgam.Contains(physical) ) {
                    return true;
                }
            }

            return false;
        }
        public override CheckResult CanAttach( GameObject obj )
        {
            if( Contains(obj) ) { return CheckResult.INVALID; }

            Physical physical = obj as Physical;
            if( physical == null ) { return CheckResult.INVALID; }

            CheckResult result = base.CanAttach(obj);
            if( result != CheckResult.VALID ) { return result; }

            if( capacity >= 0  && physical.GetVolume() >= remainingCapacity ) { return CheckResult.RESTRICTED; }

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
