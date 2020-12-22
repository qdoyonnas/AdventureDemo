using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdventureCore
{
    abstract class DescriptivePageSection
    {
        protected FrameworkElement element;
        protected DescriptivePage page;
        protected Actor observer;

        public DescriptivePageSection( string key )
        {
            element = GameManager.instance.GetResource<FrameworkElement>(key);
        }

        public virtual void AssignPage( DescriptivePage page )
        {
            this.page = page;
            observer = this.page.observer;

            page.AddContent(element);
        }

        public abstract void DisplayContents();
        public abstract void Clear();
    }
}
