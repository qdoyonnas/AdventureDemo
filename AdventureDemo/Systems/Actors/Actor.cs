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
    // TODO: Split Actor and PlayerActor code
    abstract class Actor
    {
        #region Fields

        protected GameObject controlledObject;
        protected readonly Dictionary<string[], InputManager.InputDelegate> commands;
		protected readonly List<Verb> verbs;
        protected readonly List<Verb> inherentVerbs; // Verbs that belong to the actor and are always available regardless of controlled object
        protected readonly List<WaywardEngine.Page> relatedPages;

        #endregion

        #region Events

        public delegate void ObservedActionDelegate( Dictionary<string, object> data );
        public event ObservedActionDelegate ObservedActionTaken;

        public virtual void OnObservedActionTaken( Dictionary<string, object> data )
        {
            ObservedActionTaken?.Invoke(data);

            // Do others stuff
        }

        #endregion

		public Actor()
        {
            verbs = new List<Verb>();
            inherentVerbs = new List<Verb>();
            inherentVerbs.Add( new WaitVerb() );
            inherentVerbs.Add( new EmoteVerb() );
            relatedPages = new List<WaywardEngine.Page>();

            commands = new Dictionary<string[], InputManager.InputDelegate>();
            commands.Add(new string[] { "view", "look", "observe", "l" }, ParseView);
        }

        #region Control

        public virtual void Control( GameObject obj )
        {
            verbs.Clear();

            foreach( Verb verb in inherentVerbs ) {
                verb.self = obj;
                verbs.Add(verb);
            }

            if( controlledObject != null ) {
                bool success = controlledObject.SetActor(null);
                
                if( !success ) { return; }
                controlledObject.OnActionEvent -= OnObservedActionTaken;
            }

			controlledObject = obj;

            if( controlledObject != null ) {
                bool success = controlledObject.SetActor(this);
                if( !success ) {
                    controlledObject = null;
                    return;
                }
            }

            controlledObject.OnActionEvent += OnObservedActionTaken;
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

        #endregion

        #region Observation

        /// <summary>
        /// Returns a bool indicating whether obj can be perceived at all. For example, if it should
        /// appear in an Overview page.
        /// </summary>
        /// <param name="obj">Object to be observed.</param>
        /// <returns></returns>
        public abstract bool CanObserve( GameObject obj );

		public abstract GameObjectData Observe( GameObject obj );
		public abstract GameObjectData Observe( GameObject obj, string dataKey );

        #endregion

        #region Verbs

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

        #endregion

        #region Inputs

        public virtual bool ParseInput( InputEventArgs e )
        {
            if( e.parsed ) { return true; }

            if( InputManager.instance.CheckCommands(commands, e) ) {
                return true;
            }

            foreach( Verb verb in verbs ) {
                foreach( string i in verb.validInputs ) {
                    if( i == e.action ) {
                        if( verb.ParseInput(e) ) {
                            e.parsed = true;
                            return true;
                        }

                        break;
                    }
                }
            }

            return false;
        }
        public virtual bool ParseView( InputEventArgs e )
        {
            if( e.parsed ) { return true; }

            Point position = new Point(WaywardManager.instance.window.Width / 2, WaywardManager.instance.window.Height * 0.4);
            GameManager.instance.DisplayDescriptivePage(position, controlledObject);

            return true;
        }

		#endregion

		#region Pages

        public void AddPage( WaywardEngine.Page page )
        {
            relatedPages.Add(page);
        }
        public void RemovePage( WaywardEngine.Page page )
        {
            if( relatedPages.Contains(page) ) {
                relatedPages.Remove(page);
            }
        }

        #endregion
	}

	public enum PossessionType {
        EMBODIMENT,
        INTERACTION,
        CONTENT
    }
}
