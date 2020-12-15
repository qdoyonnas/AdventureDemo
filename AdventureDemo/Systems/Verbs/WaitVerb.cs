using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class WaitVerb : Verb
    {
        public WaitVerb() : base() { }
        public WaitVerb( GameObject self ) : base(self) { }

        protected override void Construct()
        {
            _displayLabel = "Wait";

            actionTime = 500;

            _validInputs = new string[] { "wait" };
        }
        
        public override bool Action(GameObject target)
        {
            // Open dialog for choosing time
            DialogPage dialogPage =  new DialogPage();
            dialogPage.SetTitle("Wait Time");

            dialogPage.AddEntry("100", () => { WaitAction(100); });
            dialogPage.AddEntry("500", () => { WaitAction(500); });
            dialogPage.AddEntry("1000", () => { WaitAction(1000); });

            WaywardManager.instance.AddPage(dialogPage, WaywardManager.instance.GetRelativeWindowPoint(0.5, 0.5));
            return true;
        }

        protected bool WaitAction(double duration)
        {
            // Create data dictionary to be passed to observers
            Dictionary<string, object> data = new Dictionary<string, object>();

            actionTime = duration;

            // Message for Verbose pages
            data["message"] = new ObservableText($"[0] {displayLabel.ToLower()} for {actionTime.ToString()}.",
                new Tuple<GameObject, string>(self, "name top"));
            data["turnPage"] = true;
            data["displayAfter"] = true;

            self.OnAction(data);

            return true;
        }

        public override CheckResult Check(GameObject target) 
        {
            if( target == self ) {
                return CheckResult.VALID;
            } else {
                return CheckResult.INVALID;
            }
        }

        protected override void OnAssign() {}

        public override bool ParseInput(InputEventArgs e)
        {
            if( e.parsed ) { return true; }

            if( e.parameters.Length == 0 ) {
                return Action(null);
            } else {
                double waitTime;
                bool success = double.TryParse(e.parameters[0], out waitTime);
                if( !success ) { return false; }

                WaitAction(waitTime);
            }

            return true;
        }

        public bool Register(double time, bool fromPlayer = false)
        {
            bool success = TimelineManager.instance.RegisterEvent( () => { WaitAction(time); }, self, this, actionTime );

            // XXX: Set the game objects current action here

            if( fromPlayer ) {
                if( success ) {
                    GameManager.instance.Update(actionTime);
                }
                WaywardManager.instance.Update();
            }

            return success;
        }

        public override void Display(Actor actor, GameObject target, FrameworkContentElement span)
        {
            CheckResult check = Check(target);
			if( check >= CheckResult.VALID ) {
                Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();

                items.Add( WaywardTextParser.ParseAsBlock("10") , delegate { return Register(10.0, true); } );
                items.Add( WaywardTextParser.ParseAsBlock("100") , delegate { return Register(100.0, true); } );
                items.Add( WaywardTextParser.ParseAsBlock("500") , delegate { return Register(500.0, true); } );
                items.Add( WaywardTextParser.ParseAsBlock("1000") , delegate { return Register(1000.0, true); } );

                ContextMenuHelper.AddContextMenuHeader( span, WaywardTextParser.ParseAsBlock(displayLabel), items, true );
            }
        }
    }
}
