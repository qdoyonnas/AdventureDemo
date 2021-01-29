using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
	abstract class BehaviourData : BasicData
	{
		public BehaviourData() :base() { }
		public BehaviourData(BehaviourData data)
			: base(data) {}

		public virtual Dictionary<string, object> GenerateData( Dictionary<string, object> context = null )
		{
			Dictionary<string, object> data = context == null ? new Dictionary<string, object>() : context;

			return data;
		}
	}
}
