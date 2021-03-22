using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls;
using WaywardEngine;

namespace AdventureCore
{
	public class ObservableText
    {
        public readonly string template;
        public readonly Tuple<GameObject, string>[] data;

        public ObservableText(string text, params Tuple<GameObject, string>[] textData)
        {
            template = text;
            data = textData;
        }

        public TextBlock Observed(PlayerActor observer)
        {
            WaywardTextParser.ParseDelegate[] spans = new WaywardTextParser.ParseDelegate[data.Length];
            for( int i = 0; i < data.Length; i++ ) {
                Tuple<GameObject, string> dat = new Tuple<GameObject, string>(data[i].Item1, data[i].Item2);
                spans[i] = () => { return observer.Observe(dat.Item1, dat.Item2).span; };
            }
            TextBlock block = WaywardTextParser.ParseAsBlock(template, spans);

            return block;
        }
    }
}
