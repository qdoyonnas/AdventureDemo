using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class SpawnList : BasicData
    {
        public SpawnEntry[] entries = new SpawnEntry[0];

        public SpawnList() {}
        public SpawnList( SpawnList list )
        {
            entries = new SpawnEntry[list.entries.Length];
            for( int i = 0; i < entries.Length; i++ ) {
                entries[i] = new SpawnEntry(list.entries[i]);
            }
        }

        protected override object CreateInstance()
        {
            return this;
        }

        public void Spawn( Container container, double weight )
        {
            foreach( SpawnEntry entry in entries ) {
                GameObject[] objects = entry.Spawn(weight);
                foreach( GameObject obj in objects ) {
                    container.GetContents().Attach(obj);
                }
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

        public GameObject[] Spawn( double weight )
        {
            List<GameObject> objects = new List<GameObject>();

            for( int i = 0; i < spawnQuantity; i++ ) {
                double roll = GameManager.instance.world.random.NextDouble() * weight;
                if( roll > 1 - spawnChance ) {
                    objects.Add( id.LoadData<GameObject>(typeof(ObjectData)) );
                } else if( !independantSpawn ) {
                    break;
                }
            }

            return objects.ToArray();
        }
    }
}
