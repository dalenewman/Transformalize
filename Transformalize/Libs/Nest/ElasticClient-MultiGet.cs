using System;
using System.IO;
using System.Threading.Tasks;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Elasticsearch.Net.Domain.Response;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;
using Transformalize.Libs.Nest.Extensions;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest
{
    public partial class ElasticClient
	{
		/// <inheritdoc />
		public IMultiGetResponse MultiGet(Func<MultiGetDescriptor, MultiGetDescriptor> multiGetSelector)
		{
			multiGetSelector.ThrowIfNull("multiGetSelector");
			var descriptor = multiGetSelector(new MultiGetDescriptor());
			var converter = CreateCovariantMultiGetConverter(descriptor);
			var customCreator = new Func<IElasticsearchResponse, Stream, MultiGetResponse>((r, s) => this.DeserializeMultiGetResponse(r, s, converter));
			return this.Dispatch<MultiGetDescriptor, MultiGetRequestParameters, MultiGetResponse>(
				descriptor,
				(p, d) => this.RawDispatch.MgetDispatch<MultiGetResponse>(p.DeserializationState(customCreator), d)
			);
		}

		/// <inheritdoc />
		public IMultiGetResponse MultiGet(IMultiGetRequest multiRequest)
		{
			var converter = CreateCovariantMultiGetConverter(multiRequest);
			var customCreator = new Func<IElasticsearchResponse, Stream, MultiGetResponse>((r, s) => this.DeserializeMultiGetResponse(r, s, converter));
			return this.Dispatch<IMultiGetRequest, MultiGetRequestParameters, MultiGetResponse>(
				multiRequest,
				(p, d) => this.RawDispatch.MgetDispatch<MultiGetResponse>(p.DeserializationState(customCreator), d)
			);
		}

		/// <inheritdoc />
		public Task<IMultiGetResponse> MultiGetAsync(Func<MultiGetDescriptor, MultiGetDescriptor> multiGetSelector)
		{
			multiGetSelector.ThrowIfNull("multiGetSelector");
			var descriptor = multiGetSelector(new MultiGetDescriptor());
			var converter = CreateCovariantMultiGetConverter(descriptor);
			var customCreator = new Func<IElasticsearchResponse, Stream, MultiGetResponse>((r, s) => this.DeserializeMultiGetResponse(r, s, converter));
			return this.DispatchAsync<MultiGetDescriptor, MultiGetRequestParameters, MultiGetResponse, IMultiGetResponse>(
				descriptor,
				(p, d) => this.RawDispatch.MgetDispatchAsync<MultiGetResponse>(p.DeserializationState(customCreator), d)
			);
		}

		/// <inheritdoc />
		public Task<IMultiGetResponse> MultiGetAsync(IMultiGetRequest multiGetRequest)
		{
			var converter = CreateCovariantMultiGetConverter(multiGetRequest);
			var customCreator = new Func<IElasticsearchResponse, Stream, MultiGetResponse>((r, s) => this.DeserializeMultiGetResponse(r, s, converter));
			return this.DispatchAsync<IMultiGetRequest, MultiGetRequestParameters, MultiGetResponse, IMultiGetResponse>(
				multiGetRequest,
				(p, d) => this.RawDispatch.MgetDispatchAsync<MultiGetResponse>(p.DeserializationState(customCreator), d)
			);
		}



		private MultiGetResponse DeserializeMultiGetResponse(IElasticsearchResponse response, Stream stream, JsonConverter converter)
		{
			return this.Serializer.DeserializeInternal<MultiGetResponse>(stream, converter);
		}

		private JsonConverter CreateCovariantMultiGetConverter(IMultiGetRequest descriptor)
		{
			var multiGetHitConverter = new MultiGetHitConverter(descriptor);
			return multiGetHitConverter;
		}
	}
}