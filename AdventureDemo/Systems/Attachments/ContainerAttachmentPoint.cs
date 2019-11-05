using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class ContainerAttachmentPoint : PhysicalAttachmentPoint
    {
        protected List<Connection> connections;

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
            connections = new List<Connection>();
        }

        public Connection[] GetConnections()
        {
            return connections.ToArray();
        }
        public int GetConnectionsCount()
        {
            return connections.Count();
        }

        public void AddConnection( Connection connection )
        {
            connections.Add( connection );
        }
        public void AddConnection( Dictionary<string, object> data, bool isTwoWay = true )
        {
            if( !data.ContainsKey("parent") ) {
                data.Add("parent", this);
            } else {
                data["parent"] = this;
            }
            connections.Add( new Connection(data) );

            if( isTwoWay && data.ContainsKey("second") ) {
                ContainerAttachmentPoint second = data["second"] as ContainerAttachmentPoint;
                data["parent"] = second;
                data["second"] = this;

                second.AddConnection( new Connection(data) );
            }
        }

        public void AddConnection( ContainerAttachmentPoint second, double throughput = 0, bool isTwoWay = true )
        {
            connections.Add( new Connection(this, second, throughput) );

            if( isTwoWay && second != null ) {
                second.AddConnection( new Connection(second, this, throughput) );
            }
        }

        public void RemoveConnection( Connection connection )
        {
            connections.Remove( connection );
        }
    }
}
