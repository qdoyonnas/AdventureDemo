using System;

namespace AdventureDemo
{
    abstract class VerbData : BasicData
    {
        public string type;

        public string label;
        public string[] validInputs;

        public VerbData() 
        {
            validInputs = new string[0];
        }
        public VerbData( VerbData data )
        {
            type = data.type;
            label = data.label;

            validInputs = new string[data.validInputs.Length];
            Array.Copy(data.validInputs, validInputs, validInputs.Length);
        }
    }
}
