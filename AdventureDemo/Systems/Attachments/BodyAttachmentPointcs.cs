using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    class BodyAttachmentPoint : PhysicalAttachmentPoint
    {
        public BodyAttachmentPoint( GameObject parent )
            : base( parent, -1, 1, AttachmentType.BODY )
        {
            _isExternal = true;
        }
    }
}
