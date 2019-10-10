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
        protected string displayLabel;
        protected GameObject self;

        public Verb( GameObject self )
        {
            this.self = self;
        }

        public void Display( FrameworkElement span, GameObject obj )
        {
            WaywardEngine.ContextMenuHelper.AddContextMenuItem(span, $"{self.GetData("name").text} - {displayLabel}", delegate { this.Action(obj); });
        }
        public void Display( FrameworkContentElement span, GameObject obj )
        {
            WaywardEngine.ContextMenuHelper.AddContextMenuItem(span, $"{self.GetData("name").text} - {displayLabel}", delegate { this.Action(obj); });
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
