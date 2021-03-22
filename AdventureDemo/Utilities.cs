using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace AdventureCore
{
    public enum CheckResult {
        INVALID,
        RESTRICTED,
        VALID
    }

    public static class PhysicalUtilities
    {

        public static Physical FindParentPhysical( Physical obj )
        {
            if( obj.attachedTo != null ) {
                return FindParentPhysical(obj.attachedTo);
            }

            return obj;
        }
    }

    public static class SearchComparator
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
}
