using System;
using System.Collections.Generic;
using WaywardEngine;

namespace AdventureCore
{
    public interface IBehaviourScript
    {
        bool Construct(Behaviour behaviour, Dictionary<string, object> data);
        bool OnAssign(Behaviour behaviour);
        Dictionary<string, object> Interact(Behaviour behaviour, GameObject interactor, Dictionary<string, object> data);
    }
}
