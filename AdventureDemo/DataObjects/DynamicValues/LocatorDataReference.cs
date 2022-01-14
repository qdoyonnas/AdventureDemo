using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    public class LocatorDataReference
    {
        public DataReference data = null;
        public DataReference actor = null;
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
                WaywardEngine.WaywardManager.instance.Log($@"<red>ERROR: LocatorDataReference could not find location matching params: {location.ToString()}</red>");
            } else {
                container.GetContents().Attach(obj);
            }

            if( actor != null ) {
                Actor actorObj = actor.LoadData<Actor>(typeof(ActorData), context);
                actorObj.Control(obj);
            }

            return obj;
        }
    }
}
