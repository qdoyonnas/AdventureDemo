using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
	class EmoteVerb : Verb
	{
		public EmoteVerb() : base() { }
        public EmoteVerb( GameObject self ) : base(self) {}

		 protected override void Construct()
        {
            _displayLabel = "Emote";

            actionTime = 10;

            _validInputs = new string[] {
                "do", "emote"
            };
        }

		bool DisplayDialog()
		{
            // Open dialog for choosing time
            DialogPage dialogPage =  new DialogPage();
            dialogPage.SetTitle("Action");

            dialogPage.AddInputPanel((input) => {
                Register(new Dictionary<string, object>() {{ "message", input }}, true);
            });

            WaywardManager.instance.AddPage(dialogPage, WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));

            return true;
		}

		public override bool Action(Dictionary<string, object> data)
		{
			string message = null;
			if( data.ContainsKey("message") ) {
				message = data["message"] as string;
			}
			if( message == null ) { return false; }

			// Create data dictionary to be passed to observers
			Dictionary<string, object> actionData = new Dictionary<string, object>();

            // Message for Verbose pages
            actionData["message"] = new ObservableText($"[0] { message }.", 
                new Tuple<GameObject, string>(self, "name top")
            );
            actionData["turnPage"] = false;
            actionData["displayAfter"] = false;

            TimelineManager.instance.OnAction(actionData);

            return true;
		}

		public override CheckResult Check(GameObject target)
		{
			return CheckResult.VALID;
		}

		public override bool ParseInput(InputEventArgs e)
        {
            if( e.parsed ) { return true; }

            if( e.parameters.Length == 0 ) {
                return DisplayDialog();
            } else {
                string message = e.parameters[0];

                Register(new Dictionary<string, object>() {{ "message", message }}, true);
            }

            return true;
        }

        public override void Display(Actor actor, GameObject target, FrameworkContentElement span)
        {
            CheckResult check = Check(target);
			if( check >= CheckResult.VALID ) {
                ContextMenuHelper.AddContextMenuItem( span, displayLabel, delegate { DisplayDialog(); return false; } );
            }
        }

        protected override void OnAssign() {}
	}
}
