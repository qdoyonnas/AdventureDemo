﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using CSScriptLib;
using WaywardEngine;

namespace AdventureCore
{
    public class Verb
    {
        protected string _type;
        public string type
        {
            get { return _type; }
        }

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

        public void SetType(string t)
        {
            if( _type == null ) {
                _type = t;
            }
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
                ConstructDelegate construct,
                OnAssignDelegate onAssign,
                CheckDelegate check,
                ActionDelegate action,
                RegisterDelegate register,
                DisplayDelegate display,
                AddVerbDelegate addVerb,
                ParseInputDelegate parseInput
            )
        {
            ConstructMethod = construct != null ? construct : ConstructDefault;
            OnAssignMethod = onAssign != null ? onAssign : OnAssignDefault;
            CheckMethod = check != null ? check : CheckDefault;
            ActionMethod = action != null ? action : ActionDefault;
            RegisterMethod = register != null ? register : RegisterDefault;
            DisplayMethod = display != null ? display : DisplayDefault;
            AddVerbMethod = addVerb != null ? addVerb : AddVerbDefault;
            ParseInputMethod = parseInput != null ? parseInput : ParseInputDefault;
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

        public delegate bool ConstructDelegate( Verb verb, Dictionary<string, object> data );
        protected static bool ConstructDefault( Verb verb, Dictionary<string, object> data )
        {
            if( data.ContainsKey("actionTime") ) {
                try {
                    verb.blackboard["actionTime"] = (Double)data["actionTime"];
                } catch { }
            } else {
                verb.blackboard["actionTime"] = (Double)0;
            }

            return true;
        }
        protected ConstructDelegate ConstructMethod;
        public bool Construct( Dictionary<string, object> data )
        {
            if( ConstructMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a ConstructMethod.</yellow>");
                return false;
            }

            try {
                return ConstructMethod(this, data);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Verb '{displayLabel}' failed running ConstructMethod:</red> {e}");
                return false;
            }
        }

        public delegate bool OnAssignDelegate( Verb verb );
        protected static bool OnAssignDefault( Verb verb ) { return true; }
        protected OnAssignDelegate OnAssignMethod;
        /// <summary>
        /// Called when the verb is assigned a gameObject as 'self'.
        /// As an example, can be used to add necessary objects for the verb to function
        /// to the assigned gameObject.
        /// </summary>
        public bool OnAssign()
        {
            if( OnAssignMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a OnAssignMethod.</yellow>");
                return false;
            }

            try {
                return OnAssignMethod(this);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Verb '{displayLabel}' failed running OnAssignMethod:</red> {e}");
                return false;
            }
        }

        public delegate CheckResult CheckDelegate( Verb verb, GameObject target );
        protected static CheckResult CheckDefault( Verb verb, GameObject target )
        {
            return new CheckResult(CheckValue.INVALID);
        }
        protected CheckDelegate CheckMethod;
        /// <summary>
        /// Returns a CheckResult indicating whether this Verb's action can be performed
        /// based on the passed in data.
        /// </summary>
        /// <param name="target">GameObject to be checked against</param>
        /// <returns></returns>
        public CheckResult Check( GameObject target )
        {
            if( CheckMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a CheckMethod.</yellow>");
                return new CheckResult(CheckValue.INVALID);
            }

            try {
                return CheckMethod(this, target);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Verb '{displayLabel}' failed running CheckMethod:</red> {e}");
                return new CheckResult(CheckValue.INVALID);
            }
        }

        public delegate Dictionary<string, object> ActionDelegate( Verb verb, Dictionary<string, object> data );
        protected static Dictionary<string, object> ActionDefault( Verb verb, Dictionary<string, object> data )
        {
            WaywardManager.instance.Log($@"<yellow>Verb '{verb.displayLabel}' ran default action.</yellow>");
            return data;
        }
        protected ActionDelegate ActionMethod;
        public Dictionary<string, object> Action( Dictionary<string, object> data )
        {
            if( ActionMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have an ActionMethod.</yellow>");
                return data;
            }

            try {
                return ActionMethod(this, data);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Verb '{displayLabel}' failed running ActionMethod:</red> {e}");
                return data;
            }
        }

        public delegate bool RegisterDelegate( Verb verb, Dictionary<string, object> data, bool fromPlayer = false );
        protected static bool RegisterDefault( Verb verb, Dictionary<string, object> data, bool fromPlayer = false )
        {
            if( !data.ContainsKey("gameObject") ) { data["gameObject"] = verb.self; }
            if( !data.ContainsKey("verb") ) { data["verb"] = verb; }
            if( !data.ContainsKey("label") ) { data["label"] = verb.displayLabel; }
            bool success = false;
            try {
                success = TimelineManager.instance.RegisterEvent(verb.Action, data, (Double)verb.blackboard["actionTime"]);
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
        protected RegisterDelegate RegisterMethod;
        public bool Register( Dictionary<string, object> data, bool fromPlayer = false )
        {
            if( RegisterMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a RegisterMethod.</yellow>");
                return false;
            }

            try {
                return RegisterMethod(this, data, fromPlayer);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Verb '{displayLabel}' failed running RegisterMethod:</red> {e}");
                return false;
            }
        }
        
        public delegate bool DisplayDelegate( Verb verb, Actor actor, GameObject target, FrameworkContentElement span );
        protected static bool DisplayDefault( Verb verb, Actor actor, GameObject target, FrameworkContentElement span )
        {
            CheckResult check = verb.Check(target);
			if( check.value >= CheckValue.RESTRICTED ) {
                Dictionary<TextBlock, ContextMenuAction> items = new Dictionary<TextBlock, ContextMenuAction>();
                if( check.value == CheckValue.RESTRICTED ) {
                    items.Add( WaywardTextParser.ParseAsBlock($@"<gray>{verb.displayLabel}</gray>") , null );
                } else {
                    items.Add( WaywardTextParser.ParseAsBlock(verb.displayLabel) , delegate { return verb.Register(new Dictionary<string, object>(){{ "target", target }}, true); } );
                }
                ContextMenuHelper.AddContextMenuHeader(span, new TextBlock(verb.self.GetData("name upper").span), items, check.value != CheckValue.RESTRICTED);
            }

            return true;
        }
        protected DisplayDelegate DisplayMethod;
        public bool Display( Actor actor, GameObject target, FrameworkContentElement span )
        {
            if( DisplayMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a DisplayMethod.</yellow>");
                return false;
            }

            try {
                return DisplayMethod(this, actor, target, span);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Verb '{displayLabel}' failed running DisplayMethod:</red> {e}");
                return false;
            }
        }

        public delegate bool AddVerbDelegate( Verb verb, Actor actor );
        protected static bool AddVerbDefault( Verb verb, Actor actor )
        {
            if( actor.HasVerb(verb.type) ) { return false; }
            actor.AddVerb(verb);

            return true;
        }
        protected AddVerbDelegate AddVerbMethod;
        public bool AddVerb( Actor actor )
        {
            if( AddVerbMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have an AddVerbMethod.</yellow>");
                return false;
            }

            try {
                return AddVerbMethod(this, actor);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Verb '{displayLabel}' failed running AddVerbMethod:</red> {e}");
                return false;
            }
        }

        public delegate bool ParseInputDelegate( Verb verb, InputEventArgs inputEventArgs );
        protected static bool ParseInputDefault( Verb verb, InputEventArgs inputEventArgs )
        {
            if( inputEventArgs.parsed ) { return true; }

            string message = $"Verb Found:\n{verb.displayLabel} with params:";

            for( int i = 1; i < inputEventArgs.words.Length; i++ ) {
                message += $"\n{inputEventArgs.words[i]}";
            }

            WaywardManager.instance.DisplayMessage(message);

            return true;
        }
        protected ParseInputDelegate ParseInputMethod;
        public bool ParseInput( InputEventArgs inputEventArgs )
        {
            if( ParseInputMethod == null ) {
                WaywardManager.instance.Log($@"<yellow>Verb '{displayLabel}' doesn't have a ParseInputMethod.</yellow>");
                return false;
            }

            try {
                return ParseInputMethod(this, inputEventArgs);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Verb '{displayLabel}' failed running ParseInputMethod:</red> {e}");
                return false;
            }
        }
    }
}
