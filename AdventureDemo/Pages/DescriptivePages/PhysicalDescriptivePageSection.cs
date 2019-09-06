﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureDemo
{
    class PhysicalDescriptivePageSection : DescriptivePageSection
    {
        TextBlock weightText;
        TextBlock volumeText;

        public PhysicalDescriptivePageSection()
            : base("DescriptivePhysical")
        {
            weightText = Utilities.FindNode<TextBlock>( element, "Weight" );
            volumeText = Utilities.FindNode<TextBlock>( element, "Volume" );
        }

        public override void Clear()
        {
        }
        public override void DisplayContents()
        {
            weightText.Text = page.target.GetData("weight").text;
            volumeText.Text = page.target.GetData("volume").text;
        }
    }
}