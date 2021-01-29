using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
	abstract class BehaviourStrategy
	{
		public GameObject self;
		protected bool isInit = false;

		public abstract void Initialize(GameObject self);
		public abstract void Update();
	}
}
