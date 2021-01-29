using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WaywardEngine;

namespace AdventureCore
{
    class GameObject
    {
        #region Fields

        public string name;
        public string nickname; // XXX: Temp until knowledge is implemented
        public string description;

        public List<string> tags;

        protected Actor _actor;
        public virtual Actor actor {
            get {
                return _actor;
            }
        }

        // Attachments
        protected AttachmentPoint _container;
        public virtual AttachmentPoint container {
            get {
                return _container;
            }
        }
        
        protected List<AttachmentType> _attachmentTypes;
        public virtual List<AttachmentType> attachmentTypes {
            get {
                return _attachmentTypes;
            }
        }

        // Behaviours
        protected Dictionary<PossessionType, List<Verb>> verbs;
        protected List<BehaviourStrategy> behaviours;

        // Object data
        protected delegate GameObjectData DataDelegate( string[] parameters );
        protected Dictionary<string, DataDelegate> objectData;

        protected List<DataDelegate> relevantData;

        #endregion

        #region Constructors

        public GameObject( Dictionary<string, object> data )
        {
            Construct();

            name = data.ContainsKey("name") ? (string)data["name"] : name;
            description = data.ContainsKey("description") ? (string)data["description"] : description;

            if( data.ContainsKey("attachmentTypes") ) {
                foreach( AttachmentType type in (AttachmentType[])data["attachmentTypes"] ) {
                    attachmentTypes.Add(type);
                }
            }

            if( data.ContainsKey("tags") ) {
                foreach( string tag in (string[])data["tags"] ) {
                    tags.Add(tag);
                }
            }
        }
        public GameObject()
        {
            Construct();
        }
        void Construct()
        {
            name = "unknown object";
            nickname = "";
            description = "a strange object";

            tags = new List<string>();

            objectData = new Dictionary<string, DataDelegate>();
            objectData["name"] = GetName;
            objectData["description"] = GetDescription;
            objectData["data"] = GetRelevantData;

            relevantData = new List<DataDelegate>();

            verbs = new Dictionary<PossessionType, List<Verb>>();
            behaviours = new List<BehaviourStrategy>();

            _attachmentTypes = new List<AttachmentType>();
        }

        public virtual bool SetContainer( AttachmentPoint newContainer )
        {
            bool result = newContainer != null && !newContainer.Contains(this);
            if( result ) { return false; }

            _container = newContainer;
            return true;
        }
        /// <summary>
        /// Sets the Actor controlling this GameObject.
        /// </summary>
        /// <param name="actor">Controlling Actor.</param>
        /// <param name="possession">Degree of possession affecting what verbs are collected.</param>
        /// <returns></returns>
        public virtual bool SetActor( Actor actor ) // XXX: Consider using type other than string
        {
            if( actor != null && this.actor != null ) { return false; }
            _actor = actor;

            return true;
        }

        #endregion

        #region Behaviour Methods

        public virtual void CollectVerbs( Actor actor, PossessionType possession )
        {
            if( actor == null ) { return; }

            if( !verbs.ContainsKey(possession) ) { return; }
            foreach( Verb verb in verbs[possession] ) {
                verb.AddVerb( actor );
            }
        }
        public virtual List<Verb> CollectVerbs()
        {
            List<Verb> collectedVerbs = new List<Verb>();
            foreach( List<Verb> verbCollection in verbs.Values ) {
                collectedVerbs.AddRange( verbCollection );
            }

            return collectedVerbs;
        }
        public virtual List<Verb> CollectVerbs( PossessionType possession )
        {
            List<Verb> collectedVerbs = new List<Verb>();

            if( verbs.ContainsKey(possession) ) {
                foreach( Verb verb in verbs[possession] ) {
                    collectedVerbs.Add(verb);
                }
            }

            return collectedVerbs;
        }
        public virtual void AddVerb( PossessionType possession, Verb verb )
        {
            if( verbs.ContainsKey(possession) ) {
                verbs[possession].Add(verb);
            } else {
                verbs.Add( possession, new List<Verb>() { verb } );
            }
        }
        public virtual void RemoveVerb( PossessionType possession, Verb verb )
        {
            if( !verbs.ContainsKey(possession) ) { return; }

            verbs[possession].Remove(verb);
            actor.CollectVerbs();
        }

        public virtual void AddBehaviour( BehaviourStrategy behaviour )
        {
            behaviours.Add(behaviour);
            behaviour.Initialize(this);
        }
        public virtual void RemoveBehaviour( BehaviourStrategy behaviour )
        {
            behaviours.Remove(behaviour);
        }

        #endregion

        #region Descriptive Methods

        /// <summary>
        /// Returns a String that best fufills the requested data.
        /// Serves a bridge between UI and objects disconnecting the need for explicit calls.
        /// </summary>
        /// <param name="data">A String identifying the desired data.</param>
        /// <returns></returns>
        public virtual GameObjectData GetData( string key )
        {
            key = key.ToLower();
            string[] parameters = key.Split(' ');

            key = parameters[0];
            parameters = parameters.Skip(1).ToArray();
            if( !objectData.ContainsKey(key) ) {
                return new GameObjectData();
            }
            return objectData[key](parameters);
        }
        public virtual GameObjectData GetName( params string[] parameters )
        {
            GameObjectData data = new GameObjectData();
            bool upper = parameters.Contains("upper");

            string alias = name;
            bool proper = parameters.Contains("proper");
            if( !proper && !string.IsNullOrEmpty(nickname) ) {
                alias = nickname;
            }

            bool complete = parameters.Contains("complete");
            if( complete && !string.IsNullOrEmpty(nickname) ) {
                alias = $"{nickname} ({name})";
            }

            data.text = upper ? char.ToUpper(alias[0]) + alias.Substring(1) : alias;

            data.SetSpan( data.text );
            data.span.Style = GameManager.instance.GetResource<Style>("Link");

            return data;
        }
        public virtual GameObjectData GetDescription( params string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            data.text = description;
            data.SetSpan(WaywardTextParser.Parse(description));

            return data;
        }
        public virtual GameObjectData GetRelevantData( params string[] parameters )
        {
            int index = 0;
            try {
                index = int.Parse(parameters[0]);
            } catch {}

            if( index < 0 ) {
                index = relevantData.Count - index;
            } else if( index >= relevantData.Count ) {
                return new GameObjectData();
            }

            parameters = parameters.Skip(1).ToArray();
            GameObjectData data = relevantData[index](parameters);
            return data;
        }

        public virtual List<DescriptivePageSection> DisplayDescriptivePage()
        {
            return new List<DescriptivePageSection> {
                new DebugDescriptivePageSection(),
                new GameObjectDescriptivePageSection(),
                new GameObjectVerbsDescriptivePageSection()
            };
        }

        #endregion

        #region Helper Methods

        public virtual List<GameObject> GetChildObjects()
        {
            List<GameObject> children = new List<GameObject>();

            return children;
        }

        public virtual bool MatchesSearch( Dictionary<string, string> properties )
        {
            foreach( KeyValuePair<string, string> property in properties.ToArray() ) {
                properties.Remove(property.Key);
                switch( property.Key ) {
                    case "name":
                        if( !SearchComparator.CompareString(name, property.Value) ) {
                            return false;
                        }
                        break;
                    case "description":
                        if( !description.Contains(property.Value) ) {
                            return false;
                        }
                        break;
                    case "tags":
                        foreach( string tag in property.Value.Split(',', ' ') ) {
                            if( string.IsNullOrEmpty(tag) ) { continue; }

                            if( !tags.Contains(tag) ) {
                                return false;
                            }
                        }
                        break;
                    // XXX: Add Verbs
                    default:
                        properties[property.Key] = property.Value;
                        break;
                }
            }

            return properties.Count == 0;
        }

        #endregion
    }

    #region GameObjectData

    public class GameObjectData
    {
        public string text;
        public Span span;

        public GameObjectData()
        {
            text = "";
            span = new Span();
        }

        public void SetSpan( string text )
        {
            SetSpan( new Run( text ) );
        }
        public void SetSpan( params Inline[] range )
        {
            span.Inlines.Clear();
            AddSpan( range );
        }

        public void AddSpan( string text )
        {
            AddSpan( new Run( text ) );
        }
        public void AddSpan( params Inline[] range )
        {
            span.Inlines.AddRange( range );
        }

        public override string ToString()
        {
            return text;
        }
    }

    #endregion
}
