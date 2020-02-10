using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaywardEngine
{
    public class InputManagerBase
    {
        public delegate bool InputDelegate( InputEventArgs e );
        public event InputDelegate inputReceived;

        public bool inputBusy = false;

        public virtual void ParseInput( string input )
        {
            if( input.Length <= 0 ) { return; }

            if( inputReceived != null ) {
                InputEventArgs e = new InputEventArgs(input);
                inputReceived.Invoke(e);

                if( !e.parsed ) {
                    // XXX: This message should be displayed in the inputBox
                    WaywardManager.instance.DisplayMessage("Command not found.");
                }
            }
        }

        public bool CheckCommands( Dictionary<string[], InputDelegate> commands, InputEventArgs e )
        {
            if( e.parsed ) { return true; }

            foreach( KeyValuePair<string[], InputDelegate> command in commands ) {
                foreach( string s in command.Key ) {
                    if( e.action == s ) {
                        if( command.Value(e) ) {
                            e.parsed = true;
                            return true;
                        }

                        break;
                    }
                }
            }

            return false;
        }
    }

    #region InputEventArgs

    public class InputEventArgs
    {
        public string input { get; private set; }
        public string[] words { get; private set; }
        public string action { get; private set; }
        public string[] parameters{ get; private set; }
        public bool parsed = false;

        public InputEventArgs( string i )
        {
            input = i;
            words = i.Split(' ');
            action = words[0].ToLower();
            parameters = new string[words.Length - 1];
            if( parameters.Length > 0 ) {
                Array.Copy(words, 1, parameters, 0, parameters.Length);
            }
        }
    }

    #endregion
}
