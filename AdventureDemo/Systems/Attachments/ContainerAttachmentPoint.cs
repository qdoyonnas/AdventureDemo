using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    public class ContainerAttachmentPoint : PhysicalAttachmentPoint
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
            Connection connection = new Connection(data);
            connections.Add(connection);

            if( isTwoWay && data.ContainsKey("container") ) {
                Container container = data["container"] as Container;
                data["parent"] = container.GetContents();
                data["connection"] = connection;

                Connection linkedConnection = new Connection(data);
                container.AddConnection(linkedConnection);
                connection.connection = linkedConnection;
            }
        }

        public void RemoveConnection( Connection connection )
        {
            connections.Remove( connection );
        }
    }
}
