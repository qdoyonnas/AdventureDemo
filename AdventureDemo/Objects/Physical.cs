using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class Physical : GameObject
    {
        #region Fields

        protected double volume;
        protected virtual double weight {
            get {
                double totalWeight = 0;
                foreach( KeyValuePair<Material, double> material in _materials ) {
                    totalWeight += material.Key.GetWeight( volume * GetMaterialRatio(material.Key) );
                }

                return totalWeight;
            }
        }

        protected List<PhysicalAttachmentPoint> attachmentPoints;
        protected Physical _attachedTo;
        public Physical attachedTo {
            get {
                return _attachedTo;
            }
        }

        // XXX: Prevents large entities having parts in different containers
        //      example: reaching into a box.
        public override AttachmentPoint container {
            get {
                if( attachedTo != null ) {
                    return attachedTo.container;
                }

                return _container;
            }
        }

        public override Actor actor {
            get {
                if( attachedTo != null ) {
                    return attachedTo.actor;
                }

                return _actor;
            }
        }

        protected Dictionary<Material, double> _materials;
        protected virtual Dictionary<Material, double> materials {
            get {
                return _materials;
            }
        }
        protected double _totalParts = 0;
        public virtual double totalParts {
             get {
                return _totalParts;
            }
        }

        #endregion

        #region Constructors

        public Physical()
            : base()
        {
            Construct();
        }
        public Physical( Dictionary<string, object> data )
            : base(data)
        {
            Construct();

            if( data.ContainsKey("volume") ) { volume = (double)data["volume"]; }
            if( data.ContainsKey("materials") ) { 
                foreach( KeyValuePair<Material, double> mat in data["materials"] as KeyValuePair<Material, double>[] ) {
                    AddMaterial(mat.Key, mat.Value);
                }
            }
        }
        public Physical( double volume, params KeyValuePair<Material, double>[] mats )
            : base()
        {
            Construct();

            this.volume = volume;
            foreach( KeyValuePair<Material, double> mat in mats ) {
                AddMaterial(mat.Key, mat.Value);
            }
        }
        private void Construct()
        {
            tags.Add("physical");

            attachmentPoints = new List<PhysicalAttachmentPoint>();

            objectData["weight"] = GetDescriptiveWeight;
            objectData["volume"] = GetDescriptiveVolume;
            objectData["materials"] = GetDescriptiveMaterials;

            relevantData.Add(GetDescriptiveVolume);
            relevantData.Add(GetDescriptiveWeight);
            relevantData.Add(GetDescriptiveMaterials);

            this.volume = 0;
            _materials = new Dictionary<Material, double>();
        }

        #endregion

        #region Events

        public override void OnAction(Dictionary<string, object> data)
        {
            if( attachedTo == null ) {
                base.OnAction(data);
            } else {
                attachedTo.OnAction(data);
            }
        }

        #endregion

        #region Verb Methods

        public override List<Verb> CollectVerbs()
        {
            List<Verb> collectedVerbs = base.CollectVerbs();

            foreach( AttachmentPoint point in attachmentPoints ) {
                foreach( GameObject obj in point.GetAttached() ) {
                    collectedVerbs.AddRange( obj.CollectVerbs(PossessionType.CONTENT) );
                }
            }

            return collectedVerbs;
        }
        public override void CollectVerbs( Actor actor, PossessionType possession )
        {
            base.CollectVerbs(actor, possession);

            foreach( AttachmentPoint point in attachmentPoints ) {
                foreach( GameObject obj in point.GetAttached() ) {
                    obj.CollectVerbs( actor, PossessionType.CONTENT );
                }
            }
        }
        // TODO: CollectVerbs( PossessionType possession );

        #endregion

        #region Physical Methods

        public virtual double GetWeight( bool total = true )
        {
            double totalWeight = weight;

            if( total ) {
                foreach( PhysicalAttachmentPoint point in attachmentPoints ) {
                    foreach( Physical obj in point.GetAttachedAsPhysical() ) {
                        totalWeight += obj.GetWeight();
                    }
                }
            }

            return totalWeight;
        }
        public virtual double GetVolume( bool total = true )
        {
            double totalVolume = volume;

            if( total ) {
                foreach( PhysicalAttachmentPoint point in attachmentPoints ) {
                    if( point.isExternal ) {
                        foreach( Physical obj in point.GetAttachedAsPhysical() ) {
                            totalVolume += obj.GetVolume();
                        }
                    }
                }
            }

            return totalVolume;
        }

        #endregion

        #region Attachment Point Methods

        public virtual PhysicalAttachmentPoint[] GetAttachmentPoints()
        {
            return attachmentPoints.ToArray();
        }
        public virtual int GetAttachmentCount()
        {
            return attachmentPoints.Count;
        }

        public virtual void AddAttachmentPoint( Dictionary<string, object> data )
        {
            if( !data.ContainsKey("parent") ) {
                data.Add("parent", this);
            }

            attachmentPoints.Add(new PhysicalAttachmentPoint(data) );
        }
        public virtual void AddAttachmentPoint( PhysicalAttachmentPoint point )
        {
            attachmentPoints.Add(point);
        }
        public virtual void RemoveAttachmentPoint( PhysicalAttachmentPoint point )
        {
            attachmentPoints.Remove(point);
        }

        public virtual bool Contains( Physical obj )
        {
            if( this == obj ) { return true; }

            foreach( PhysicalAttachmentPoint point in attachmentPoints ) {
                if( point.Contains(obj) ) {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Material Methods 

        public virtual void AddMaterial( Material material, double parts )
        {
            if( !_materials.ContainsKey(material) ) {
                _materials.Add( material, parts );
            } else {
                _totalParts -= _materials[material];
                _materials[material] = parts;
            }

            _totalParts += parts;
        }
        public virtual void RemoveMaterial( Material material )
        {
            if( !_materials.ContainsKey(material) ) { return; }

            _totalParts -= _materials[material];
            _materials.Remove(material);
        }

        public virtual List<Material> GetMaterials()
        {
            return new List<Material>(_materials.Keys);
        }
        public virtual double GetMaterialParts( Material material )
        {
            double totalParts = 0;

            foreach( KeyValuePair<Material, double> mat in materials ) {
                if( mat.Key == material ) {
                    totalParts += mat.Value;
                }
            }

            return totalParts;
        }
        public virtual double GetMaterialRatio( Material material, bool asPercent = false )
        {
            if( !_materials.ContainsKey(material) ) { return 0; }

            double ratio = _materials[material] / _totalParts;

            if( asPercent ) {
                ratio = Math.Round(ratio * 100, 1);
            }

            return ratio;
        }

        #endregion

        #region Descriptive Methods

        public override List<DescriptivePageSection> DisplayDescriptivePage()
        {
            List<DescriptivePageSection> sections = base.DisplayDescriptivePage();

            sections.Add( new PhysicalDescriptivePageSection() );
            sections.Add( new PhysicalAttachmentDescriptivePageSection() );

            return sections;
        }

        public override GameObjectData GetName(params string[] parameters)
        {
            if( parameters.Contains("top") && attachedTo != null ) {
                return attachedTo.GetName(parameters);
            } else {
                return base.GetName(parameters);
            }
        }

        public virtual GameObjectData GetDescriptiveWeight( string[] parameters )
        {
            bool getTotal = !(parameters.Length > 0 && parameters[0] == "partial");

            GameObjectData data = new GameObjectData();
            data.text = $"{GetWeight(getTotal).ToString()} lbs";

            data.SetSpan( data.text );

            return data;
        }
        public virtual GameObjectData GetDescriptiveVolume( string[] parameters )
        {
            bool getTotal = !(parameters.Length > 0 && parameters[0] == "partial");

            GameObjectData data = new GameObjectData();

            data.text = $"{GetVolume(getTotal).ToString()} L";
            data.SetSpan( data.text );

            return data;
        }
        public virtual GameObjectData GetDescriptiveMaterials( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            int index = -1;
            if( parameters.Length > 0 ) {
                if( !int.TryParse(parameters[0], out index)
                    || index < 0
                    || index >= _materials.Count ) 
                {
                    index = -1;
                }
            }

            KeyValuePair<Material, double>[] mats = materials.ToArray();
            if( index != -1 ) {
                GetMat( data, mats[index] );
            } else {
                for(int i = 0; i < mats.Length; i++ ) {
                    GetMat( data, mats[i] );
                }
            }

            return data;
        }
        protected virtual void GetMat( GameObjectData data, KeyValuePair<Material, double> mat )
        {
            GameObjectData matData = mat.Key.GetData("name upper");
            data.text = $"{matData.text} : {GetMaterialRatio(mat.Key, true)}%";

            data.AddSpan( WaywardTextParser.Parse(@"[0] : [1]    ", 
                () => { return matData.span; },
                () => { return WaywardTextParser.Parse($@"{GetMaterialRatio(mat.Key, true)}%"); }
            ) );
        }

        #endregion

        #region Helper Methods

        public virtual bool SetAttachedTo( Physical obj )
        {
            _attachedTo = obj;
            return true;
        }

        public override List<GameObject> GetChildObjects()
        {
            List<GameObject> children = base.GetChildObjects();

            foreach( PhysicalAttachmentPoint point in attachmentPoints ) {
                children.AddRange(point.GetAttached());
            }

            return children;
        }

        public override bool MatchesSearch(Dictionary<string, string> properties)
        {
            foreach( KeyValuePair<string, string> property in properties.ToArray() ) {
                properties.Remove(property.Key);
                switch( property.Key ) {
                    case "volume":
                        if( !SearchComparator.CompareNumber(volume, property.Value) ) {
                            return false;
                        }
                        break;
                    case "weight":
                        if( !SearchComparator.CompareNumber(weight, property.Value) ) {
                            return false;
                        }
                        break;
                    case "totalWeight":
                        if( !SearchComparator.CompareNumber(GetWeight(true), property.Value) ) {
                            return false;
                        }
                        break;
                    // XXX: Add Materials, Attachment Points
                    default:
                        properties[property.Key] = property.Value;
                        break;
                }
            }

            return base.MatchesSearch(properties);
        }

        #endregion
    }
}
