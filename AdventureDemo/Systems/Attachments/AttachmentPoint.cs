using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class AttachmentPoint
    {
        protected AttachmentType[] attachmentTypes;

        protected GameObject parentObject;
        protected List<GameObject> attachedObjects;

        protected bool divisable = true;
        protected bool loose = true;

        public AttachmentPoint( GameObject parent, params AttachmentType[] types )
        {
            parentObject = parent;
            attachmentTypes = types;

            attachedObjects = new List<GameObject>(1);
        }

        public virtual GameObject GetParent()
        {
            return parentObject;
        }

        public virtual bool CanAttach( GameObject obj )
        {
            if( !divisable && attachedObjects.Count > 0 ) { return false; }

            return true;
        }
        public virtual bool Attach( GameObject obj )
        {
            if( !CanAttach(obj) ) { return false; }

            attachedObjects.Add(obj);

            return true;
        }
        public virtual bool Remove( GameObject obj )
        {
            attachedObjects.Remove(obj);
            return true;
        }

        public GameObject[] GetAttached()
        {
            return attachedObjects.ToArray();
        }
    }

    public enum AttachmentType {
        SURFACE,
        HANG,
        WRAP
    }
}
