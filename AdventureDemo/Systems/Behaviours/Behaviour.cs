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
            OnWorldLoadedMethod = OnWorldLoadedDefault;
            InteractMethod = InteractDefault;
        }
        public void SetMethods(
                ConstructDelegate construct,
                OnAssignDelegate onAssign,
                OnWorldLoadedDelegate onWorldLoaded,
                InteractDelegate interact
            )
        {
            ConstructMethod = construct != null ? construct : ConstructDefault;
            OnAssignMethod = onAssign != null ? onAssign : OnAssignDefault;
            OnWorldLoadedMethod = onWorldLoaded != null ? onWorldLoaded : OnWorldLoadedDefault;
            InteractMethod = interact != null ? interact : InteractDefault;
        }

        public delegate bool ConstructDelegate(Behaviour behaviour, Dictionary<string, object> data);
        protected static bool ConstructDefault(Behaviour behaviour, Dictionary<string, object> data)
        {
            GameManager.instance.world.OnWorldLoaded += behaviour.OnWorldLoaded;
            return true;
        }
        protected ConstructDelegate ConstructMethod;
        /// <summary>
        /// Called when a behaviour has just been loaded from a script. Use to set initial values in blackboard
        /// </summary>
        /// <param name="data">(string, object> pairs of data to help instantiate the behaviour</param>
        /// <returns></returns>
        public bool Construct(Dictionary<string, object> data)
        {
            if (ConstructMethod == null) {
                WaywardManager.instance.Log($@"<yellow>Behaviour '{displayLabel}' doesn't have a ConstructMethod.</yellow>");
                return false;
            }

            try {
                return ConstructMethod(this, data);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Behaviour '{displayLabel}' failed running ConstructMethod:</red> {e}");
                return false;
            }
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
                return false;
            }

            try {
                return OnAssignMethod(this);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Behaviour '{displayLabel}' failed running OnAssignMethod:</red> {e}");
                return false;
            }
        }

        public delegate bool OnWorldLoadedDelegate(Behaviour behaviour);
        protected static bool OnWorldLoadedDefault(Behaviour behaviour) { return true; }
        protected OnWorldLoadedDelegate OnWorldLoadedMethod;
        public bool OnWorldLoaded()
        {
            if (OnWorldLoadedMethod == null) {
                WaywardManager.instance.Log($@"<yellow>Behaviour '{displayLabel}' doesn't have a OnWorldLoadedMethod.</yellow>");
                return false;
            }

            try {
                return OnWorldLoadedMethod(this);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Behaviour '{displayLabel}' failed running OnWorldLoadedMethod:</red> {e}");
                return false;
            }
        }

        public delegate Dictionary<string, object> InteractDelegate(Behaviour behaviour, GameObject interactor, Dictionary<string, object> data);
        protected static Dictionary<string, object> InteractDefault(Behaviour behaviour, GameObject interactor, Dictionary<string, object> data)
        {
            WaywardManager.instance.Log($@"<yellow>Verb '{behaviour.displayLabel}' ran default interact action.</yellow>");
            return data;
        }
        protected InteractDelegate InteractMethod;
        public Dictionary<string, object> Interact(GameObject interactor, Dictionary<string, object> data)
        {
            if (InteractMethod == null) {
                WaywardManager.instance.Log($@"<yellow>Behaviour '{displayLabel}' doesn't have an InteractMethod.</yellow>");
                return data;
            }

            try {
                return InteractMethod(this, interactor, data);
            } catch (SystemException e) {
                WaywardManager.instance.Log($@"<red>Behaviour '{displayLabel}' failed running InteractMethod:</red> {e}");
                return data;
            }
        }
    }
}
