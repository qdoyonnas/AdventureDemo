using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
	class BehaviourReference
	{
        public DataReference behaviour;

		public BehaviourReference() { }
        public BehaviourReference( BehaviourReference value )
        {
            behaviour = new DataReference(value.behaviour.value);
        }

        public BehaviourStrategy GetValue(Dictionary<string, object> context = null)
        {
            BehaviourStrategy b = behaviour.LoadData<BehaviourStrategy>(typeof(BehaviourData), context);

            return b;
        }
	}
}
