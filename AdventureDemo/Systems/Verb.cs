using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows;

namespace AdventureDemo
{
    abstract class Verb
    {
        protected string _displayLabel;
        public string displayLabel {
            get {
                return _displayLabel;
            }
        }
        readonly public GameObject self;

        public Verb( GameObject self )
        {
            this.self = self;
        }

        /// <summary>
        /// Returns a bool indicating whether this Verb's action can be performed
        /// based on the passed in data.
        /// </summary>
        /// <param name="data">Arbitrary key-value dictionary to be used for parameter passing.</param>
        /// <returns></returns>
        public abstract CheckResult Check( GameObject target );
        public abstract void Action( GameObject target );
    }

    public enum CheckResult {
        INVALID,
        RESTRICTED,
        VALID
    }
}
