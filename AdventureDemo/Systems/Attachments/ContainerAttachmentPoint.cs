using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class ContainerAttachmentPoint : PhysicalAttachmentPoint
    {
        protected List<AttachmentPoint> connections;

        public ContainerAttachmentPoint( Dictionary<string, object> data )
            : base( data )
        {
            Construct();
        }
        public ContainerAttachmentPoint( GameObject parent, double capacity )
            : base( parent, capacity, AttachmentType.ALL )
        {
            Construct();
        }
        private void Construct()
        {
            _maxQuantity = -1;
            connections = new List<AttachmentPoint>();
        }
    }
}
