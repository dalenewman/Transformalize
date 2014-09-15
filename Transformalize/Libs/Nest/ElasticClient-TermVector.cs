using System;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;

namespace Transformalize.Libs.Nest
{
	public partial class ElasticClient
	{
		///<inheritdoc />
		public ITermVectorResponse TermVector<T>(Func<TermvectorDescriptor<T>, TermvectorDescriptor<T>> termVectorSelector)
			where T : class
		{
			return this.Dispatch<TermvectorDescriptor<T>, TermvectorRequestParameters, TermVectorResponse>(
				termVectorSelector,
				(p, d) => this.RawDispatch.TermvectorDispatch<TermVectorResponse>(p, d)
			);
		}

		///<inheritdoc />
		public ITermVectorResponse TermVector(ITermvectorRequest termvectorRequest)
		{
			return this.Dispatch<ITermvectorRequest, TermvectorRequestParameters, TermVectorResponse>(
				termvectorRequest,
				(p, d) => this.RawDispatch.TermvectorDispatch<TermVectorResponse>(p, d)
			);
		}

		///<inheritdoc />
		public Task<ITermVectorResponse> TermVectorAsync<T>(Func<TermvectorDescriptor<T>, TermvectorDescriptor<T>> termVectorSelector)
			where T : class
		{
			return this.DispatchAsync<TermvectorDescriptor<T>, TermvectorRequestParameters, TermVectorResponse, ITermVectorResponse>(
				termVectorSelector,
				(p, d) => this.RawDispatch.TermvectorDispatchAsync<TermVectorResponse>(p, d)
			);
		}

		///<inheritdoc />
		public Task<ITermVectorResponse> TermVectorAsync(ITermvectorRequest termvectorRequest)
		{
			return this.DispatchAsync<ITermvectorRequest , TermvectorRequestParameters, TermVectorResponse, ITermVectorResponse>(
				termvectorRequest,
				(p, d) => this.RawDispatch.TermvectorDispatchAsync<TermVectorResponse>(p, d)
			);
		}

	}
}
