using System.Collections.Generic;
using Transformalize.Libs.Nest.Domain.Alias;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IGetAliasesResponse : IResponse
	{
		IDictionary<string, IList<AliasDefinition>> Indices { get; }
	}

	public class GetAliasesResponse : BaseResponse, IGetAliasesResponse
	{
		public GetAliasesResponse()
		{
			this.IsValid = true;
			this.Indices = new Dictionary<string, IList<AliasDefinition>>();
		}

		public IDictionary<string, IList<AliasDefinition>> Indices { get; internal set; }
	}
}