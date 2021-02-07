using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
	abstract class ActorData : BasicData
	{
		public ActorData() :base() { }
		public ActorData(ActorData data)
			: base(data) {}
	}
}
