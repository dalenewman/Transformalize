using System;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;

namespace Transformalize.Libs.Nest
{
	public partial class ElasticClient
	{
		/// <inheritdoc />
		public IIndicesOperationResponse CreateIndex(Func<CreateIndexDescriptor, CreateIndexDescriptor> createIndexSelector)
		{
			var descriptor = createIndexSelector(new CreateIndexDescriptor(this._connectionSettings)); 
			return this.Dispatch<ICreateIndexRequest, CreateIndexRequestParameters, IndicesOperationResponse>(
				descriptor,
				(p, d) => this.RawDispatch.IndicesCreateDispatch<IndicesOperationResponse>(p, d.IndexSettings)
			);
		} 

		/// <inheritdoc />
		public IIndicesOperationResponse CreateIndex(ICreateIndexRequest createIndexRequest)
		{
			return this.Dispatch<ICreateIndexRequest, CreateIndexRequestParameters, IndicesOperationResponse>(
				createIndexRequest,
				(p, d) => this.RawDispatch.IndicesCreateDispatch<IndicesOperationResponse>(p, d.IndexSettings)
			);
		} 

		/// <inheritdoc />
		public Task<IIndicesOperationResponse> CreateIndexAsync(Func<CreateIndexDescriptor, CreateIndexDescriptor> createIndexSelector)
		{
			var descriptor = createIndexSelector(new CreateIndexDescriptor(this._connectionSettings)); 
			return this.DispatchAsync<ICreateIndexRequest, CreateIndexRequestParameters, IndicesOperationResponse, IIndicesOperationResponse>(
					descriptor,
					(p, d) => this.RawDispatch.IndicesCreateDispatchAsync<IndicesOperationResponse>(p, d.IndexSettings)
				);
		}

		/// <inheritdoc />
		public Task<IIndicesOperationResponse> CreateIndexAsync(ICreateIndexRequest createIndexRequest)
		{
			return this.DispatchAsync<ICreateIndexRequest, CreateIndexRequestParameters, IndicesOperationResponse, IIndicesOperationResponse>(
				createIndexRequest,
				(p, d) => this.RawDispatch.IndicesCreateDispatchAsync<IndicesOperationResponse>(p, d.IndexSettings)
			);
		}

	}
}