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
            if( controlledObject != null ) {
                bool success = controlledObject.SetActor(null, PossessionType.EMBODIMENT);
                if( !success ) { return; }
            }
			controlledObject = obj;
            if( controlledObject != null ) {
                bool success = controlledObject.SetActor(this, PossessionType.EMBODIMENT);
                if( !success ) {
                    controlledObject = null;
                    return;
                }
            }
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

            GameObject thisContainer = controlledObject.container as GameObject;
            if( obj == thisContainer || obj.container == controlledObject.container ) {
                return true;
            } else {
                GameObject objContainer = obj.container as GameObject;
                if( objContainer != null ) {
                    return CanObserve(objContainer);
                }
            }

            return false;
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
						verb.Display(data.span, obj);
                    }
				}
            }

			return data;
        }

        public void AddVerb( Verb verb )
        {
            verbs.Add(verb);
        }
    }

    public enum PossessionType {
        EMBODIMENT,
        INTERACTION,
        CONTENT
    }
}
