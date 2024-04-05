using System;
using System.Collections.Generic;
using System.Windows;
using WaywardEngine;

namespace AdventureCore
{
	public interface IVerbScript
	{
		bool Construct(Verb verb, Dictionary<string, object> data);
        bool OnAssign(Verb verb);
        CheckResult Check(Verb verb, GameObject target);
        Dictionary<string, object> Action(Verb verb, Dictionary<string, object> data);
        bool Register(Verb verb, Dictionary<string, object> data, bool fromPlayer = false);
        bool Display(Verb verb, Actor actor, GameObject target, FrameworkContentElement span);
        bool AddVerb(Verb verb, Actor actor);
        bool ParseInput(Verb verb, InputEventArgs e);
    }
}
