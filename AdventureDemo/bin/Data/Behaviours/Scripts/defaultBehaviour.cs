using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;
using AdventureCore;

public class DefaultBehaviour : IBehaviourScript
{
    public virtual bool Construct(Behaviour behaviour, Dictionary<string, object> data)
    {
        return true;
    }
    public virtual bool OnAssign(Behaviour behaviour)
    {
        return true;
    }
    public virtual Dictionary<string, object> Interact(Behaviour behaviour, GameObject interactor, Dictionary<string, object> data)
    {
        return data;
    }
}