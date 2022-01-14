using System;
using System.Reflection;
using System.Collections.Generic;
using CSScriptLib;

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
            Dictionary<string, object> data = GenerateData(context);
            Verb verb = new Verb(data);

            //
            // Currently this will successfully load scripts, however the scripts have no access
            // To the Verb they are being assigned to. So they cannot access things like other methods or displayLabel
            // and the likes. I need to either pass the verb through to the methods whenever they are called, or wrap
            // the methods in an object that is assigned the verb <- not sure how to do this while keeping the scripts
            // separate.
            //

            try {
                IVerbScript verbScript = CSScript.Evaluator.LoadCode(script.GetCode()) as IVerbScript;
				verb.SetMethods(
                    verbScript.Construct,
					verbScript.OnAssign,
					verbScript.Check,
					verbScript.Action,
					verbScript.Register,
					verbScript.Display,
					verbScript.AddVerb,
					verbScript.ParseInput
				);
			} catch( SystemException e ) {
                WaywardEngine.WaywardManager.instance.Log($@"<red>VerbData with id '{id}' failed loading scripts:</red> {e}");
            }

            verb.Construct(data);

            return verb;
        }
    }
}
