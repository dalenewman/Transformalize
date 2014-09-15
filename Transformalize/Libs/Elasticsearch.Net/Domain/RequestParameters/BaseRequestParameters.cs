using System;
using System.Collections.Generic;
using System.IO;
using Transformalize.Libs.Elasticsearch.Net.Connection.Configuration;
using Transformalize.Libs.Elasticsearch.Net.Domain.Response;

namespace Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters
{
	public class RequestParameters : IRequestParameters
	{
		public IDictionary<string, object> QueryString { get; set; }
		public Func<IElasticsearchResponse, Stream, object> DeserializationState { get; set; }
		public IRequestConfiguration RequestConfiguration { get; set; }
		
		public RequestParameters()
		{
			this.QueryString = new Dictionary<string, object>();
		}

		protected TOut GetQueryStringValue<TOut>(string name)
		{
			if (!this.QueryString.ContainsKey(name))
				return default(TOut);
			var value = this.QueryString[name];
			if (value == null)
				return default(TOut);
			return (TOut)value;
		}
	}
}