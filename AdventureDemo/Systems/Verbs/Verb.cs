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

        public string[] validInputs { get; protected set; }

        readonly public GameObject self;

        public Verb( GameObject self )
        {
            this.self = self;
            validInputs = new string[0];
        }

        /// <summary>
        /// Returns a bool indicating whether this Verb's action can be performed
        /// based on the passed in data.
        /// </summary>
        /// <param name="data">Arbitrary key-value dictionary to be used for parameter passing.</param>
        /// <returns></returns>
        public abstract CheckResult Check( GameObject target );
        public abstract bool Action( GameObject target );

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
