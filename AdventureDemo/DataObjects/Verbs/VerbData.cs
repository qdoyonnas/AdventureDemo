using System;
using System.Collections.Generic;

namespace AdventureCore
{
    abstract class VerbData : BasicData
    {
        public string type;

        public string label;
        public string[] validInputs;

        public VerbData() 
        {
            validInputs = new string[0];
        }
        public VerbData( VerbData data )
        {
            type = data.type;
            label = data.label;

            validInputs = new string[data.validInputs.Length];
            Array.Copy(data.validInputs, validInputs, validInputs.Length);
        }

        public virtual Dictionary<string, object> GenerateData(Dictionary<string, object> context = null)
        {
            // XXX: 'Context' dictionary is being passed into the 'Data' dictionary used to instaniate the objects,
            //      this might be a problem.
            context = context == null ? new Dictionary<string, object>() : context;
            Dictionary<string, object> data = new Dictionary<string, object>(context);

            data["label"] = this.label;
            data["validInputs"] = this.validInputs;

            return data;
        }
    }
}
