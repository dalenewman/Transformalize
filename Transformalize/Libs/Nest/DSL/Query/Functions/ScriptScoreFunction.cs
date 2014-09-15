using System;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.DSL.Filter;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class ScriptScoreFunction<T> : FunctionScoreFunction<T> where T : class
	{
		[JsonProperty(PropertyName = "script_score")]
		internal ScriptFilterDescriptor _ScriptScore { get; set; }

		public ScriptScoreFunction(Action<ScriptFilterDescriptor> scriptSelector)
		{
			var descriptor = new ScriptFilterDescriptor();
			if (scriptSelector != null)
				scriptSelector(descriptor);

			this._ScriptScore = descriptor;
		}
	}
}