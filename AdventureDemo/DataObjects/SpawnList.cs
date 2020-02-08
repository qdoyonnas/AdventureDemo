using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class SpawnList : BasicData
    {
        public SpawnEntry[] entries;

        public override object Create()
        {
            return this;
        }

        public void Spawn( Container container, double weight )
        {
            foreach( SpawnEntry entry in entries ) {
                entry.Spawn(container, weight);
            }
        }
    }

    class SpawnEntry
    {
        public DataReference id;
        public double spawnChance = 0;
        public int spawnQuantity = 1;
        public bool independantSpawn = true;

        public void Spawn( Container container, double weight )
        {
            GameObject gameObject = id.LoadData<GameObject>(typeof(ObjectData));
            container.GetContents().Attach(gameObject);
        }
    }
}
