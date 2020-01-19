using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media;
using WaywardEngine;

namespace AdventureDemo
{
    class GameObject
    {
        #region Fields

        public string name;
        public string description;

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

        protected Dictionary<PossessionType, List<Verb>> verbs;

        // Object data
        protected delegate GameObjectData DataDelegate( string[] parameters );
        protected Dictionary<string, DataDelegate> objectData;

        protected List<DataDelegate> relevantData;

        #endregion

        #region Constructors

        public static Dictionary<string, object> ParseData( ObjectData objectData )
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            data["name"] = objectData.name;
            data["description"] = objectData.description;

            return data;
        }

        public GameObject( Dictionary<string, object> data )
        {
            Construct(data.ContainsKey("name") ? (string)data["name"] : "Unknown Object");

            description = data.ContainsKey("description") ? (string)data["description"] : "a strange object";
        }
        public GameObject( string name )
        {
            Construct(name);
        }
        void Construct( string name )
        {
            this.name = name;
            this.description = "a strange object";

            objectData = new Dictionary<string, DataDelegate>();
            objectData["name"] = GetName;
            objectData["description"] = GetDescription;
            objectData["data"] = GetRelevantData;

            relevantData = new List<DataDelegate>();

            verbs = new Dictionary<PossessionType, List<Verb>>();

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

        #region Verb Methods

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
        public virtual GameObjectData GetName( string[] parameters )
        {
            GameObjectData data = new GameObjectData();
            bool upper = parameters.Length > 0 && parameters[0] == "upper";

            data.text = upper ? char.ToUpper(name[0]) + name.Substring(1) : name;

            data.SetSpan( data.text );
            data.span.Style = GameManager.instance.GetResource<Style>("Link");

            return data;
        }
        public virtual GameObjectData GetDescription( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            data.text = description;
            data.SetSpan(WaywardTextParser.Parse(description));

            return data;
        }
        public virtual GameObjectData GetRelevantData( string[] parameters )
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
    }

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
}
