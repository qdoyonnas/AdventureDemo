using System;
using System.Collections.Generic;
using CSScriptLib;

namespace AdventureCore
{
	class BehaviourData : BasicData
	{
		public ScriptReference script;

		public string label;

		public Dictionary<string, object> data;

		public BehaviourData() :base()
		{
			data = new Dictionary<string, object>();
		}
		public BehaviourData(BehaviourData inData)
			: base(inData)
		{
			script = inData.script;
			label = inData.label;

			data = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> pair in inData.data) {
				data[pair.Key] = pair.Value;
			}
		}

		public virtual Dictionary<string, object> GenerateData( Dictionary<string, object> context = null )
		{
			context = context == null ? new Dictionary<string, object>() : context;
			Dictionary<string, object> collectedData = new Dictionary<string, object>(context);

			collectedData["label"] = this.label;

			foreach (KeyValuePair<string, object> entry in data) {
				collectedData[entry.Key] = entry.Value;
			}

			return collectedData;
		}

        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
			Dictionary<string, object> data = GenerateData(context);
			Behaviour behaviour = new Behaviour(data);

			try {
				IBehaviourScript behaviourScript = CSScript.Evaluator.LoadCode(script.GetCode()) as IBehaviourScript;
				behaviour.SetMethods(
					behaviourScript.Construct,
					behaviourScript.OnAssign
				);
			}
			catch (SystemException e) {
				WaywardEngine.WaywardManager.instance.Log($@"<red>BehaviourData with id '{id}' failed loading scripts:</red> {e}");
			}

			behaviour.Construct(data);

			return behaviour;
		}
    }
}
