using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class Container : GameObject, IContainer
    {
        List<GameObject> contents;

        public Container( string name ) : base(name)
        {
            contents = new List<GameObject>();
        }

        public GameObject GetContent( int i )
        {
            return contents[i];
        }
        public int ContentCount()
        {
            return contents.Count;
        }
        public bool DoesContain( GameObject obj )
        {
            return contents.Contains(obj);
        }
        public void AddContent( GameObject obj )
        {
            contents.Add(obj);
        }
        public void RemoveContent( GameObject obj )
        {
            contents.Remove(obj);
        }
    }
}
