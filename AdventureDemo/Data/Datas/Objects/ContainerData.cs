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
        public DataReference[] spawnLists;

        public override Dictionary<string, object> GenerateData()
        {
            Dictionary<string, object> data = base.GenerateData();

            double v = innerVolume.GetValue(data);
            if( v < 0 ) { v = double.PositiveInfinity; }
            data["innerVolume"] = v;

            if( spawnLists != null && spawnLists.Length > 0 ) {
                SpawnList[] spawns = new SpawnList[spawnLists.Length];
                for( int i = 0; i < spawnLists.Length; i++ ) {
                    spawns[i] = spawnLists[i].GetData<SpawnData>().Create();
                }
                data["spawnLists"] = spawns;
            }

            return data;
        }
        public override GameObject Create()
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
