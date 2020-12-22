using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NCalc;

namespace AdventureCore
{
    [JsonConverter(typeof(DynamicStringConverter))]
    class DynamicString : DynamicValue<string>
    {
        public DynamicString( string expression )
        {
            if( EvaluateForChoice(expression, (s) => { return s.Trim(); }) ) { return; }

            // No range with strings XXX: yet
            
            if( EvaluateForExpression(expression) ) { return; }

            // Is VALUE
            method = Method.VALUE;
            choices = new string[] { expression };
        }
        public DynamicString( DynamicString str )
            : base(str) { }

        private string EvaluateExpression( Dictionary<string, object> data )
        {
            try {
                foreach( string param in parameters ) {
                    expression.Parameters[param] = data[param];
                }
            } catch( Exception e ) {
                Console.WriteLine($"ERROR: DynamicString did not evaluate properly. Could not assign parameters: {e}");
                return string.Empty;
            }

            return (string)expression.Evaluate();
        }

        public override string GetValue()
        {
            return GetValue(new Dictionary<string, object>());
        }
        public override string GetValue( Dictionary<string, object> data )
        {
            switch( method ) {
                case Method.VALUE:
                    return choices[0];
                case Method.CHOICE:
                    int i = GameManager.instance.world.random.Next(0, choices.Length);
                    return choices[i];
                case Method.EXPRESSION:
                    return EvaluateExpression(data);

            }

            Console.WriteLine("ERROR: DynamicString did not evaluate properly.");
            return string.Empty;
        }
    }

    class DynamicStringConverter : JsonConverter
    {
        public override bool CanConvert( Type objectType )
        {
            return typeof(DynamicString).IsAssignableFrom(objectType);
        }

        public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        {
            return new DynamicString((string)reader.Value);
        }

        // XXX: Do this later
        public override bool CanWrite => false;
        public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        {
            throw new NotImplementedException();
        }
    }
}
