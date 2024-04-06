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
		behaviour.SetType("interactable");
		return true;
	}

	public override Dictionary<string, object> Interact(Behaviour behaviour, GameObject interactor, Dictionary<string, object> data)
	{
		WaywardManager.instance.DisplayMessage($"{behaviour.self.GetName("upper")} interacted with by {interactor.GetName("upper")}");

		return data;
	}
}
