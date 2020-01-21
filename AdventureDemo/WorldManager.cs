using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaywardEngine;

namespace AdventureDemo
{
    class WorldManager
    {
        public PlayerActor player;
        private List<Container> rootObjects;

        public Random random;

        public WorldManager( ScenarioData data )
        {
            random = new Random();

            rootObjects = new List<Container>();
            foreach( string key in data.roots.Keys ) {
                Container root = DataManager.instance.GetObjectData(key).Create() as Container;
                if( root == null ) { continue; }

                AddRoot(root);

                foreach( DataReference dataReference in data.roots[key] ) {
                    GameObject obj = dataReference.GetData<ObjectData>().Create();
                }
            }

            player = new PlayerActor();
            WaywardWill will = new WaywardWill();
            rootObjects[0].GetContents().Attach(will);
            player.Control(will);
        }

        public void AddRoot( Container obj )
        {
            rootObjects.Add(obj);
        }
        public void RemoveRoot( Container obj )
        {
            rootObjects.Remove(obj);
        }

        public int RootCount()
        {
            return rootObjects.Count;
        }
        public GameObject GetRoot( int i )
        {
            return rootObjects[i];
        }
    }
}
