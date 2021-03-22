using System;
using System.Collections.Generic;
using CSScriptLibrary;

namespace AdventureCore
{
    class VerbData : BasicData
    {
        public ScriptReference script; // Optional way to contain all methods in a single script. Individually overwritten by following scripts
        public ScriptReference constructScript;
        public ScriptReference onAssignScript;
        public ScriptReference checkScript;
        public ScriptReference actionScript;
        public ScriptReference registerScript;
        public ScriptReference displayScript;
        public ScriptReference addVerbScript;
        public ScriptReference parseInputScript;

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
            constructScript = inData.constructScript;
            onAssignScript = inData.onAssignScript;
            checkScript = inData.checkScript;
            actionScript = inData.actionScript;
            registerScript = inData.registerScript;
            displayScript = inData.displayScript;
            addVerbScript = inData.addVerbScript;
            parseInputScript = inData.parseInputScript;

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

        protected virtual void PopulateScripts()
        {
            if( script == null ) { return; }

            constructScript = constructScript == null ? script : constructScript;
            onAssignScript = onAssignScript == null ? script : onAssignScript;
            checkScript = checkScript == null ? script : checkScript;
            actionScript = actionScript == null ? script : actionScript;
            registerScript = registerScript == null ? script : registerScript;
            displayScript = displayScript == null ? script : displayScript;
            addVerbScript = addVerbScript == null ? script : addVerbScript;
            parseInputScript = parseInputScript == null ? script : parseInputScript;
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

            PopulateScripts();
            try {
                verb.SetMethods(
                    CSScript.LoadMethod(constructScript.GetCode()).GetStaticMethod<bool>("Construct"),
                    CSScript.LoadMethod(onAssignScript.GetCode()).GetStaticMethod<bool>("OnAssign"),
                    CSScript.LoadMethod(checkScript.GetCode()).GetStaticMethod<CheckResult>("Check"),
                    CSScript.LoadMethod(actionScript.GetCode()).GetStaticMethod<bool>("Action"),
                    CSScript.LoadMethod(registerScript.GetCode()).GetStaticMethod<bool>("Register"),
                    CSScript.LoadMethod(displayScript.GetCode()).GetStaticMethod<bool>("Display"),
                    CSScript.LoadMethod(addVerbScript.GetCode()).GetStaticMethod<bool>("AddVerb"),
                    CSScript.LoadMethod(parseInputScript.GetCode()).GetStaticMethod<bool>("ParseInput")
                );
            } catch( SystemException e ) {
                WaywardEngine.WaywardManager.instance.Log($@"<red>VerbData with id '{id}' failed loading scripts:</red> {e}");
            }

            verb.Construct(data);

            return verb;
        }
    }
}
