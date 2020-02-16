using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
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
                InitVerb();
            }
        }

        public Verb() {}
        public Verb( GameObject self )
        {
            this.self = self;
        }

        protected abstract void Construct();
        protected abstract void InitVerb();

        /// <summary>
        /// Returns a bool indicating whether this Verb's action can be performed
        /// based on the passed in data.
        /// </summary>
        /// <param name="data">Arbitrary key-value dictionary to be used for parameter passing.</param>
        /// <returns></returns>
        public abstract CheckResult Check( GameObject target );
        public virtual bool Action( GameObject target )
        {
            WaywardManager.instance.Update(); // XXX: (UPDATE) This is the place to update the game properly
            return true;
        }

        public virtual void Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            CheckResult check = Check(target);
			if( check >= CheckResult.RESTRICTED ) {
                Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();
                if( check == CheckResult.RESTRICTED ) {
                    items.Add( WaywardTextParser.ParseAsBlock($@"<gray>{displayLabel}</gray>") , null );
                } else {
                    items.Add( WaywardTextParser.ParseAsBlock(displayLabel) , delegate { return Action(target); } );
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
