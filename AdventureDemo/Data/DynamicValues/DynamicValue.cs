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

        public Expression expression;
        public string[] parameters;

        protected delegate T ParseDelegate( string expression );

        protected bool EvaluateForChoice( string expression, ParseDelegate parse )
        {
            expression = expression.Trim();

            Regex regex = new Regex(@"^\[(?:([^,\]]+),*)+\]$");
            Match match = regex.Match(expression);

            if( !match.Success ) { return false; }
            method = Method.CHOICE;

            choices = new T[match.Groups.Count-1];
            for( int i = 1; i < match.Groups.Count; i++ ) {
                try {
                    choices[i-1] = parse.Invoke(match.Groups[i].Value);
                } catch( Exception e ) {
                    throw new Exception($"DynamicValue could not parse '{match.Groups[i].Value}' in EvaluateForChoice: {e}");
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
    }
}
