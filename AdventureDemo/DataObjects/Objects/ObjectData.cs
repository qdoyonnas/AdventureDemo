using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaywardEngine;

namespace AdventureCore
{
    class ObjectData : BasicData
    {
        public string type = "ObjectData";

        public DynamicString name = new DynamicString("unknown object");
        public string description = "a strange object";
        public string[] tags = new string[0];

        public AttachmentType[] attachmentTypes = new AttachmentType[0];

        public VerbReference[] verbs = new VerbReference[0];
        public BehaviourReference[] behaviours = new BehaviourReference[0];

        public ObjectData() { }
        public ObjectData( ObjectData data )
            : base(data)
        {
            type = data.type;

            name = new DynamicString(data.name);
            description = data.description;

            tags = new string[data.tags.Length];
            Array.Copy(data.tags, tags, tags.Length);

            attachmentTypes = new AttachmentType[data.attachmentTypes.Length];
            Array.Copy(data.attachmentTypes, attachmentTypes, attachmentTypes.Length);

            verbs = new VerbReference[data.verbs.Length];
            for( int i = 0; i < verbs.Length; i++ ) {
                verbs[i] = new VerbReference(data.verbs[i]);
            }

            behaviours = new BehaviourReference[data.behaviours.Length];
            for( int i = 0; i < behaviours.Length; i++ ) {
                behaviours[i] = new BehaviourReference(data.behaviours[i]);
            }
        }

        public virtual Dictionary<string, object> GenerateData(Dictionary<string, object> context = null)
        {
            // XXX: 'Context' dictionary is being passed into the 'Data' dictionary used to instaniate the objects,
            //      this might be a problem.
            context = context == null ? new Dictionary<string, object>() : context;
            Dictionary<string, object> data = new Dictionary<string, object>(context);

            data["name"] = this.name.GetValue(data);
            data["description"] = this.description;
            data["tags"] = tags;

            data["attachmentTypes"] = this.attachmentTypes;

            return data;
        }
        protected override object CreateInstance(Dictionary<string, object> context = null)
        {
            GameObject gameObject = null;

            try {
                gameObject = new GameObject(GenerateData(context));
                
                PostInstantiate(gameObject, context);
            } catch( Exception e ) {
                WaywardManager.instance.Log($@"<red>ERROR: Could not instantiate GameObject from ObjectData: {e}</red>");
            }

            return gameObject;
        }

        protected virtual void PostInstantiate(GameObject gameObject, Dictionary<string, object> context = null)
        {
            foreach( VerbReference verbReference in verbs ) {
                KeyValuePair<Verb, PossessionType> verb = verbReference.GetValue();
                if( verb.Key == null ) {
                    WaywardManager.instance.Log($@"<red>ERROR: Invalid verb - Skipping</red>");
                    continue;
                }
                verb.Key.self = gameObject;
                gameObject.AddVerb(verb.Value, verb.Key);
            }

            foreach( BehaviourReference behaviourReference in behaviours ) {
                Behaviour behaviour = behaviourReference.GetValue();
                if (behaviour == null) {
                    WaywardManager.instance.Log($@"<red>ERROR: Invalid behaviour - Skipping</red>");
                    continue;
                }
                behaviour.self = gameObject;
                gameObject.AddBehaviour(behaviour);
            }
        }
    }
}
