using System;
using System.IO;
using System.Threading.Tasks;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Elasticsearch.Net.Domain.Response;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest
{
    public partial class ElasticClient
	{
		/// <inheritdoc />
		public IMultiSearchResponse MultiSearch(Func<MultiSearchDescriptor, MultiSearchDescriptor> multiSearchSelector)
		{
			return this.Dispatch<MultiSearchDescriptor, MultiSearchRequestParameters, MultiSearchResponse>(
				multiSearchSelector(new MultiSearchDescriptor()),
				(p, d) =>
				{
					var converter = CreateMultiSearchConverter(d);
					var json = Serializer.SerializeMultiSearch(d);
					var creator = new Func<IElasticsearchResponse, Stream, MultiSearchResponse>((r, s) => this.DeserializeMultiSearchHit(r, s, converter));
					return this.RawDispatch.MsearchDispatch<MultiSearchResponse>(p.DeserializationState(creator), json);
				}
			);
		}

		/// <inheritdoc />
		public IMultiSearchResponse MultiSearch(IMultiSearchRequest multiSearchRequest)
		{
			return this.Dispatch<IMultiSearchRequest, MultiSearchRequestParameters, MultiSearchResponse>(
				multiSearchRequest,
				(p, d) =>
				{
					var converter = CreateMultiSearchConverter(d);
					var json = Serializer.SerializeMultiSearch(d);
					var creator = new Func<IElasticsearchResponse, Stream, MultiSearchResponse>((r, s) => this.DeserializeMultiSearchHit(r, s, converter));
					return this.RawDispatch.MsearchDispatch<MultiSearchResponse>(p.DeserializationState(creator), json);
				}
			);
		}

		/// <inheritdoc />
		public Task<IMultiSearchResponse> MultiSearchAsync(Func<MultiSearchDescriptor, MultiSearchDescriptor> multiSearchSelector) {
			return this.DispatchAsync<MultiSearchDescriptor, MultiSearchRequestParameters, MultiSearchResponse, IMultiSearchResponse>(
				multiSearchSelector(new MultiSearchDescriptor()),
				(p, d) =>
				{
					var converter = CreateMultiSearchConverter(d);
					var json = Serializer.SerializeMultiSearch(d);
					var creator = new Func<IElasticsearchResponse, Stream, MultiSearchResponse>((r, s) => this.DeserializeMultiSearchHit(r, s, converter));
					return this.RawDispatch.MsearchDispatchAsync<MultiSearchResponse>(p.DeserializationState(creator), json);
				}
			);
		}

		/// <inheritdoc />
		public Task<IMultiSearchResponse> MultiSearchAsync(IMultiSearchRequest multiSearchRequest) 
		{
			return this.DispatchAsync<IMultiSearchRequest, MultiSearchRequestParameters, MultiSearchResponse, IMultiSearchResponse>(
				multiSearchRequest,
				(p, d) =>
				{
					var converter = CreateMultiSearchConverter(d);
					var json = Serializer.SerializeMultiSearch(d);
					var creator = new Func<IElasticsearchResponse, Stream, MultiSearchResponse>((r, s) => this.DeserializeMultiSearchHit(r, s, converter));
					return this.RawDispatch.MsearchDispatchAsync<MultiSearchResponse>(p.DeserializationState(creator), json);
				}
			);
		}



		private MultiSearchResponse DeserializeMultiSearchHit(IElasticsearchResponse response, Stream stream, JsonConverter converter)
		{
			return this.Serializer.DeserializeInternal<MultiSearchResponse>(stream, converter);
		}

		private JsonConverter CreateMultiSearchConverter(IMultiSearchRequest descriptor)
		{
			if (descriptor.Operations != null)
			{
				foreach (var kv in descriptor.Operations)
					SearchPathInfo.CloseOverAutomagicCovariantResultSelector(this.Infer, kv.Value);				
			}


			var multiSearchConverter = new MultiSearchConverter(_connectionSettings, descriptor);
			return multiSearchConverter;
		}
	}
}