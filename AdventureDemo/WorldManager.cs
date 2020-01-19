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
        private List<GameObject> rootObjects;

        private Dictionary<string, Material> materials;

        public WorldManager( ScenarioData data )
        {
            rootObjects = new List<GameObject>();

            materials = new Dictionary<string, Material>();
        }

        public void AddRoot( GameObject obj )
        {
            rootObjects.Add(obj);
        }
        public void RemoveRoot( GameObject obj )
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

        public Material GetMaterial( string id )
        {
            return materials[id];
        }
    }
}
