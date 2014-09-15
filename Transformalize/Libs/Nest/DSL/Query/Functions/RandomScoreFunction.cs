using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<RandomScoreFunction>))]
	public interface IRandomScoreFunction
	{
		[JsonProperty(PropertyName = "seed")]
		int? Seed { get; set; }
	}

	public class RandomScoreFunction : IRandomScoreFunction
	{
		int? IRandomScoreFunction.Seed { get; set; }

		public RandomScoreFunction(int seed)
		{
			((IRandomScoreFunction)this).Seed = seed;
		}

		public RandomScoreFunction()
		{
		}
	}
}