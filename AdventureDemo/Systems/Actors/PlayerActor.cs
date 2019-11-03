using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    class PlayerActor : Actor
    {
        private string controlledName; // Temporary until objects can be renamed in game

        public override void Control( GameObject obj )
        {
            if( controlledObject != null && !string.IsNullOrEmpty(controlledName) ) {
                controlledObject.name = controlledName;
            }

            base.Control(obj);

            controlledName = obj.name;
            obj.name = "You";
        }

        public override bool CanObserve( GameObject obj )
        {
            // TODO: Implement senses

            GameObject thisContainer = controlledObject.container.GetParent();
            if( obj == thisContainer || obj.container == controlledObject.container ) {
                return true;
            } else {
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
                    if( objSuggest != null ) {
                        objSuggest.DisplayVerb(verb, data.span);
                        isDefaultSet = isDefaultSet | objSuggest.SetDefaultVerb(verb, data.span);
                    }
                    verb.Display(this, obj, data.span);
				}
            }

            ContextMenuHelper.AddContextMenuItem( data.span, "View", delegate { GameManager.instance.DisplayDescriptivePage(obj); } );

            if( !isDefaultSet ) {
                data.span.MouseLeftButtonUp += delegate { GameManager.instance.DisplayDescriptivePage(obj); };
            }

			return data;
        }
    }
}
