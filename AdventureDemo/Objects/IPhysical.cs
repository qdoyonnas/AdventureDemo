using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    interface IPhysical
    {
        double GetVolume();
        double GetWeight();

        GameObjectData GetDescriptiveWeight( string[] parameters );
        GameObjectData GetDescriptiveVolume( string[] parameters );

        AttachmentPoint[] GetAttachmentPoints();
        int GetAttachmentCount();
        void AddAttachmentPoint( AttachmentPoint point );
        void RemoveAttachmentPoint( AttachmentPoint point );
    }
}
