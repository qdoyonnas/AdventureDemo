using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    public enum CheckResult {
        INVALID,
        RESTRICTED,
        VALID
    }

    static class PhysicalUtilities
    {

        public static Physical FindParentPhysical( Physical obj )
        {
            if( obj.attachedTo != null ) {
                return FindParentPhysical(obj.attachedTo);
            }

            return obj;
        }
    }
}
