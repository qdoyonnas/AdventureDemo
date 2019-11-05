using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class Connection : GameObject
    {
        ContainerAttachmentPoint _secondContainer;
        public ContainerAttachmentPoint secondContainer {
            get {
                return _secondContainer;
            }
            set {
                if( value != null ) {
                    _secondContainer = value;
                } else {
                    ContainerAttachmentPoint parentContainer = container.GetParent().container as ContainerAttachmentPoint;
                    _secondContainer = parentContainer;
                }
            }
        }

        double _throughput;
        public double throughput {
             get {
                return _throughput;
            }
        }

        public Connection( Dictionary<string, object> data )
            : base(data)
        {
            if( !data.ContainsKey("parent") ) { throw new System.ArgumentException("Connection requires a parent container"); }
            ContainerAttachmentPoint parent = data["parent"] as ContainerAttachmentPoint;

            ContainerAttachmentPoint second = data.ContainsKey("second") ? data["second"] as ContainerAttachmentPoint : null;
            double volume = data.ContainsKey("throughput") ? (double)data["throughput"] : 0;

            Construct( parent, second, volume );

            if( !data.ContainsKey("description") ) {
                description = "an opening";
            }
        }
        public Connection( ContainerAttachmentPoint parent )
            : base( "opening", null )
        {
            Construct( parent, null, 0 );
            description = "an opening";
        }
        public Connection( ContainerAttachmentPoint parent, ContainerAttachmentPoint second )
            : base( "opening", null )
        {
            Construct( parent, second, 0 );
            description = "an opening";
        }
        public Connection( ContainerAttachmentPoint parent, ContainerAttachmentPoint second, double throughput )
            : base( "opening", null )
        {
            Construct( parent, second, throughput );
            description = "an opening";
        }
        void Construct( ContainerAttachmentPoint parent, ContainerAttachmentPoint second, double throughput )
        {
            this._throughput = throughput;

            this.secondContainer = second;
        }
    }
}
