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

        public ContainerData() {}
        public ContainerData( ContainerData data )
            : base(data)
        {
            innerVolume = new DynamicDouble(data.innerVolume);
            spawnLists = new DataReference[data.spawnLists.Length];
            for( int i = 0; i < spawnLists.Length; i++ ) {
                spawnLists[i] = new DataReference(data.spawnLists[i].value);
            }
        }

        public override Dictionary<string, object> GenerateData()
        {
            Dictionary<string, object> data = base.GenerateData();

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
        protected override object CreateInstance()
        {
            Container container = null;

            try {
                container = new Container(GenerateData());
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate Container from ContainerData: {e}");
            }

            return container;
        }
    }
}
