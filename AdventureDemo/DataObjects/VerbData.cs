using System;
using System.Collections.Generic;
using CSScriptLibrary;

namespace AdventureCore
{
    class VerbData : BasicData
    {
        public ScriptReference script;

        public string label;
        public string[] validInputs;

        public Dictionary<string, object> data;

        public VerbData() 
        {
            validInputs = new string[0];
            data = new Dictionary<string, object>();
        }
        public VerbData( VerbData inData )
        {
            script = inData.script;
            label = inData.label;

            validInputs = new string[inData.validInputs.Length];
            Array.Copy(inData.validInputs, validInputs, validInputs.Length);

            data = new Dictionary<string, object>();
            foreach( KeyValuePair<string, object> pair in inData.data ) {
                data[pair.Key] = pair.Value;
            }
        }

        public virtual Dictionary<string, object> GenerateData(Dictionary<string, object> context = null)
        {
            // XXX: 'Context' dictionary is being passed into the 'Data' dictionary used to instaniate the objects,
            //      this might be a problem.
            context = context == null ? new Dictionary<string, object>() : context;
            Dictionary<string, object> collectedData = new Dictionary<string, object>(context);

            collectedData["label"] = this.label;
            collectedData["validInputs"] = this.validInputs;

            foreach( KeyValuePair<string, object> entry in data ) {
                collectedData[entry.Key] = entry.Value;
            }

            return collectedData;
        }

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            Verb verb = CSScript.Evaluator.LoadCode<Verb>(script.GetCode(), GenerateData());

            return verb;
        }
    }
}
