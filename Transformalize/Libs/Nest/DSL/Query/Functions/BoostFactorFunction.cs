using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class BoostFactorFunction<T> : FunctionScoreFunction<T> where T : class
	{
		[JsonProperty(PropertyName = "boost_factor")]
		internal double _BoostFactor { get; set; }

		public BoostFactorFunction(double boostFactor)
		{
			_BoostFactor = boostFactor;
		}
	}
}