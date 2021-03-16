using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
	class AnimalActor : Actor
	{
		Random random = new Random();

		public override bool Control(GameObject obj)
		{
			GameObject controlled = controlledObject;

			if( !base.Control(obj) ) {
				return false;
			}

			Dictionary<string, object> data = new Dictionary<string, object>();
			data["gameObject"] = controlled;
			TimelineManager.instance.ClearEvents(new TimelineManager.ClearEventFilter(data));

			Act();
			return true;
		}

		public override void OnObservedActionTaken(Dictionary<string, object> data)
		{
			base.OnObservedActionTaken(data);

			if( data.ContainsKey("gameObject") ) {
				 GameObject obj = data["gameObject"] as GameObject;
				if( obj == controlledObject ) {
					Act();
				}
			}
		}

		private void Act()
		{
			List<Verb> traversalVerbs = GetVerbs("TraversalVerb");
			if( traversalVerbs.Count > 0 ) {
				ContainerAttachmentPoint container = controlledObject.container as ContainerAttachmentPoint;
				if( container != null ) {
					Connection[] connections = container.GetConnections();
					int choice = random.Next(0, connections.Length);

					Dictionary<string, object> data = new Dictionary<string, object>();
					data["target"] = connections[choice];
					traversalVerbs[0].Register(data); // XXX: Should pick most appropriate traversal verb
				}
			}
		}
	}
}
