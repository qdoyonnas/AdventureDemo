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

        public SpawnList()
        {
            entries = new SpawnEntry[0];
        }
        public SpawnList( SpawnList list )
        {
            entries = new SpawnEntry[list.entries.Length];
            for( int i = 0; i < entries.Length; i++ ) {
                entries[i] = new SpawnEntry(list.entries[i]);
            }
        }

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

        public SpawnEntry() { }
        public SpawnEntry( SpawnEntry entry )
        {
            id = new DataReference(entry.id.value);
            spawnChance = entry.spawnChance;
            spawnQuantity = entry.spawnQuantity;
            independantSpawn = entry.independantSpawn;
        }

        public void Spawn( Container container, double weight )
        {
            GameObject gameObject = id.LoadData<GameObject>(typeof(ObjectData));
            container.GetContents().Attach(gameObject);
        }
    }
}
