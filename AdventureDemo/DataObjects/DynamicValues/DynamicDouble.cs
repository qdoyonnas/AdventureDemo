﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NCalc;

namespace AdventureCore
{
    [JsonConverter(typeof(DynamicDoubleConverter))]
    class DynamicDouble : DynamicValue<Double>
    {
        public DynamicDouble( string expression )
        {
            if( EvaluateForChoice(expression, double.Parse) ) { return; }

            if( EvaluateForRange(expression, double.Parse) ) { return; }

            if( EvaluateForExpression(expression) ) { return; }

            // Is VALUE
            method = Method.VALUE;
            try {
                choices = new double[] { double.Parse(expression) };
            } catch( Exception e ) {
                throw new Exception($"DynamicDouble could not parse '{expression}' as value: {e}");
            }
        }
        public DynamicDouble( DynamicDouble value )
            : base(value) { }

        private double EvaluateExpression( Dictionary<string, object> data )
        {
            try {
                foreach( string param in parameters ) {
                    expression.Parameters[param] = data[param];
                }
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: DynamicDouble did not evaluate properly. Could not assign parameters: {e}");
                return 0;
            }

            return (double)expression.Evaluate();
        }

        public override double GetValue()
        {
            return GetValue(new Dictionary<string, object>());
        }
        public override double GetValue( Dictionary<string, object> data )
        {
            switch( method ) {
                case Method.VALUE:
                    return choices[0];
                case Method.CHOICE:
                    int i = GameManager.instance.world.random.Next(0, choices.Length);
                    return choices[i];
                case Method.RANGE:
                    double v = GameManager.instance.world.random.NextDouble();
                    return choices[0] + (v * (choices[1] - choices[1]));
                case Method.EXPRESSION:
                    return EvaluateExpression(data);
            }

            Console.WriteLine("ERROR: DynamicDouble did not evaluate properly.");
            return 0;
        }
    }

    class DynamicDoubleConverter : JsonConverter
    {
        public override bool CanConvert( Type objectType )
        {
            return typeof(DynamicDouble).IsAssignableFrom(objectType);
        }

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            return new DynamicDouble(reader.Value.ToString());
        }

        public override bool CanWrite => false;
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            throw new NotImplementedException();
        }
    }
}
