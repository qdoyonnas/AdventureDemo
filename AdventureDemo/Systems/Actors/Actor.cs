using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    public abstract class Actor
    {
        #region Fields

        protected GameObject controlledObject;
		protected readonly List<Verb> verbs;
        protected readonly List<Verb> inherentVerbs; // Verbs that belong to the actor and are always available regardless of controlled object

        #endregion

        #region Events

        public delegate void ObservedActionDelegate( Dictionary<string, object> data );
        public event ObservedActionDelegate ObservedActionTaken;

        public virtual void OnObservedActionTaken( Dictionary<string, object> data )
        {
            ObservedActionTaken?.Invoke(data);
        }

        #endregion

		public Actor()
        {
            verbs = new List<Verb>();
            inherentVerbs = new List<Verb>();
            inherentVerbs.Add( DataManager.instance.LoadObject<Verb>("wait", typeof(VerbData)) );
            inherentVerbs.Add( DataManager.instance.LoadObject<Verb>("emote", typeof(VerbData)) );

            TimelineManager.instance.OnActionEvent += OnObservedActionTaken;
        }

        #region Control

        public virtual bool Control( GameObject obj )
        {
            if( controlledObject != null ) {
                bool success = controlledObject.SetActor(null);
                
                if( !success ) { return false; }
            }

			controlledObject = obj;

            if( controlledObject != null ) {
                bool success = controlledObject.SetActor(this);
                if( !success ) {
                    controlledObject = null;
                    return false;
                }
            }

            CollectVerbs();
            return true;
        }
		public virtual GameObject GetControlled()
        {
			return controlledObject;
        }

        #endregion

        #region Observation

        /// <summary>
        /// Returns a bool indicating whether obj can be perceived at all. For example, if it should
        /// appear in an Overview page.
        /// </summary>
        /// <param name="obj">Object to be observed.</param>
        /// <returns></returns>
        public virtual bool CanObserve( GameObject obj )
        {
            // TODO: Implement senses

            GameObject thisContainer = controlledObject.attachPoint.GetParent();
            if( obj == thisContainer || obj.attachPoint == controlledObject.attachPoint ) {
                return true;
            } else if( obj.attachPoint != null ) {
                GameObject objContainer = obj.attachPoint.GetParent();
                if( objContainer != null ) {
                    return CanObserve(objContainer);
                }
            }

            return false;
        }

        #endregion

        #region Verbs

        public virtual bool HasVerb( string type)
        {
            foreach( Verb verb in verbs ) {
                if ( verb.type == type ) { return true; }
            }

            return false;
        }
        public virtual List<Verb> GetVerbs()
        {
            return verbs;
        }
        public virtual List<Verb> GetVerbs( string type )
        {
            List<Verb> filteredVerbs = new List<Verb>();

            foreach( Verb verb in verbs ) {
                if( verb.GetType().Name == type ) {
                    filteredVerbs.Add(verb);
                }
            }

            return filteredVerbs;
        }
        public virtual void AddVerb( Verb verb )
        {
            verbs.Add(verb);
        }

        public virtual void CollectVerbs()
        {
            verbs.Clear();

            if( controlledObject == null ) { return; }

            foreach( Verb verb in inherentVerbs ) {
                if( verb == null ) { continue; }

                verb.self = controlledObject;
                verbs.Add(verb);
            }

            controlledObject.CollectVerbs(this, PossessionType.EMBODIMENT);
        }

        #endregion
	}

	public enum PossessionType {
        EMBODIMENT,
        INTERACTION,
        CONTENT
    }
}
