using System;
using System.Collections.Generic;
using WaywardEngine;

namespace AdventureCore
{
	public class Behaviour
	{
		protected string _type;
		public string type {
			get { return _type; }
		}

        protected string _displayLabel;
        public string displayLabel {
            get {
                return _displayLabel;
            }
            set {
                if (!string.IsNullOrEmpty(value)) {
                    _displayLabel = value;
                }
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
		protected bool isInit = false;

		public readonly Dictionary<string, object> blackboard = new Dictionary<string, object>();

        public Behaviour()
        {
            SetDefaults();
        }
        public Behaviour(Dictionary<string, object> data)
        {
            SetDefaults();

            _displayLabel = data.ContainsKey("label") ? (string)data["label"] : _displayLabel;
        }
        public Behaviour(GameObject self)
        {
            SetDefaults();
            this.self = self;
        }

        public void SetType(string t)
		{
			if (_type == null) {
				_type = t;
			}
		}

        protected void SetDefaults()
        {
            ConstructMethod = ConstructDefault;
            OnAssignMethod = OnAssignDefault;
        }
        public void SetMethods(
                ConstructDelegate construct,
                OnAssignDelegate onAssign
            )
        {
            ConstructMethod = construct != null ? construct : ConstructDefault;
            OnAssignMethod = onAssign != null ? onAssign : OnAssignDefault;
        }

        public delegate bool ConstructDelegate(Behaviour behaviour, Dictionary<string, object> data);
        protected static bool ConstructDefault(Behaviour behaviour, Dictionary<string, object> data)
        {
            return true;
        }
        protected ConstructDelegate ConstructMethod;
        public bool Construct(Dictionary<string, object> data)
        {
            if (ConstructMethod == null) {
                WaywardManager.instance.Log($@"<yellow>Behaviour '{displayLabel}' doesn't have a ConstructMethod.</yellow>");
            }

            return ConstructMethod(this, data);
        }

        public delegate bool OnAssignDelegate(Behaviour behaviour);
        protected static bool OnAssignDefault(Behaviour behaviour) { return true; }
        protected OnAssignDelegate OnAssignMethod;
        /// <summary>
        /// Called when the behaviour is assigned a gameObject as 'self'.
        /// As an example, can be used to add necessary objects for the behaviour to function
        /// to the assigned gameObject.
        /// </summary>
        public bool OnAssign()
        {
            if (OnAssignMethod == null) {
                WaywardManager.instance.Log($@"<yellow>Behaviour '{displayLabel}' doesn't have a OnAssignMethod.</yellow>");
            }

            return OnAssignMethod(this);
        }
    }
}
