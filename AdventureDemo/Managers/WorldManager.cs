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
        // XXX: Incorrect use of Random
        //      every use of random should be done from a common 'world seed'
        //      offset by the unique 'position' of the object in that world
        public Random random;

        public PlayerActor player;
        private List<Container> rootObjects;

        private Dictionary<string, object> objectReferenceIds = new Dictionary<string, object>();

        public int worldSeed = -1;

        public WorldData data;
        public ScenarioData scenarioData;

        public WorldManager( WorldData data, int seed )
        {
            this.data = data;

            worldSeed = seed;
            random = worldSeed == -1 ? new Random() : new Random(worldSeed);

            rootObjects = new List<Container>();
            foreach( string key in data.roots.Keys ) {
                Container root = DataManager.instance.LoadObject<Container>(key, typeof(ContainerData));
                if( root == null ) { continue; }

                AddRoot(root);

                foreach( DataReference dataReference in data.roots[key] ) {
                    GameObject obj = dataReference.LoadData<GameObject>(typeof(ObjectData));
                    root.GetContents().Attach(obj);
                }
            }
        }

        public void LoadScenario( ScenarioData data )
        {
            scenarioData = data;

            foreach( LocatorDataReference objectData in data.objects ) {
                objectData.LoadData<GameObject>(typeof(ObjectData));
            }

            player = new PlayerActor();
            InputManager.instance.inputReceived += player.ParseInput;
            
            GameObject playerObj = data.playerInfo.LoadData<GameObject>(typeof(ObjectData));
            player.Control(playerObj);
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
        
        public void SaveObjectReference( string id, object obj )
        {
            objectReferenceIds[id] = obj;
        }
        public object GetObjectReference( string id )
        {
            if( !objectReferenceIds.ContainsKey(id) ) { return null; }

            return objectReferenceIds[id];
        }

        // XXX: Searching is recursive and therefore prone to heavy slowdown
        //      The following solutions need to be implemented:
        //      (1) Frame limiting with foundObjects being updated throughout the process (and correctly used by the searching objects)
        //      (2) Multi-threading, the searching should happen on a separate thread entirely
        public GameObject[] FindObjects( Dictionary<string, string> properties )
        {
            return FindObjects(GetRoot(0), 3, properties);
        }
        public GameObject[] FindObjects( int searchDepth, Dictionary<string, string> properties )
        {
            return FindObjects(GetRoot(0), searchDepth, properties);
        }
        public GameObject[] FindObjects( GameObject relativeObject, Dictionary<string, string> properties )
        {
            return FindObjects(relativeObject, 3, properties);
        }
        public GameObject[] FindObjects( GameObject relativeObject, int searchDepth, Dictionary<string, string> properties )
        {
            List<GameObject> foundObjects = new List<GameObject>();
            List<GameObject> objectsToSearch = new List<GameObject>();

            if( relativeObject.MatchesSearch(new Dictionary<string,string>(properties)) ) {
                foundObjects.Add(relativeObject);
            }
            if( searchDepth > 0 ) {
                foreach( GameObject child in relativeObject.GetChildObjects() ) {
                    foundObjects.AddRange( FindObjects( child, searchDepth - 1, properties ) );
                }
            }
            
            return foundObjects.ToArray();
        }

        public void Unload()
        {
            InputManager.instance.inputReceived -= player.ParseInput;
            player = null;

            // For now delete everything non root
            foreach( GameObject root in rootObjects ) {
                Container containerRoot = root as Container;
                if( containerRoot != null ) {
                    ContainerAttachmentPoint contents = containerRoot.GetContents();
                    foreach (GameObject obj in contents.GetAttached()) {
                        contents.Remove(obj);
                    }
                }
            }
        }
    }
}
