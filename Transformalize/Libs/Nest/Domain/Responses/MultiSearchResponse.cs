using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.Response;
using Transformalize.Libs.Nest.Extensions;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	[JsonObject]
	public class MultiSearchResponse : BaseResponse, IMultiSearchResponse
	{
		public MultiSearchResponse()
		{
			this._Responses = new Dictionary<string, object>();
		}

		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]	
		internal IDictionary<string, object> _Responses { get; set; }

		public int TotalResponses { get { return this._Responses.HasAny() ? this._Responses.Count() : 0 ; } }

		public IEnumerable<SearchResponse<T>> GetResponses<T>() where T : class
		{
			foreach (var r in this._Responses.Values.OfType<IResponseWithRequestInformation>())
				r.RequestInformation = this.ConnectionStatus;
			return this._Responses.Values.OfType<SearchResponse<T>>();
		}
		public SearchResponse<T> GetResponse<T>(string name) where T : class
		{
			object response = null;
			this._Responses.TryGetValue(name, out response);
			var r = response as IResponseWithRequestInformation;
			if (r != null)
				r.RequestInformation = this.ConnectionStatus;
			return response as SearchResponse<T>;
		}
	}
}
