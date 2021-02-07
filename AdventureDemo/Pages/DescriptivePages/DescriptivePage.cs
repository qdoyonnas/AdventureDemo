using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    class DescriptivePage : ContentPage
    {
        GameObject _target;
        public GameObject target {
            get {
                return _target;
            }
        }

        PlayerActor _observer;
        public PlayerActor observer {
            get {
                return _observer;
            }
        }

        List<DescriptivePageSection> sections;

        public DescriptivePage( PlayerActor observer, GameObject target, DescriptivePageSection[] sections )
            : base()
        {
            this.sections = new List<DescriptivePageSection>();

            _observer = observer;
            _target = target;
            SetTitle( observer.Observe(_target, "name upper complete").text );

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

        public override void Clear() {}
        public override void Update()
        {
            SetTitle( observer.Observe(_target, "name upper complete").text );
            DisplayTarget();
        }
    }
}
