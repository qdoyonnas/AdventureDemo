using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;

namespace AdventureCore
{
    class CreateVerb : Verb
    {
        public CreateVerb() : base() { }
        public CreateVerb( Dictionary<string, object> data )
            : base(data) {}
        public CreateVerb(GameObject self) : base(self) {}

        protected override void Construct()
        {
            _displayLabel = "Create";
            _validInputs = new string[] { "create" };
        }
        protected override void OnAssign() {}

        public override CheckResult Check( GameObject target ) { return CheckResult.INVALID; }

        public override bool Action( Dictionary<string, object> data )
        {
            WaywardManager.instance.Update();
            return true;
        }

        public override bool ParseInput( InputEventArgs e )
        {
            if( e.parsed ) { return true; }
            if( e.parameters.Length <= 0 ) {
                WaywardManager.instance.DisplayMessage("Create what?");
                return true;
            }

            // XXX: Should this iterate over all e.parameters to spawn multiple objects at once?
            //      Though it is common principle that data ids should be one word it *isn't* enforced
            GameObject createdObject = DataManager.instance.LoadObject<GameObject>(e.parameterInput, typeof(ObjectData));
            if( createdObject == null ) {
                WaywardManager.instance.DisplayMessage($"Could not create object '{e.parameterInput}'.");
                return true;
            }

            if( self.container.CanAttach(createdObject) != CheckResult.VALID ) {
                WaywardManager.instance.DisplayMessage($"Could not place created object '{e.parameterInput}' in current space.");
                return true;
            }

            self.container.Attach(createdObject);
            Action(new Dictionary<string, object>());

            return true;
        }
    }
}
