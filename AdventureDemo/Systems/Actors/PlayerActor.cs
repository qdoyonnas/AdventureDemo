using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class PlayerActor : Actor
    {
        private string controlledName; // XXX: Temporary until objects can be renamed in game

        public override void OnObservedActionTaken(Dictionary<string, object> data)
        {
            TextBlock text = null;
            if( data.ContainsKey("message") ) {
                text = data["message"] as TextBlock;
            }

            bool turnPage = false;
            if( data.ContainsKey("turnPage") ) {
                try {
                    turnPage = (bool)data["turnPage"];
                } catch {
                    // XXX: error log?
                }
            }

            bool displayAfter = false;
            if( data.ContainsKey("displayAfter") ) {
                try {
                    displayAfter = (bool)data["displayAfter"];
                } catch {
                    // XXX: error log?
                }
            }

            if( turnPage ) {
                OnTurnVerbosePages(!displayAfter);
            }
            if( text != null ) {
                OnMessageVerbosePages( text );
            }
            if( displayAfter ) {
                OnDisplayVerbosePages();
            }
        }

        public override void Control( GameObject obj )
        {
            if( controlledObject != null ) {
                controlledObject.nickname = controlledName;
            }

            base.Control(obj);

            controlledName = obj.nickname;
            obj.nickname = $"You";
        }

        public override bool CanObserve( GameObject obj )
        {
            // TODO: Implement senses

            GameObject thisContainer = controlledObject.container.GetParent();
            if( obj == thisContainer || obj.container == controlledObject.container ) {
                return true;
            } else if( obj.container != null ) {
                GameObject objContainer = obj.container.GetParent();
                if( objContainer != null ) {
                    return CanObserve(objContainer);
                }
            }

            return false;
        }

        public override GameObjectData Observe( GameObject obj )
        {
			return Observe(obj, "name");
        }
		public override GameObjectData Observe( GameObject obj, string dataKey )
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
    }
}
