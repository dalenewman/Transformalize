using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Query
{

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(ReadAsTypeConverter<MatchAllQuery>))]
	public interface IMatchAllQuery : IQuery
	{
		[JsonProperty(PropertyName = "boost")]
		double? Boost { get; }

		[JsonProperty(PropertyName = "norm_field")]
		string NormField { get; }
	}

	public class MatchAllQuery : PlainQuery, IMatchAllQuery
	{
		public double? Boost { get; internal set; }

		public string NormField { get; internal set; }

		bool IQuery.IsConditionless { get { return false; } }

		protected override void WrapInContainer(IQueryContainer container)
		{
			container.MatchAllQuery = this;
		}
	}
}
