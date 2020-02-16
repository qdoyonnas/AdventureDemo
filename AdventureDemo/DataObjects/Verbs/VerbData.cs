using System;

namespace AdventureDemo
{
    abstract class VerbData : BasicData
    {
        public string type;

        public string displayLabel;
        public string[] validInputs;

        public VerbData() 
        {
            validInputs = new string[0];
        }
        public VerbData( VerbData data )
        {
            type = data.type;
            displayLabel = data.displayLabel;

            validInputs = new string[data.validInputs.Length];
            Array.Copy(data.validInputs, validInputs, validInputs.Length);
        }
    }
}
