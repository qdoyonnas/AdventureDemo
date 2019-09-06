using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdventureDemo
{
    abstract class DescriptivePageSection
    {
        protected FrameworkElement element;
        protected DescriptivePage page;

        public DescriptivePageSection( string key )
        {
            element = GameManager.instance.GetResource<FrameworkElement>(key);
        }

        public void AssignPage( DescriptivePage page )
        {
            this.page = page;

            page.AddContent(element);
        }

        public abstract void DisplayContents();
        public abstract void Clear();
    }
}
