using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class LocatorDataReference
    {
        public DataReference data = null;
        public SearchParams location = null;

        public LocatorDataReference() {}
        public LocatorDataReference( string data, SearchParams location )
        {
            this.data = new DataReference(data);
            this.location = location;
        }
        public LocatorDataReference( LocatorDataReference data )
        {
            this.data = data.data == null ? null : new DataReference(data.data.value);
            this.location = data.location == null ? null : new SearchParams(data.location);
        }

        public T LoadData<T>( Type dataType, Dictionary<string, object> context = null )
            where T : GameObject
        {
            T obj = data.LoadData<T>(dataType, context);

            Container container = location.FindRandom() as Container;
            if( container == null ) {
                Console.WriteLine($"ERROR: LocatorDataReference could find location matching params: {location.ToString()}");
            } else {
                container.GetContents().Attach(obj);
            }

            return obj;
        }
    }
}
