using System;
using System.Collections.Generic;
using WaywardEngine;

namespace AdventureCore
{
	class IntermittentMessageBehaviour : BehaviourStrategy
	{
		double interval = 1000.0;
		string message = null;

		public IntermittentMessageBehaviour( Dictionary<string, object> data )
		{
			if( data.ContainsKey("interval") ) {
				try {
					interval = (double)data["interval"];
				} catch { }
			}

			if( data.ContainsKey("message") ) {
				try {
					message = (string)data["message"];
				} catch { }
			}
		}
		public IntermittentMessageBehaviour(string message, double interval)
		{
			this.message = message;
			this.interval = interval;
		}

		public override void Initialize(GameObject self)
		{
			if( isInit ) { return; }

			this.self = self;
			
			Dictionary<string, object> data = new Dictionary<string, object>();
			data["gameObject"] = self;
			TimelineManager.instance.RegisterEvent(EmitMessage, data, interval);

			isInit = true;
		}

		public override void Update() {}

		private void EmitMessage()
		{
			TimelineManager.instance.OnAction(new Dictionary<string, object>() {
				{ "message", new ObservableText($@"[0] {message}",
					new Tuple<GameObject, string>(self, "name upper")) }
			});
		
			Dictionary<string, object> data = new Dictionary<string, object>();
            data["gameObject"] = self;
			TimelineManager.instance.RegisterEvent(EmitMessage, data, interval);
		}
	}
}
