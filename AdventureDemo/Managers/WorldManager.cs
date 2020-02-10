﻿using System;
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

        public int randomSeed = -1;

        public WorldManager( ScenarioData data )
        {
            if( randomSeed != -1 ) {
                GameManager.instance.random = new Random(randomSeed);
            } else {
                GameManager.instance.random = new Random();
            }

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

            player = new PlayerActor();
            InputManager.instance.inputReceived += player.ParseInput;
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
        
        public GameObject[] FindObjects( GameObject relativeObject, params string[] inputs )
        {
            return FindObjects(relativeObject, 3, inputs);
        }
        public GameObject[] FindObjects( GameObject relativeObject, int searchDepth, params string[] inputs )
        {
            List<GameObject> foundObjects = new List<GameObject>();
            List<GameObject> objectsToSearch = new List<GameObject>();

            if( relativeObject.container != null ) {
                objectsToSearch.Add(relativeObject.container.GetParent());
            }

            for( int i = inputs.Length-1; i >= 0; i-- ) {
                string lowerInput = inputs[i].ToLower();
                if( lowerInput == "self"
                    || lowerInput == "me"
                    || lowerInput == relativeObject.GetData("name").text.ToLower() ) {
                    foundObjects.Add(relativeObject);
                }

                for( int o = 0; o < objectsToSearch.Count; o++ ) {
                    SearchObject(lowerInput, objectsToSearch[o], foundObjects, objectsToSearch, true);
                }
            }
            
            return foundObjects.ToArray();
        }
        private void SearchObject( string input, GameObject obj, List<GameObject> foundObjects, List<GameObject> objectsToSearch, bool searchChildren )
        {
            if( input == obj.GetData("name").text.ToLower() ) {
                foundObjects.Add(obj);
            }

            if( searchChildren ) {
                foreach( GameObject child in obj.GetChildObjects() ) {
                    if( !objectsToSearch.Contains(child) ) {
                        objectsToSearch.Add(child);
                    }
                }
            }
        }
    }
}
