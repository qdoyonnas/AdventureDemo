using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace AdventureCore
{
    public enum CheckValue {
        INVALID,
        RESTRICTED,
        VALID
    }

    public class CheckResult
    {
        public CheckValue value = CheckValue.INVALID;
        public List<string> messages = new List<string>();

        public CheckResult()
        {
            value = CheckValue.INVALID;
            messages = new List<string>();
        }
        public CheckResult(CheckValue value)
        {
            this.value = value;
            messages = new List<string>();
        }
        public CheckResult(CheckValue value, params string[] messages)
        {
            this.value = value;
            this.messages = new List<string>(messages);
        }
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
