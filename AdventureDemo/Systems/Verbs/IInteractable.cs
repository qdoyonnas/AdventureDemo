using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    public interface IInteractable
    {
        CheckResult CanInteract(GameObject interactor);
        Dictionary<string, object> Interact(GameObject interactor, Dictionary<string, object> data);
    }
}
