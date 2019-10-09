using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureDemo
{
    abstract class Verb
    {
        protected GameObject self;
        protected Actor actor;

        public Verb( GameObject self )
        {
            this.self = self;
            this.actor = this.self.actor;
        }

        /// <summary>
        /// Returns a bool indicating whether this Verb's action can be performed
        /// based on the passed in data.
        /// </summary>
        /// <param name="data">Arbitrary key-value dictionary to be used for parameter passing.</param>
        /// <returns></returns>
        public abstract bool Check( GameObject target );
        public abstract void Action( GameObject target );
    }
}
