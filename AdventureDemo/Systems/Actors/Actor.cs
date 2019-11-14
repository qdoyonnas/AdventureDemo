using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    // TODO: Split Actor and PlayerActor code
    abstract class Actor
    {
		protected GameObject controlledObject;

		protected readonly List<Verb> verbs;

		public Actor()
        {
			verbs = new List<Verb>();
        }

		public virtual void Control( GameObject obj )
        {
            verbs.Clear();

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

            controlledObject.CollectVerbs(this, PossessionType.EMBODIMENT);
        }
		public virtual GameObject GetControlled()
        {
			return controlledObject;
        }

        public List<GameObject> GetSubjectObjects()
        {
            // TODO: Senses here?

            List<GameObject> subjects = new List<GameObject>();

            subjects.Add(controlledObject);

            return subjects;
        }

		/// <summary>
        /// Returns a bool indicating whether obj can be perceived at all. For example, if it should
        /// appear in an Overview page.
        /// </summary>
        /// <param name="obj">Object to be observed.</param>
        /// <returns></returns>
		public abstract bool CanObserve( GameObject obj );

		public abstract GameObjectData Observe( GameObject obj );
		public abstract GameObjectData Observe( GameObject obj, string dataKey );

        public virtual bool HasVerb( Type type )
        {
            foreach( Verb verb in verbs ) {
                if( verb.GetType() == type ) { return true; }
            }

            return false;
        }
        public virtual List<Verb> GetVerbs()
        {
            return verbs;
        }
        public virtual List<Verb> GetVerbs( Type type )
        {
            List<Verb> filteredVerbs = new List<Verb>();

            foreach( Verb verb in verbs ) {
                if( verb.GetType() == type ) {
                    filteredVerbs.Add(verb);
                }
            }

            return filteredVerbs;
        }
        public virtual void AddVerb( Verb verb )
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
