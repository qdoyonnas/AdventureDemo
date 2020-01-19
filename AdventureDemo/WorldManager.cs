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

        public WorldManager( ScenarioData data )
        {
            rootObjects = new List<Container>();
            foreach( KeyValuePair<string,string> root in data.roots ) {
                Container rootObject = (Container)DataManager.instance.LoadObject(root.Key);
                AddRoot(rootObject);

                string[] entries = root.Value.Split(' ');
                foreach( string entry in entries ) {
                    string[] values = entry.Split(':');
                    switch( values[0] ) {
                        case "o":
                            rootObject.GetContents().Attach( DataManager.instance.LoadObject(values[1]) );
                            break;
                        case "s":
                            // Load Spawn list
                            break;
                    }
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
