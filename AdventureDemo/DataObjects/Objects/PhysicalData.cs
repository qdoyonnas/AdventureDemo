﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    class PhysicalData : ObjectData
    {
        public DynamicDouble volume = new DynamicDouble("0");
        public MaterialReference[] materials;

        public override Dictionary<string, object> GenerateData()
        {
            Dictionary<string, object> data = base.GenerateData();

            if( materials != null && materials.Length > 0 ) {
                KeyValuePair<Material, double>[] mats = new KeyValuePair<Material, double>[materials.Length];
                for( int i = 0; i < materials.Length; i++ ) {
                    mats[i] = materials[i].GetValue(mats);
                }
                data["materials"] = mats;
            }

            double v = volume.GetValue(data);
            if( v < 0 ) { v = double.PositiveInfinity; }
            data["volume"] = v;

            return data;
        }
        public override object Create()
        {
            Physical physical = null;

            try {
                physical = new Physical(GenerateData());
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: Could not instantiate Physical from PhysicalData: {e}");
            }

            return physical;
        }
    }
}