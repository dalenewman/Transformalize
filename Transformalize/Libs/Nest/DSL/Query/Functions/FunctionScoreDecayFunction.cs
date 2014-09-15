using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class FunctionScoreDecayFunction<T> : FunctionScoreFunction<T>
		where T : class
	{
	}
}