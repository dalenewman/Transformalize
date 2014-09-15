using System.Collections.Generic;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IMultiSearchResponse : IResponse
	{
		IEnumerable<SearchResponse<T>> GetResponses<T>() where T : class;
		SearchResponse<T> GetResponse<T>(string name) where T : class;
		int TotalResponses { get; }
	}
}