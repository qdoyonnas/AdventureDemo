﻿using System;
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
        protected string name;

        protected IContainer _container;
        public virtual IContainer container {
            get {
                return _container;
            }
        }

        protected Actor _actor;
        public virtual Actor actor {
            get {
                return _actor;
            }
        }

        protected Dictionary<PossessionType, Verb[]> verbs;

        protected delegate GameObjectData DataDelegate( string[] parameters );
        protected readonly Dictionary<string, DataDelegate> objectData;

        protected readonly List<DataDelegate> relevantData;

        public GameObject( string name, IContainer container )
        {
            this.name = name;

            objectData = new Dictionary<string, DataDelegate>();
            objectData["name"] = GetName;
            objectData["description"] = GetDescription;
            objectData["data"] = GetRelevantData;

            relevantData = new List<DataDelegate>();

            verbs = new Dictionary<PossessionType, Verb[]>();

            if( container != null ) {
                SetContainer(container);
            } else {
                GameManager.instance.AddRoot(this);
            }
        }

        public virtual bool SetContainer( IContainer newContainer )
        {
            if( newContainer == container ) { return true; }
            if( !newContainer.CanContain(this) ) { return false; }

            if( _container == null || _container.RemoveContent(this) ) {
                if( newContainer.AddContent(this) ) {
                    _container = newContainer;
                } else {
                    _container.AddContent(this);
                    return false;
                }
            }

            WaywardManager.instance.Update(); // XXX: Not where this should be // TODO: Game requires proper update sequence
            return true;
        }
        /// <summary>
        /// Sets the Actor controlling this GameObject.
        /// </summary>
        /// <param name="actor">Controlling Actor.</param>
        /// <param name="possession">Degree of possession affecting what verbs are collected.</param>
        /// <returns></returns>
        public virtual bool SetActor( Actor actor, PossessionType possession ) // XXX: Consider using type other than string
        {
            if( actor != null && this.actor != null ) { return false; }
            _actor = actor;

            CollectVerbs(actor, possession);

            return true;
        }
        public virtual void CollectVerbs( Actor actor, PossessionType possession )
        {
            if( _actor != null ) {
                foreach( Verb verb in verbs[possession] ) {
                    actor.AddVerb(verb);
                }
            }
        }

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

            data.text = name;

            data.SetSpan( data.text );
            data.span.Style = GameManager.instance.GetResource<Style>("Link");

            return data;
        }
        public virtual GameObjectData GetDescription( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            GameObjectData nameData = GetData("name");
            data.text = $"This is a {nameData.text}";

            data.SetSpan( 
                new Run("This is a "),
                nameData.span,
                new Run(".")
            );

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

        public virtual DescriptivePage DisplayDescriptivePage()
        {
            Point mousePosition = WaywardManager.instance.GetMousePosition();

            return GameManager.instance.DisplayDescriptivePage( mousePosition, this, new DescriptivePageSection[] {
                new GameObjectDescriptivePageSection()
            });
        }
    }

    public class GameObjectData
    {
        public string text;
        public Span span;

        public GameObjectData()
        {
            text = "--";
            span = new Span( new Run( text ));
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
    }
}
