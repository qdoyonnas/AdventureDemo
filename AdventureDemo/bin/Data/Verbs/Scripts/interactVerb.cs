using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;
using AdventureCore;

//css_include ../Data/Verbs/Scripts/defaultVerb.cs;

public class GrabVerb : DefaultVerb
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
        IInteractable target = null;
        if (data.ContainsKey("target")) {
            target = data["target"] as IInteractable;
        }
        if (target == null) {
            return null;
        }

        CheckResult result = verb.Check(target);
        if (result.value != CheckValue.VALID) {
            WaywardManager.instance.DisplayMessage(result.messages[0]);
            return null;
        }

        data = target.Interact(self, data);

        return data;
    }

    public override CheckResult Check(Verb verb, GameObject target)
    {
        IInteractable interactable = target as IInteractable;
        if (interactable == null) {
            return new CheckResult(CheckValue.INVALID, $@"No way to interact with that");
        }

        CheckResult result = interactable.CanInteract(self);
        return result;
    }

    public override bool Display(Verb verb, Actor actor, GameObject target, FrameworkContentElement span)
    {

        return false;
    }

    public override bool ParseInput(Verb verb, InputEventArgs inputEventArgs)
    {
        if (inputEventArgs.parsed) { return true; }
        if (inputEventArgs.words.Length <= 1) { return false; }

        WaywardManager.instance.DisplayMessage($"Interact with {inputEventArgs.words[1]}");
    }
}
