using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    // XXX: Searching is recursive and therefore prone to heavy slowdown
    //      The following solutions need to be implemented:
    //      (1) Frame limiting with foundObjects being updated throughout the process (and correctly used by the searching objects)
    //      (2) Multi-threading, the searching should happen on a separate thread entirely
    class SearchParams
    {
        public string referenceId = null;
        public Dictionary<string, string> properties = new Dictionary<string, string>();
        public SearchParams[] subSearches = new SearchParams[0];

        public SearchParams() { }
        public SearchParams( SearchParams search )
        {
            referenceId = search.referenceId;
            properties = search.properties == null ? null : new Dictionary<string, string>(search.properties);

            subSearches = new SearchParams[search.subSearches.Length];
            for( int i = 0; i < subSearches.Length; i++ ) {
                subSearches[i] = new SearchParams(search.subSearches[i]);
            }
        }

        public GameObject[] FindAll()
        {
            GameObject[] foundObjects = null;
            if( referenceId == null ) {
                foundObjects = GameManager.instance.world.FindObjects(properties);
            } else {
                GameObject referenceObject = GameManager.instance.world.GetObjectReference(referenceId) as GameObject;
                foundObjects = referenceObject == null ? null : new GameObject[] { referenceObject };
            }

            if( subSearches.Length > 0 ) { 
                return PerformSubSearches(foundObjects);
            } else {
                return foundObjects;
            }
        }
        public GameObject[] FindAllIn( params GameObject[] objectsToSearch )
        {
            if( referenceId != null ) { return null; }

            List<GameObject> foundObjects = new List<GameObject>();
            if( objectsToSearch.Length ==  1 ) {
                GameObject[] worldFoundObjects = GameManager.instance.world.FindObjects(objectsToSearch[0], new Dictionary<string,string>(properties));
                if( subSearches.Length > 0 ) {
                    foundObjects.AddRange( PerformSubSearches(worldFoundObjects) );
                } else {
                    foundObjects.AddRange(worldFoundObjects);
                }
            } else {
                foreach( GameObject obj in objectsToSearch ) {
                    foundObjects.AddRange( FindAllIn(obj) );
                }
            }

            return foundObjects.ToArray();
        }
        public GameObject FindFirst()
        {
            return FindAll()[0]; // XXX: Inefficient
        }
        public GameObject FindRandom()
        {
            GameObject[] foundObjects = FindAll();
            if( foundObjects.Length == 0 ) { return null; }

            int i = GameManager.instance.world.random.Next(0, foundObjects.Length);
            return foundObjects[i];
        }

        protected GameObject[] PerformSubSearches( GameObject[] objectsToSearch )
        {
            List<GameObject> foundObjects = new List<GameObject>();
            
            foreach( SearchParams search in subSearches ) {
                foundObjects.AddRange(search.FindAllIn(objectsToSearch));
            }

            return foundObjects.ToArray();
        }

        public override string ToString()
        {
            return ToString(new StringBuilder(), 4);
        }
        public string ToString(StringBuilder sb, int indent)
        {
            sb.Append(Environment.NewLine);

            sb.Append('\t', indent);
            sb.AppendLine("referenceId: " + (referenceId == null ? "null" : referenceId));

            sb.Append('\t', indent);
            sb.Append("properties: {");
            if( properties.Count == 0 ) {
                sb.Append(Environment.NewLine);
                sb.Append('\t', indent + 1);
                sb.AppendLine("empty");
            } else {
                foreach( KeyValuePair<string, string> property in properties ) {
                    sb.Append(Environment.NewLine);
                    sb.Append('\t', indent + 1);
                    sb.AppendLine($"{property.Key} : {property.Value}");
                }
            }
            sb.Append('\t', indent);
            sb.AppendLine("}");

            sb.Append('\t', indent);
            sb.Append("subSearches: {");
            if( subSearches.Length == 0 ) {
                sb.Append(Environment.NewLine);
                sb.Append('\t', indent + 1);
                sb.AppendLine("empty");
            } else {
                foreach( SearchParams search in subSearches ) {
                    search.ToString(sb, indent + 1);
                }
            }
            sb.Append('\t', indent);
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
