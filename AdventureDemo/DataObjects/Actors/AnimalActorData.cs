using System;
using System.Collections.Generic;

namespace AdventureCore
{
	class AnimalActorData : ActorData
	{
		protected override object CreateInstance(Dictionary<string, object> context = null)
		{
			AnimalActor actor = new AnimalActor();

			return actor;
		}
	}
}
