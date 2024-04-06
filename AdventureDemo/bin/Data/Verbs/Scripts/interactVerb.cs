using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

//css_include ../Data/Verbs/Scripts/defaultVerb.cs;

public class InteractVerb : DefaultVerb
{
    public override bool Construct(Verb verb, Dictionary<string, object> data)
    {
        verb.SetType("InteractVerb");
        verb.displayLabel = "Interact";

        verb.blackboard["actionTime"] = 50.0;

        verb.AddValidInput("use", "interact", "activate");

        return true;
    }

    public override Dictionary<string, object> Action(Verb verb, Dictionary<string, object> data)
    {
        GameObject target = null;
        if (data.ContainsKey("target")) {
            target = data["target"] as GameObject;
        }
        if (target == null) {
            return null;
        }

        CheckResult result = verb.Check(target);
        if (result.value != CheckValue.VALID) {
            WaywardManager.instance.DisplayMessage(result.messages[0]);
            return null;
        }

        // XXX: Temporary interact with first behaviour
        Behaviour interactableBehaviour = null;
        foreach (Behaviour behaviour in target.GetBehaviours()) {
            if (behaviour.type == "interactable") {
                interactableBehaviour = behaviour;
                break;
            }
        }
        if (interactableBehaviour != null) {
            data = interactableBehaviour.Interact(verb.self, data);
        }

        return data;
    }

    public override CheckResult Check(Verb verb, GameObject target)
    {
        List<Behaviour> interactableBehaviours = new List<Behaviour>();
        foreach (Behaviour behaviour in target.GetBehaviours()) {
            if (behaviour.type == "interactable") {
                interactableBehaviours.Add(behaviour);
            }
        }

        // Filter based on interactableBehaviours.CheckInteract()

        if (interactableBehaviours.Count == 0) {
            return new CheckResult(CheckValue.INVALID, $@"No way to interact with that");
        }

        return new CheckResult(CheckValue.VALID);
    }

    public override bool ParseInput(Verb verb, InputEventArgs inputEventArgs)
    {
        if (inputEventArgs.parsed) { return true; }
        if (inputEventArgs.words.Length <= 1) { return false; }

        WaywardManager.instance.DisplayMessage($"Interact with {inputEventArgs.words[1]}");

        return true;
    }
}
