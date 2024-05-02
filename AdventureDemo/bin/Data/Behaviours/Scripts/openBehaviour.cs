using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

//css_include ../Data/Behaviours/Scripts/defaultBehaviour.cs;

public class OpenBehaviour : DefaultBehaviour
{
	public override bool Construct(Behaviour behaviour, Dictionary<string, object> data)
	{
		GameManager.instance.world.OnWorldLoaded += behaviour.OnWorldLoaded;
		behaviour.SetType("interactable");

		behaviour.blackboard["isOpen"] = false;

		return true;
	}

	public override bool OnWorldLoaded(Behaviour behaviour)
	{
		behaviour.blackboard["connection"] = GetOwnConnection(behaviour);

		return true;
	}

	public Connection GetOwnConnection(Behaviour behaviour)
	{
		ContainerAttachmentPoint container = behaviour.self.attachPoint as ContainerAttachmentPoint;
		if (container != null) {
			foreach (Connection connection in container.GetConnections()) {
				foreach (GameObject blockingObject in connection.blockingObjects) {
					if (blockingObject == behaviour.self) {
						return connection;
					}
				}
			}
		}
		return null;
	}

	public override Dictionary<string, object> Interact(Behaviour behaviour, GameObject interactor, Dictionary<string, object> data)
	{
		bool isOpen = (bool)behaviour.blackboard["isOpen"];
		string label = isOpen ? "close" : "open";

		Connection connection = (Connection)behaviour.blackboard["connection"];
		if (connection == null) {
			data["message"] = new ObservableText($"[0] try to { label } the [1]. But it is not attached to an opening.",
			new Tuple<GameObject, string>(interactor, "name top"),
			new Tuple<GameObject, string>(behaviour.self, "name")
		);
			data["displayAfter"] = false;
			return data;
		}

		if (isOpen) {
			connection.blockingObjects.Add(behaviour.self as Physical);
		} else {
			connection.blockingObjects.Remove(behaviour.self as Physical);
		}
		behaviour.blackboard["isOpen"] = !isOpen;

		data["message"] = new ObservableText($"[0] { label } the [1].",
			new Tuple<GameObject, string>(interactor, "name top"),
			new Tuple<GameObject, string>(behaviour.self, "name")
		);
		data["displayAfter"] = false;

		return data;
	}
}
