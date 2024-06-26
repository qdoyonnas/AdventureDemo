﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
    // XXX: I don't love that the Amalgam mainly circumvents the implementation of Physical
    class PhysicalAmalgam : Physical
    {
        #region Fields

        protected override double weight {
            get {
                return 0;
            }
        }

        List<Physical> parts = new List<Physical>();

        protected override Dictionary<Material, double> materials {
            get {
                Dictionary<Material, double> allMats = new Dictionary<Material, double>();

                List<Material> mats = GetMaterials();
                foreach( Material mat in mats ) {
                    allMats[mat] = GetMaterialParts(mat);
                }

                return allMats;
            }
        }

        public override double totalParts {
            get {
                double total = 0;

                foreach( Physical part in parts ) {
                    total += part.totalParts;
                }

                return total;
            }
        }

        #endregion

        #region Constructors

        public PhysicalAmalgam( Dictionary<string, object> data )
            : base(data)
        {
            Construct();
        }
        public PhysicalAmalgam()
            : base()
        {
            Construct();
        }
        void Construct()
        {
            tags.Add("amalgam");

            volume = 0;
        }

        public override bool SetContainer( AttachmentPoint newContainer )
        {
            bool result = base.SetContainer(newContainer);
            if( !result ) { return false; }

            foreach( Physical part in parts ) {
                part.SetContainer(newContainer);
            }

            return true;
        }

        #endregion

        #region Verb Methods

        /// <summary>
        /// Amalgams cannot have their own verbs. Add verbs to the amalgam's parts instead.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="parts"></param>
        public override void AddVerb( PossessionType possession, Verb verb )
        {
            return;
        }

        public override List<Verb> CollectVerbs()
        {
            List<Verb> collectedVerbs = new List<Verb>();

            foreach( Physical part in parts ) {
                collectedVerbs.AddRange(part.CollectVerbs());
            }

            return collectedVerbs;
        }
        public override void CollectVerbs( Actor actor, PossessionType possession )
        {
            foreach( Physical part in parts ) {
                part.CollectVerbs(actor, possession);
            }
        }

        #endregion

        #region Physical Methods

        public bool AddPart( Physical part )
        {
            if( part.attachPoint != null ) {
                if( !part.attachPoint.Remove(part) ) {
                    return false;
                }
            }

            parts.Add(part);
            part.SetAttachedTo(this);
            part.SetContainer(attachPoint);

            return true;
        }
        public void RemovePart( Physical part )
        {
            parts.Remove(part);
            part.SetAttachedTo(null);
        }
        public Physical[] GetParts()
        {
            return parts.ToArray();
        }
        public int GetPartsCount()
        {
            return parts.Count;
        }

        public override double GetWeight( bool total = true )
        {
            double totalWeight = base.GetWeight(total);

            foreach( Physical obj in parts ) {
                totalWeight += obj.GetWeight();
            }

            return totalWeight;
        }
        public override double GetVolume( bool total = true )
        {
            double totalVolume = base.GetVolume(total);

            foreach( Physical obj in parts ) {
                totalVolume += obj.GetVolume();
            }

            return totalVolume;
        }

        #endregion

        #region Attachment Point Methods

        public override PhysicalAttachmentPoint[] GetAttachmentPoints()
        {
            List<PhysicalAttachmentPoint> points = new List<PhysicalAttachmentPoint>();

            foreach( Physical part in parts ) {
                points.AddRange( part.GetAttachmentPoints() );
            }

            return points.ToArray();
        }
        public override int GetAttachmentCount()
        {
            int total = 0;

            foreach( Physical part in parts ) {
                total += part.GetAttachmentCount();
            }

            return total;
        }
        /// <summary>
        /// Amalgams cannot have their own attachment points. Add points to the amalgam's parts instead.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="parts"></param>
        public override void AddAttachmentPoint( Dictionary<string, object> data )
        {
            return;
        }
        /// <summary>
        /// Amalgams cannot have their own attachment points. Add points to the amalgam's parts instead.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="parts"></param>
        public override void AddAttachmentPoint( PhysicalAttachmentPoint point )
        {
            return;
        }
        /// <summary>
        /// Amalgams cannot have their own attachment points. Add points to the amalgam's parts instead.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="parts"></param>
        public override void RemoveAttachmentPoint( PhysicalAttachmentPoint point )
        {
            return;
        }
        public override bool Contains( Physical obj )
        {
            foreach( Physical part in parts ) {
                if( part.Contains(obj) ) {
                    return true;
                }
            }
            
            return false;
        }

        #endregion

        #region Material Methods

        /// <summary>
        /// Amalgams cannot have their own materials. Add materials to the amalgam's parts instead.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="parts"></param>
        public override void AddMaterial( Material material, double parts )
        {
            return;
        }
        /// <summary>
        /// Amalgams cannot have their own materials. Add materials to the amalgam's parts instead.
        /// </summary>
        /// <param name="material"></param>
        /// <param name="parts"></param>
        public override void RemoveMaterial( Material material )
        {
            return;
        }
        public override List<Material> GetMaterials()
        {
            List<Material> mats = new List<Material>();

            foreach( Physical part in parts ) {
                foreach( Material partMat in part.GetMaterials() ) {
                    if( mats.Count == 0 ) {
                        mats.Add(partMat);
                    } else {
                        bool add = true;
                        foreach( Material mat in mats ) {
                            if( mat == partMat ) { 
                                add = false;
                                break;
                            }
                        }
                        if( add ) { mats.Add(partMat); }
                    }
                }
            }

            return mats;
        }
        public override double GetMaterialParts( Material material )
        {
            // XXX: This needs work to take in to account relative volume of different parts

            double materialParts = 0;

            foreach( Physical part in parts ) {
                materialParts += part.GetMaterialParts(material);
            }

            return materialParts;
        }
        public override double GetMaterialRatio( Material material, bool asPercent = false )
        {
            double materialParts = 0;

            foreach( Physical part in parts ) {
                materialParts += part.GetMaterialParts(material);
            }

            double ratio = materialParts / totalParts;

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

            sections.Add( new PhysicalAmalgamDescriptivePageSection() );

            return sections;
        }

        #endregion

        #region Helper Methods

        public override List<GameObject> GetChildObjects()
        {
            List<GameObject> children = base.GetChildObjects();

            children.AddRange(parts);

            return children;
        }

        public override bool MatchesSearch(Dictionary<string, string> properties)
        {
            foreach( KeyValuePair<string, string> property in properties.ToArray() ) {
                properties.Remove(property.Key);
                switch( property.Key ) {
                    case "partCount":
                        if( !SearchComparator.CompareNumber(GetPartsCount(), property.Value) ) {
                            return false;
                        }
                        break;
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
