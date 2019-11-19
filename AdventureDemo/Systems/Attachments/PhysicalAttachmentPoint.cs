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
            Physical physical = obj as Physical;
            if( physical == null ) { return false; }

            if( attachedObjects.Contains(obj) ) { return true; }

            foreach( Physical attached in GetAttachedAsPhysical() ) {
                if( attached.Externalize(physical) ) {
                    return true;
                }
            }

            return false;
        }
        public override CheckResult CanAttach( GameObject obj )
        {
            if( Contains(obj) ) { return CheckResult.VALID; }
            Physical physical = obj as Physical;
            if( physical == null ) { return CheckResult.INVALID; }

            CheckResult result = base.CanAttach(obj);
            if( result != CheckResult.VALID ) { return result; }

            if( capacity >= 0  && physical.GetVolume() > remainingCapacity ) { return CheckResult.RESTRICTED; }

            return CheckResult.VALID;
        }
        public override bool Attach( GameObject obj )
        {
            if( !isExternal ) { return base.Attach(obj); }

            if( obj == null ) { return false; }
            if( attachedObjects.Contains(obj) ) { return true; }

            if( CanAttach(obj) != CheckResult.VALID ) { return false; }
            if( parentObject.container != null && parentObject.container.CanAttach(obj) != CheckResult.VALID ) { return false; } // XXX: This might be incorrect due to 
                                                                                                                                 // attaching objects to an external attachment point changes the volume and weight of the parentObject
            if( obj.container != null && !obj.container.Remove(obj) ) { return false; }
            attachedObjects.Add(obj);
            obj.SetContainer(parentObject.container); // XXX: obj has no reference to this attachmentpoint (it doesn't know what it is attached to, only what it is contained by)

            return true;
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
