using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using NCalc;

namespace AdventureDemo
{
    abstract class DynamicValue<T>
    {
        public enum Method
        {
            VALUE,
            RANGE,
            CHOICE,
            EXPRESSION
        }
        public Method method;

        public T[] choices;

        public string originalExpression;
        public Expression expression;
        public string[] parameters;

        public DynamicValue()
        {
            choices = new T[0];
        }
        public DynamicValue( DynamicValue<T> value )
        {
            method = value.method;
            choices = new T[value.choices.Length];
            Array.Copy(value.choices, choices, choices.Length);

            if( value.expression != null ) {
                originalExpression = value.originalExpression;
                expression = new Expression(value.originalExpression);
                parameters = new string[value.parameters.Length];
                Array.Copy(value.parameters, parameters, parameters.Length);
            }
        }

        protected delegate T ParseDelegate( string expression );

        protected bool EvaluateForChoice( string expression, ParseDelegate parse )
        {
            expression = expression.Trim();

            if( !Regex.IsMatch(expression, @"^\[[^\]]+\]$") ) { return false; }
            method = Method.CHOICE;

            Regex regex = new Regex(@"\s*([^,\[\]]+),*");
            MatchCollection matches = regex.Matches(expression);

            choices = new T[matches.Count];
            for( int i = 0; i < matches.Count; i++ ) {
                try {
                    choices[i] = parse.Invoke(matches[i].Groups[1].Value);
                } catch( Exception e ) {
                    throw new Exception($"DynamicValue could not parse '{matches[i].Groups[1].Value}' in EvaluateForChoice: {e}");
                }
            }

            return true;
        }
        protected bool EvaluateForRange( string expression, ParseDelegate parse )
        {
            expression = expression.Trim();

            Regex regex = new Regex(@"(?<min>[^:]):(?<max>.+)");
            Match match = regex.Match(expression);

            if( !match.Success ) { return false; }
            method = Method.RANGE;

            choices = new T[2];

            try {
                choices[0] = parse.Invoke(match.Groups["min"].Value);
            } catch( Exception e ) {
                throw new Exception($"DynamicValue could not parse '{match.Groups["min"].Value}' in EvaluateForRange: {e}");
            }

            try {
                choices[0] = parse.Invoke(match.Groups["max"].Value);
            } catch( Exception e ) {
                throw new Exception($"DynamicValue could not parse '{match.Groups["max"].Value}' in EvaluateForRange: {e}");
            }

            return true;
        }
        protected bool EvaluateForExpression( string expression )
        {
            expression = expression.Trim();

            Regex regex = new Regex(@"[^+-/*\^]+\s*[+-/*\^]\s*[^+-/*\^]+");

            if( !regex.IsMatch(expression) ) { return false; }

            method = Method.EXPRESSION;
            this.originalExpression = expression;
            this.expression = new Expression(expression);

            regex = new Regex(@"\[(\w+)\]");
            MatchCollection matches = regex.Matches(expression);
            
            parameters = new string[matches.Count];
            for( int i = 0; i < matches.Count; i++ ) {
                parameters[i] = matches[i].Groups[1].Value;
            }

            return true;
        }

        public abstract T GetValue( Dictionary<string, object> data );
        public abstract T GetValue();
    }
}
