using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class AttachmentPoint
    {
        public string name = "slot";

        protected AttachmentType[] attachmentTypes; // Allowed AttachmentTypes

        protected GameObject parentObject;
        protected List<GameObject> attachedObjects;

        protected int _maxQuantity = 1; // -1 for infinite
        public int maxQuantity {
            get {
                return _maxQuantity;
            }
        }

        public AttachmentPoint( Dictionary<string, object> data )
        {
            if( !data.ContainsKey("parent") ) { throw new System.ArgumentException("AttachmentPoint requires a parent GameObject"); }

            name = data.ContainsKey("name") ? (string)data["name"] : "slot";
            _maxQuantity = data.ContainsKey("quantity") ? (int)data["quantity"] : 1;
            AttachmentType[] types = data.ContainsKey("types") ? data["types"] as AttachmentType[] : new AttachmentType[] { AttachmentType.ALL };

            Construct( data["parent"] as GameObject, types );
        }
        public AttachmentPoint( GameObject parent, int quantity, params AttachmentType[] types )
        {
            Construct(parent, types);
            _maxQuantity = quantity;
        }
        public AttachmentPoint( GameObject parent, params AttachmentType[] types )
        {
            Construct( parent, types );
        }
        private void Construct( GameObject parent, AttachmentType[] types)
        {
            parentObject = parent;
            attachmentTypes = types;

            attachedObjects = new List<GameObject>(1);
        }

        public virtual GameObject GetParent()
        {
            return parentObject;
        }

        public virtual CheckResult CanAttach( GameObject obj )
        {
            if( !CompareAttachmentTypes(obj) ) { return CheckResult.INVALID; }

            if( maxQuantity >= 0 && attachedObjects.Count >= maxQuantity ) { return CheckResult.RESTRICTED; }

            return CheckResult.VALID;
        }
        public virtual bool CompareAttachmentTypes( GameObject obj )
        {
            if( attachmentTypes.Contains(AttachmentType.ALL)
                || obj.attachmentTypes.Contains(AttachmentType.ALL) ) { return true; }
            AttachmentType[] objectAttachmentTypes = obj.attachmentTypes.ToArray();
            foreach( AttachmentType type in attachmentTypes ) {
                foreach( AttachmentType objectType in objectAttachmentTypes ) {
                    if( type == objectType ) { return true; }
                }
            }

            return false;
        }

        public virtual bool Attach( GameObject obj )
        {
            if( obj == null ) { return false; }
            if( attachedObjects.Contains(obj) ) { return true; }

            if( CanAttach(obj) != CheckResult.VALID ) { return false; }

            if( obj.container != null && !obj.container.Remove(obj) ) { return false; }
            attachedObjects.Add(obj);
            obj.SetContainer(this);

            return true;
        }
        public virtual bool Remove( GameObject obj )
        {
            attachedObjects.Remove(obj);
            return true;
        }

        public virtual GameObject[] GetAttached()
        {
            return attachedObjects.ToArray();
        }
        public virtual GameObject GetAttached( int i )
        {
            return attachedObjects[i];
        }
        public virtual bool Contains( GameObject obj )
        {
            return attachedObjects.Contains(obj);
        }
        public int GetAttachedCount()
        {
            return attachedObjects.Count;
        }
    }

    public enum AttachmentType {
        ALL, // AttachmentPoints use this to accept anything. Of GameObjects, only the Wayward Will should have this
        HANG,
        WRAP,
        BODY // XXX: Might be better as derived class
    }
}
