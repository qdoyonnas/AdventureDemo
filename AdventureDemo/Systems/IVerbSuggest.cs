using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdventureDemo
{
    interface IVerbSuggest
    {
        bool DisplayVerb( Verb verb, FrameworkContentElement span );
        bool SetDefaultVerb( Verb verb, FrameworkContentElement span );
    }
}
