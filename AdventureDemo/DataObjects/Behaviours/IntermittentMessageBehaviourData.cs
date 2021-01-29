using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
	class IntermittentMessageBehaviourData : BehaviourData
	{
		public double interval = 1000.0;
		public string message = null;

		public IntermittentMessageBehaviourData() : base() { }
		public IntermittentMessageBehaviourData( IntermittentMessageBehaviourData data )
			: base(data)
		{
			interval = data.interval;
			message = data.message;
		}

		public override Dictionary<string, object> GenerateData(Dictionary<string, object> context = null)
		{
			Dictionary<string, object> data = base.GenerateData(context);

			data["interval"] = interval;
			data["message"] = message;

			return data;
		}

		protected override object CreateInstance(Dictionary<string, object> context = null)
		{
			return new IntermittentMessageBehaviour(GenerateData(context));
		}
	}
}
