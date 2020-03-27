using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class ContainerData : PhysicalData
    {
        public DynamicDouble innerVolume = new DynamicDouble("0");
        public DataReference[] spawnLists = new DataReference[0];

        public ConnectionDataReference[] connections = new ConnectionDataReference[0];

        public ContainerData() {}
        public ContainerData( ContainerData data )
            : base(data)
        {
            innerVolume = new DynamicDouble(data.innerVolume);

            connections = new ConnectionDataReference[data.connections.Length];
            for( int i = 0; i < connections.Length; i++ ) {
                connections[i] = new ConnectionDataReference(data.connections[i]);
            }

            spawnLists = new DataReference[data.spawnLists.Length];
            for( int i = 0; i < spawnLists.Length; i++ ) {
                spawnLists[i] = new DataReference(data.spawnLists[i].value);
            }
        }

        public override Dictionary<string, object> GenerateData(Dictionary<string, object> context = null)
        {
            Dictionary<string, object> data = base.GenerateData(context);

            double v = innerVolume.GetValue(data);
            if( v < 0 ) { v = double.PositiveInfinity; }
            data["innerVolume"] = v;

            if( spawnLists != null && spawnLists.Length > 0 ) {
                SpawnList[] spawns = new SpawnList[spawnLists.Length];
                for( int i = 0; i < spawnLists.Length; i++ ) {
                    spawns[i] = spawnLists[i].LoadData<SpawnList>(typeof(SpawnList));
                }
                data["spawnLists"] = spawns;
            }

            return data;
        }
        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            Container container = null;

            try {
                container = new Container(GenerateData(context));

                foreach( ConnectionDataReference reference in connections ) {
                    Container linked = GameManager.instance.world.GetObjectReference(reference.linkReference) as Container;
                    if( linked != null ) {
                        Dictionary<string, object> connectionData = new Dictionary<string, object>();
                        connectionData.Add("name", reference.name);
                        connectionData.Add("description", reference.description);
                        connectionData.Add("second", linked.GetContents());
                        connectionData.Add("throughput", reference.throughput.GetValue(connectionData));

                        container.AddConnection(connectionData, reference.isTwoWay);
                    }
                }
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate Container from ContainerData: {e}");
            }

            return container;
        }
    }
}
