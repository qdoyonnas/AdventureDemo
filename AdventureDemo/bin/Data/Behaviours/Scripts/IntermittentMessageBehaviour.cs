using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

public class IntermittentMessageBehaviour: IBehaviourScript
{
	public bool Construct(Behaviour behaviour, Dictionary<string, object> data)
	{
		behaviour.blackboard["interval"] = data.ContainsKey("interval") ? data["interval"] : 1000.0;
		behaviour.blackboard["message"] = data.ContainsKey("message") ? data["message"] : "";

		return true;
	}

	public bool OnAssign(Behaviour behaviour)
	{
		double interval = (double)behaviour.blackboard["interval"];

		Dictionary<string, object> data = new Dictionary<string, object>();
		data["gameObject"] = behaviour.self;
		data["behaviour"] = behaviour;
		data["label"] = behaviour.blackboard["message"];
		TimelineManager.instance.RegisterEvent(EmitMessage, data, interval);

		return true;
	}

	public Dictionary<string, object> EmitMessage(Dictionary<string, object> data)
	{
		Behaviour behaviour = (Behaviour)data["behaviour"];
		double interval = (double)behaviour.blackboard["interval"];
		string message = (string)behaviour.blackboard["message"];

		data["message"] = new ObservableText($@"[0] {message}",
				new Tuple<GameObject, string>(behaviour.self, "name upper")
		);

		TimelineManager.instance.RegisterEvent(EmitMessage, data, interval);

		return data;
	}
}
