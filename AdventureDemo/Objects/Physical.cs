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

        protected List<AttachmentPoint> attachmentPoints;

        public Dictionary<Material, double> materials;
        protected double totalParts = 0;

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

            attachmentPoints = new List<AttachmentPoint>();

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

        public override List<Verb> CollectVerbs()
        {
            List<Verb> collectedVerbs = base.CollectVerbs();

            foreach( AttachmentPoint point in attachmentPoints ) {
                foreach( GameObject obj in point.GetAttached() ) {
                    collectedVerbs.AddRange( obj.CollectVerbs() );
                }
            }

            return collectedVerbs;
        }
        public override void CollectVerbs( Actor actor, PossessionType possession )
        {
            base.CollectVerbs(actor, possession);

            foreach( AttachmentPoint point in attachmentPoints ) {
                foreach( GameObject obj in point.GetAttached() ) {
                    obj.CollectVerbs( actor, possession );
                }
            }
        }

        public virtual GameObjectData GetDescriptiveWeight( string[] parameters )
        {
            GameObjectData data = new GameObjectData();
            data.text = $"{GetWeight().ToString()} pounds";

            data.SetSpan( data.text );

            return data;
        }
        public virtual GameObjectData GetDescriptiveVolume( string[] parameters )
        {
            GameObjectData data = new GameObjectData();

            data.text = $"{GetVolume().ToString()} L";
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

        public virtual double GetWeight()
        {
            return weight;
        }
        public virtual double GetVolume()
        {
            return volume;
        }

        public override List<DescriptivePageSection> DisplayDescriptivePage()
        {
            List<DescriptivePageSection> sections = base.DisplayDescriptivePage();

            sections.Add( new PhysicalDescriptivePageSection() );
            sections.Add( new PhysicalAttachmentDescriptivePageSection() );

            return sections;
        }
        
        public virtual AttachmentPoint[] GetAttachmentPoints()
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
        public virtual void AddAttachmentPoint( AttachmentPoint point )
        {
            attachmentPoints.Add(point);
        }
        public virtual void RemoveAttachmentPoint( AttachmentPoint point )
        {
            attachmentPoints.Remove(point);
        }

        public virtual void AddMaterial( Material material, double parts )
        {
            if( !materials.ContainsKey(material) ) {
                materials.Add( material, parts );
            } else {
                totalParts -= materials[material];
                materials[material] = parts;
            }

            totalParts += parts;
        }
        public virtual void RemoveMaterial( Material material )
        {
            if( !materials.ContainsKey(material) ) { return; }

            totalParts -= materials[material];
            materials.Remove(material);
        }
        public virtual double GetMaterialRatio( Material material, bool asPercent = false )
        {
            if( !materials.ContainsKey(material) ) { return 0; }

            double ratio = materials[material] / totalParts;

            if( asPercent ) {
                ratio = Math.Round(ratio * 100, 1);
            }

            return ratio;
        }
    }
}
