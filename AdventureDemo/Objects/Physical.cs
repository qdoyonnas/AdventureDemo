using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using WaywardEngine;

namespace AdventureDemo
{
    class Physical : GameObject
    {
        #region Fields

        protected double volume;
        protected double weight {
            get {
                double totalWeight = 0;
                foreach( KeyValuePair<Material, double> material in materials ) {
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

        public override AttachmentPoint container {
            get {
                if( attachedTo != null ) {
                    return attachedTo.container;
                }

                return _container;
            }
        }

        protected Dictionary<Material, double> materials;
        protected double _totalParts = 0;
        public double totalParts {
             get {
                return _totalParts;
            }
        }

        #endregion

        #region Constructors

        public Physical( Dictionary<string, object> data )
            : base(data)
        {
            Construct(
                data.ContainsKey("volume") ? (double)data["volume"] : 0,
                data.ContainsKey("materials") ? data["materials"] as KeyValuePair<Material, double>[] : new KeyValuePair<Material, double>[0]
            );
        }
        public Physical( string name )
            : base(name)
        {
            Construct(0, new KeyValuePair<Material, double>[0]);
        }
        public Physical( string name, double volume, params KeyValuePair<Material, double>[] mats )
            : base(name)
        {
            Construct(volume, mats);
        }
        private void Construct( double volume, KeyValuePair<Material, double>[] mats )
        {
            this.description = "a solid object";

            attachmentPoints = new List<PhysicalAttachmentPoint>();

            objectData["weight"] = GetDescriptiveWeight;
            objectData["volume"] = GetDescriptiveVolume;
            objectData["materials"] = GetDescriptiveMaterials;

            relevantData.Add(GetDescriptiveVolume);
            relevantData.Add(GetDescriptiveWeight);
            relevantData.Add(GetDescriptiveMaterials);

            this.volume = volume;
            materials = new Dictionary<Material, double>();
            foreach( KeyValuePair<Material, double> mat in mats ) {
                AddMaterial( mat.Key, mat.Value );
            }
        }

        public virtual bool SetAttachedTo( Physical obj )
        {
            _attachedTo = obj;
            return true;
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
            if( !materials.ContainsKey(material) ) {
                materials.Add( material, parts );
            } else {
                _totalParts -= materials[material];
                materials[material] = parts;
            }

            _totalParts += parts;
        }
        public virtual void RemoveMaterial( Material material )
        {
            if( !materials.ContainsKey(material) ) { return; }

            _totalParts -= materials[material];
            materials.Remove(material);
        }
        public virtual List<Material> GetMaterials()
        {
            return new List<Material>(materials.Keys);
        }
        public virtual double GetMaterialParts( Material material )
        {
            if( !materials.ContainsKey(material) ) { return 0; }

            return materials[material];
        }
        public virtual double GetMaterialRatio( Material material, bool asPercent = false )
        {
            if( !materials.ContainsKey(material) ) { return 0; }

            double ratio = materials[material] / _totalParts;

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
                    || index >= materials.Count ) 
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
    }
}
