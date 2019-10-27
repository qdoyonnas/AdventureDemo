using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PhysicalAttachmentPoint : AttachmentPoint
    {
        public PhysicalAttachmentPoint( GameObject parent, params AttachmentType[] types )
            : base( parent, types )
        {

        }
    }
}
