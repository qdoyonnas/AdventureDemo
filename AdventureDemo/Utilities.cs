using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace AdventureDemo
{
    public enum CheckResult {
        INVALID,
        RESTRICTED,
        VALID
    }

    static class PhysicalUtilities
    {

        public static Physical FindParentPhysical( Physical obj )
        {
            if( obj.attachedTo != null ) {
                return FindParentPhysical(obj.attachedTo);
            }

            return obj;
        }
    }

    static class SearchComparator
    {
        public static bool CompareString( string value, string searchValue )
        {
            if( searchValue == "/any" ) { return true; }

            string[] valids = searchValue.Split('|');
            foreach( string valid in valids ) {
                string trimmedValid = valid.Trim();
                if( value == trimmedValid ) { return true; }
            }

            return false;
        }
        public static bool CompareNumber( double value, string searchValue )
        {
            if( searchValue == "/any" ) { return true; }

            string[] valids = searchValue.Split('|');
            foreach( string valid in valids ) {
                string trimmedValid = valid.Trim();
                Regex regex = new Regex(@"^(?<min>\d+(\.\d+)?)/s*:/s*(?<max>\d+(\.\d+)?)$");
                Match match = regex.Match(trimmedValid);
                if( match.Success ) {
                    double min = double.Parse(match.Groups["min"].Value);
                    double max = double.Parse(match.Groups["max"].Value);
                    if( value >= min && value <= max ) {
                        return true;
                    }
                } else {
                    if( value == double.Parse(trimmedValid) ) { return true; }
                }
            }

            return false;
        }
    }

    class ObservableText
    {
        public readonly string template;
        public readonly Tuple<GameObject, string>[] data;

        public ObservableText(string text, params Tuple<GameObject, string>[] textData)
        {
            template = text;
            data = textData;
        }

        public TextBlock Observe(Actor observer)
        {
            WaywardEngine.WaywardTextParser.ParseDelegate[] spans = new WaywardEngine.WaywardTextParser.ParseDelegate[data.Length];
            for( int i = 0; i < data.Length; i++ ) {
                Tuple<GameObject, string> dat = new Tuple<GameObject, string>(data[i].Item1, data[i].Item2);
                spans[i] = () => { return observer.Observe(dat.Item1, dat.Item2).span; };
            }
            TextBlock block = WaywardEngine.WaywardTextParser.ParseAsBlock(template, spans);

            return block;
        }
    }
}
