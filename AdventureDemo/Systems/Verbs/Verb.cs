using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    abstract class Verb
    {
        protected string _displayLabel;
        public string displayLabel {
            get {
                return _displayLabel;
            }
        }

        public string[] _validInputs = new string[0];
        public string[] validInputs { 
            get {
                return _validInputs;
            }
        }

        private GameObject _self;
        public GameObject self {
            get {
                return _self;
            }
            set {
                _self = value;
                OnAssign();
            }
        }

        public double actionTime = 0; // XXX: Temporary hard coded value

        public Verb() 
        {
            Construct();
        }
        public Verb(Dictionary<string, object> data)
        {
            Construct();

            _displayLabel = data.ContainsKey("label") ? (string)data["label"] : _displayLabel;

            if( data.ContainsKey("validInputs") ) {
                try {
                    string[] dataInputs = (string[])data["validInputs"];
                    Array.Copy( dataInputs, 0, _validInputs, 0, dataInputs.Length );
                } catch { }
            }
        }
        public Verb( GameObject self )
        {
            Construct();
            this.self = self;
        }

        protected abstract void Construct();

        /// <summary>
        /// Called when the verb is assigned a gameObject as 'self'.
        /// As an example, can be used to add necessary objects for the verb to function
        /// to the assigned gameObject.
        /// </summary>
        protected abstract void OnAssign();

        /// <summary>
        /// Returns a bool indicating whether this Verb's action can be performed
        /// based on the passed in data.
        /// </summary>
        /// <param name="data">Arbitrary key-value dictionary to be used for parameter passing.</param>
        /// <returns></returns>
        public abstract CheckResult Check( GameObject target );
        public abstract bool Action( Dictionary<string, object> data );

        public virtual bool Register( Dictionary<string, object> data, bool fromPlayer = false )
        {
            bool success = TimelineManager.instance.RegisterEvent( () => { Action(data); }, self, this, actionTime );

            // XXX: Set the game objects current action here

            if( fromPlayer ) {
                if( success ) {
                    GameManager.instance.Update(actionTime);
                }
                WaywardManager.instance.Update();
            }

            return success;
        }

        public virtual void Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            CheckResult check = Check(target);
			if( check >= CheckResult.RESTRICTED ) {
                Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();
                if( check == CheckResult.RESTRICTED ) {
                    items.Add( WaywardTextParser.ParseAsBlock($@"<gray>{displayLabel}</gray>") , null );
                } else {
                    items.Add( WaywardTextParser.ParseAsBlock(displayLabel) , delegate { return Register(new Dictionary<string, object>(){{ "target", target }}, true); } );
                }
                ContextMenuHelper.AddContextMenuHeader(span, new TextBlock(self.GetData("name upper").span), items, check != CheckResult.RESTRICTED);
            }
        }

        public virtual void AddVerb( Actor actor )
        {
            if( actor.HasVerb(this.GetType()) ) { return; }
            actor.AddVerb(this);
        }

        public virtual bool ParseInput( InputEventArgs e )
        {
            if( e.parsed ) { return true; }

            string message = $"Verb Found:\n{displayLabel} with params:";

            for( int i = 1; i < e.words.Length; i++ ) {
                message += $"\n{e.words[i]}";
            }

            WaywardManager.instance.DisplayMessage(message);

            return true;
        }
    }
}
