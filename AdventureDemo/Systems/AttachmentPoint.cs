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

        protected List<IPhysical> attachedObjects;

        protected bool divisable = true;
        protected bool loose = true;

        public AttachmentPoint( params AttachmentType[] types )
        {
            attachmentTypes = types;

            attachedObjects = new List<IPhysical>(1);
        }

        public virtual bool CanAttach( IPhysical obj )
        {
            if( !divisable && attachedObjects.Count > 0 ) { return false; }

            return true;
        }
        public virtual bool Attach( IPhysical obj )
        {
            if( !CanAttach(obj) ) { return false; }

            attachedObjects.Add(obj);

            return true;
        }
    }

    public enum AttachmentType {
        SURFACE,
        HANG,
        WRAP
    }
}
