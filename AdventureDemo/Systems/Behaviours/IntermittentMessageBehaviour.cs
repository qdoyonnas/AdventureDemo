using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
	class IntermittentMessageBehaviour : BehaviourStrategy
	{
		GameObject self;

		double interval = 1000.0;
		string message = null;

		bool isInit = false;

		public IntermittentMessageBehaviour(string message, double interval)
		{
			this.message = message;
			this.interval = interval;
		}

		public override void Initialize(GameObject self)
		{
			if( isInit ) { return; }

			this.self = self;

			TimelineManager.instance.RegisterEvent(EmitMessage, self, null, interval);

			isInit = true;
		}

		public override void Update() {}

		private void EmitMessage()
		{
			self.OnAction(new Dictionary<string, object>() {
				{ "message", message }
			});
		
			TimelineManager.instance.RegisterEvent(EmitMessage, self, null, interval);
		}
	}
}
