using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureCore
{
	class ScriptData
	{
		public string id = String.Empty;
		public string code = String.Empty;

		public ScriptData() { }
		public ScriptData( ScriptData data )
		{
			id = data.id;
			code = data.code;
		}
	}
}
