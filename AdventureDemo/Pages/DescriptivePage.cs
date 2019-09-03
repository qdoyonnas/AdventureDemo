using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class DescriptivePage : ContentPage
    {
        GameObject _target;
        public GameObject target {
            get {
                return _target;
            }
            set {
                _target = value;
                Clear();
                SetTitle( _target.GetData("name").text );
            }
        }

        List<DescriptivePageSection> sections;

        public DescriptivePage( GameObject target, DescriptivePageSection[] sections )
            : base()
        {
            this.sections = new List<DescriptivePageSection>();

            this.target = target;

            foreach( DescriptivePageSection section in sections ) {
                AddSection(section, false);
            }

            Update();
        }

        public void AddSection( DescriptivePageSection section, bool performUpdate = true )
        {
            section.AssignPage(this);
            sections.Add(section);

            if( performUpdate ) {
                Update();
            }
        }

        public void DisplayTarget()
        {
            foreach( DescriptivePageSection section in sections ) {
                section.Clear();
                section.DisplayContents();
            }
        }
        
        public override void Update()
        {
            DisplayTarget();
        }
    }
}
