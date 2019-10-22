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
        public override bool CanObserve( GameObject obj )
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
			if( dataKey == "name" ) {
				foreach( Verb verb in verbs) {
                    if( objSuggest != null ) {
                        objSuggest.DisplayVerb(verb, data.span);
                        isDefaultSet = isDefaultSet | objSuggest.SetDefaultVerb(verb, data.span);
                    }
                    DisplayVerb(obj, verb, data.span);
				}
            }

            ContextMenuHelper.AddContextMenuItem( data.span, "View", delegate { obj.DisplayDescriptivePage(); } );

            if( !isDefaultSet ) {
                data.span.MouseLeftButtonUp += delegate { obj.DisplayDescriptivePage(); };
            }

			return data;
        }

        private void DisplayVerb(GameObject obj, Verb verb, FrameworkContentElement span)
        {
            CheckResult check = verb.Check(obj);
			if( check >= CheckResult.RESTRICTED ) {
                string text = verb.self == controlledObject ? $"{verb.displayLabel}" 
                    : $"{verb.self.GetData("name").text} - {verb.displayLabel}";

                if( check == CheckResult.RESTRICTED ) {
                    ContextMenuHelper.AddContextMenuItem(span, WaywardTextParser.ParseAsBlock($@"<gray>{text}</gray>") , null, false);
                } else {
                    ContextMenuHelper.AddContextMenuItem(span, WaywardTextParser.ParseAsBlock(text) , delegate { verb.Action(obj); }, true);
                }
            }
        }
    }
}
