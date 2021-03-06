﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
    class GameObjectDescriptivePageSection : DescriptivePageSection
    {
        TextBlock descriptiveText;

        public GameObjectDescriptivePageSection()
            : base("DescriptiveGameObject")
        {
            descriptiveText = Utilities.FindNode<TextBlock>( element, "Description" );
        }

        public override void Clear()
        {
            descriptiveText.Inlines.Clear();
        }

        public override void DisplayContents()
        {
            string formatedDesc = page.target.description + ".";
            formatedDesc = char.ToUpper(formatedDesc[0]) + formatedDesc.Substring(1);
            descriptiveText.Inlines.Add(formatedDesc);
        }
    }
}
