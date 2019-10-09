using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class Actor
    {
		GameObject controlledObject;

		readonly List<Verb> verbs;

		public Actor()
        {
			verbs = new List<Verb>();
        }

		public void Control( GameObject obj )
        {
			controlledObject = obj;
        }
		public GameObject GetControlled()
        {
			return controlledObject;
        }

		/// <summary>
        /// Returns a bool indicating whether obj can be perceived at all. For example, if it should
        /// appear in an Overview page.
        /// </summary>
        /// <param name="obj">Object to be observed.</param>
        /// <returns></returns>
		public bool CanObserve( GameObject obj )
        {
            // TODO: Implement senses

            return true;
        }

		public GameObjectData Observe( GameObject obj )
        {
			return Observe(obj, "name");
        }
		public GameObjectData Observe( GameObject obj, string dataKey )
        {
			// TODO: Implement senses

			GameObjectData data = obj.GetData(dataKey);

			if( dataKey == "name" ) {
				foreach( Verb verb in verbs) {
					if( verb.Check(obj) ) {
						WaywardEngine.ContextMenuHelper.AddContextMenuItem(data.span, 
                    }
				}
            }

			return data;
        }
    }
}
