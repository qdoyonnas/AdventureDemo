using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    public class PlayerActor : Actor
    {
        protected readonly Dictionary<string[], InputManager.InputDelegate> commands;

        private string controlledName; // XXX: Temporary until objects can be renamed in game

        public PlayerActor()
            :base()
        {
            commands = new Dictionary<string[], InputManager.InputDelegate>();
            commands.Add(new string[] { "view", "look", "observe", "l" }, ParseView);
        }

        public override bool Control( GameObject obj )
        {
            if( controlledObject != null ) {
                controlledObject.nickname = controlledName;
            }

            if( !base.Control(obj))  {
                return false;
            }

            controlledName = obj.nickname;
            obj.nickname = $"You";
            return true;
        }

		#region Observation

		public List<GameObject> GetSubjectObjects()
        {
            // TODO: Senses here?

            List<GameObject> subjects = new List<GameObject>();

            subjects.Add(controlledObject);

            return subjects;
        }

        public GameObjectData Observe( GameObject obj )
        {
			return Observe(obj, "name");
        }
		public GameObjectData Observe( GameObject obj, string dataKey )
        {
			// TODO: Implement senses

			GameObjectData data = obj.GetData(dataKey);

            IVerbSuggest objSuggest = obj as IVerbSuggest;

            bool isDefaultSet = false;
			if( dataKey.Contains("name") ) {
				foreach( Verb verb in verbs) {
                    bool verbDisplayed = false;
                    if( objSuggest != null ) {
                        verbDisplayed = objSuggest.DisplayVerb(verb, data.span);
                        isDefaultSet = isDefaultSet | objSuggest.SetDefaultVerb(verb, data.span);
                    }
                    if( !verbDisplayed ) {
                        verb.Display(this, obj, data.span);
                    }
				}

                ContextMenuHelper.AddContextMenuItem( data.span, "View", delegate { GameManager.instance.DisplayDescriptivePage(obj); return false; } );

                if( !isDefaultSet ) {
                    data.span.MouseLeftButtonUp += delegate { GameManager.instance.DisplayDescriptivePage(obj); };
                }
            }

			return data;
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
    }
}
