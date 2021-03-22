using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using CSScriptLibrary;
using WaywardEngine;

namespace AdventureCore
{
    public class Verb
    {
        protected string _displayLabel;
        public string displayLabel {
            get {
                return _displayLabel;
            }
            set {
                if( !string.IsNullOrEmpty(value) ) {
                    _displayLabel = value;
                }
            }
        }

        protected List<string> _validInputs = new List<string>();
        public string[] validInputs {
            get {
                return _validInputs.ToArray();
            }
        }

        protected GameObject _self;
        public GameObject self {
            get {
                return _self;
            }
            set {
                _self = value;
                OnAssign();
            }
        }

        public readonly Dictionary<string, object> blackboard = new Dictionary<string, object>();

        public Verb() 
        {
            SetDefaults();
        }
        public Verb(Dictionary<string, object> data)
        {
            SetDefaults();

            _displayLabel = data.ContainsKey("label") ? (string)data["label"] : _displayLabel;

            if( data.ContainsKey("validInputs") ) {
                try {
                    string[] dataInputs = (string[])data["validInputs"];
                    AddValidInput(dataInputs);
                } catch { }
            }
        }
        public Verb( GameObject self )
        {
            SetDefaults();
            this.self = self;
        }

        protected void SetDefaults()
        {
            ConstructMethod = ConstructDefault;
            OnAssignMethod = OnAssignDefault;
            CheckMethod = CheckDefault;
            ActionMethod = ActionDefault;
            RegisterMethod = RegisterDefault;
            DisplayMethod = DisplayDefault;
            AddVerbMethod = AddVerbDefault;
            ParseInputMethod = ParseInputDefault;
        }
        public void SetMethods(
                MethodDelegate<bool> construct,
                MethodDelegate<bool> onAssign,
                MethodDelegate<CheckResult> check,
                MethodDelegate<bool> action,
                MethodDelegate<bool> register,
                MethodDelegate<bool> display,
                MethodDelegate<bool> addVerb,
                MethodDelegate<bool> parseInput
            )
        {
            ConstructMethod = construct;
            OnAssignMethod = onAssign;
            CheckMethod = check;
            ActionMethod = action;
            RegisterMethod = register;
            DisplayMethod = display;
            AddVerbMethod = addVerb;
            ParseInputMethod = parseInput;
        }

        public void AddValidInput( params string[] inputs )
        {
            foreach( string input in inputs ) {
                if( !_validInputs.Contains(input) ) {
                    _validInputs.Add(input);
                }
            }
        }
        public void RemoveValidInput( params string[] inputs )
        {
            foreach( string input in inputs ) {
                if( _validInputs.Contains(input) ) {
                    _validInputs.Remove(input);
                }
            }
        }

        protected static bool ConstructDefault( params object[] parameters )
        {
            Verb verb = null;
            Dictionary<string, object> data = null;
            try {
                verb = (Verb)parameters[0];
                data = (Dictionary<string, object>)parameters[1];
            } catch( SystemException e ) {
                WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed parsing parameters into ConstructDefault:</red> {e}");
                return false;
            }

            if( data.ContainsKey("actionTime") ) {
                try {
                    verb.blackboard["actionTime"] = (Double)data["actionTime"];
                } catch { }
            } else {
                verb.blackboard["actionTime"] = (Double)0;
            }

            return true;
        }
        protected MethodDelegate<bool> ConstructMethod;
        public bool Construct( Dictionary<string, object> data )
        {
            if( ConstructMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a ConstructMethod.</yellow>");
            }

            return ConstructMethod(this, data);
        }

        protected static bool OnAssignDefault( params object[] parameters ) { return true; }
        protected MethodDelegate<bool> OnAssignMethod;
        /// <summary>
        /// Called when the verb is assigned a gameObject as 'self'.
        /// As an example, can be used to add necessary objects for the verb to function
        /// to the assigned gameObject.
        /// </summary>
        public bool OnAssign()
        {
            if( OnAssignMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a OnAssignMethod.</yellow>");
            }

            return OnAssignMethod(this);
        }

        protected static CheckResult CheckDefault( params object[] parameters )
        {
            return CheckResult.INVALID;
        }
        protected MethodDelegate<CheckResult> CheckMethod;
        /// <summary>
        /// Returns a bool indicating whether this Verb's action can be performed
        /// based on the passed in data.
        /// </summary>
        /// <param name="data">Arbitrary key-value dictionary to be used for parameter passing.</param>
        /// <returns></returns>
        public CheckResult Check( GameObject target )
        {
            if( CheckMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a CheckMethod.</yellow>");
            }

            return CheckMethod(this, target);
        }

        protected static bool ActionDefault( params object[] parameters )
        {
            Verb verb = null;
            try {
                verb = (Verb)parameters[0];

            } catch( SystemException e ) {
                WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed parsing parameters into ActionDefault:</red> {e}");
                return false;
            }

            WaywardEngine.WaywardManager.instance.Log($@"<yellow>Verb '{verb.displayLabel}' ran default action.</yellow>");
            return false;
        }
        protected MethodDelegate<bool> ActionMethod;
        public bool Action( Dictionary<string, object> data )
        {
            if( ActionMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a ActionMethod.</yellow>");
            }

            return ActionMethod(this, data);
        }

        protected static bool RegisterDefault( params object[] parameters )
        {
            Verb verb = null;
            Dictionary<string, object> data = null;
            bool fromPlayer = false;
            try {
                verb = (Verb)parameters[0];
                data = (Dictionary<string, object>)parameters[1];
                if( parameters.Length > 1 ) {
                    fromPlayer = (bool)parameters[2];
                }
            } catch( SystemException e ) {
                WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed parsing parameters into RegisterDefault:</red> {e}");
                return false;
            }

            if( !data.ContainsKey("gameObject") ) { data["gameObject"] = verb.self; }
            if( !data.ContainsKey("verb") ) { data["verb"] = verb; }
            if( !data.ContainsKey("label") ) { data["label"] = verb.displayLabel; }
            bool success = false;
            try {
                success = TimelineManager.instance.RegisterEvent( (d) => { verb.Action(d); }, data, (Double)verb.blackboard["actionTime"]);
            } catch( SystemException e ) {
                WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed registering action:</red> {e}");
                return false;
            }

            // XXX: Set the game objects current action here

            if( fromPlayer ) {
                if( success ) {
                    try {
                        GameManager.instance.Update((Double)verb.blackboard["actionTime"]);
                    } catch( SystemException e ) {
                        WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed updating GameManager:</red> {e}");
                        return false;
                    }
                }
                WaywardManager.instance.Update();
            }

            return success;
        }
        protected MethodDelegate<bool> RegisterMethod;
        public bool Register( Dictionary<string, object> data, bool fromPlayer = false )
        {
            if( RegisterMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a RegisterMethod.</yellow>");
            }

            return RegisterMethod(this, data, fromPlayer);
        }
        
        protected static bool DisplayDefault( params object[] parameters )
        {
            Verb verb = null;
            Actor actor = null;
            GameObject target = null;
            FrameworkContentElement span = null;
            try {
                verb = (Verb)parameters[0];
                actor = (Actor)parameters[1];
                target = (GameObject)parameters[2];
                span = (FrameworkContentElement)parameters[3];
            } catch( SystemException e ) {
                WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed parsing parameters into DisplayDefault:</red> {e}");
            }

            CheckResult check = verb.Check(target);
			if( check >= CheckResult.RESTRICTED ) {
                Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();
                if( check == CheckResult.RESTRICTED ) {
                    items.Add( WaywardTextParser.ParseAsBlock($@"<gray>{verb.displayLabel}</gray>") , null );
                } else {
                    items.Add( WaywardTextParser.ParseAsBlock(verb.displayLabel) , delegate { return verb.Register(new Dictionary<string, object>(){{ "target", target }}, true); } );
                }
                ContextMenuHelper.AddContextMenuHeader(span, new TextBlock(verb.self.GetData("name upper").span), items, check != CheckResult.RESTRICTED);
            }

            return true;
        }
        protected MethodDelegate<bool> DisplayMethod;
        public bool Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            if( DisplayMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a DisplayMethod.</yellow>");
            }

            return DisplayMethod(this, actor, target, span);
        }

        protected static bool AddVerbDefault( params object[] parameters )
        {
            Verb verb = null;
            Actor actor = null;
            try {
                verb = (Verb)parameters[0];
                actor = (Actor)parameters[1];
            } catch( SystemException e ) {
                WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed parsing parameters into AddVerbDefault:</red> {e}");
            }

            if( actor.HasVerb(verb.GetType()) ) { return false; }
            actor.AddVerb(verb);

            return true;
        }
        protected MethodDelegate<bool> AddVerbMethod;
        public bool AddVerb( Actor actor )
        {
            if( AddVerbMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a AddVerbMethod.</yellow>");
            }

            return AddVerbMethod(this, actor);
        }

        protected static bool ParseInputDefault( params object[] parameters )
        {
            Verb verb = null;
            InputEventArgs e = null;
            try {
                verb = (Verb)parameters[0];
                e = (InputEventArgs)parameters[1];
            } catch( SystemException x ) {
                WaywardManager.instance.Log($@"<red>Verb '{verb.displayLabel}' failed parsing parameters into ParseInputDefault:</red> {x}");
            }

            if( e.parsed ) { return true; }

            string message = $"Verb Found:\n{verb.displayLabel} with params:";

            for( int i = 1; i < e.words.Length; i++ ) {
                message += $"\n{e.words[i]}";
            }

            WaywardManager.instance.DisplayMessage(message);

            return true;
        }
        protected MethodDelegate<bool> ParseInputMethod;
        public bool ParseInput( InputEventArgs e )
        {
            if( ParseInputMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a ParseInputMethod.</yellow>");
            }

            return ParseInputMethod(this, e);
        }
    }
}
