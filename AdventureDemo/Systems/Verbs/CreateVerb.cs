using System;
using System.Windows;
using WaywardEngine;

namespace AdventureDemo
{
    class CreateVerb : Verb
    {
        public CreateVerb() : base() { }
        public CreateVerb(GameObject self) : base(self) {}

        protected override void Construct()
        {
            _displayLabel = "Create";
            _validInputs = new string[] { "create" };

            updatesGame = false;
        }
        protected override void OnAssign() {}

        public override CheckResult Check( GameObject target ) { return CheckResult.INVALID; }

        public override bool Action(GameObject target)
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

            int index = e.input.IndexOf(" ");
            string input = e.input.Substring(index).Trim();
            GameObject createdObject = DataManager.instance.LoadObject<GameObject>(input, typeof(ObjectData));
            if( createdObject == null ) {
                WaywardManager.instance.DisplayMessage($"Could not create object '{input}'.");
                return true;
            }

            if( self.container.CanAttach(createdObject) != CheckResult.VALID ) {
                WaywardManager.instance.DisplayMessage($"Could not place created object '{input}' in current space.");
                return true;
            }

            self.container.Attach(createdObject);
            Action(createdObject);

            return true;
        }
    }
}
