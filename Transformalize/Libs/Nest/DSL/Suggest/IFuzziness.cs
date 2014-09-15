using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Suggest
{
	[JsonConverter(typeof(FuzzinessConverter))]
	public interface IFuzziness
	{
		bool Auto { get;  }
		int? EditDistance { get;  }
		double? Ratio { get;  }
	}
}