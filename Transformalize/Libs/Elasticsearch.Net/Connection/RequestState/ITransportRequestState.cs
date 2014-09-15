using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Libs.Elasticsearch.Net.Connection.Configuration;
using Transformalize.Libs.Elasticsearch.Net.Domain.Response;

namespace Transformalize.Libs.Elasticsearch.Net.Connection.RequestState
{
	internal interface ITransportRequestState
	{
		IRequestTimings InitiateRequest(RequestType requestType);
		Uri CreatePathOnCurrentNode(string path);
		IRequestConfiguration RequestConfiguration { get; }
		int Retried { get; }
		bool SniffedOnConnectionFailure { get; set; }
		int? Seed { get; set; }
		Uri CurrentNode { get; set; }
		List<RequestMetrics> RequestMetrics { get; set; }
		List<Exception> SeenExceptions { get; }
		Func<IElasticsearchResponse, Stream, object> ResponseCreationOverride { get; set; }
	}
}