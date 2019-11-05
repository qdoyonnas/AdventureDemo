using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class BodyAttachmentPoint : PhysicalAttachmentPoint
    {
        public BodyAttachmentPoint( GameObject parent )
            : base( parent, 1, AttachmentType.BODY )
        {
        }
    }
}
