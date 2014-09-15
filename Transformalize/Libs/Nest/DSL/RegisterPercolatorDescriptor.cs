using System;
using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;
using Transformalize.Libs.Nest.DSL.Query;
using Transformalize.Libs.Nest.Extensions;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL
{

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	[JsonConverter(typeof(CustomJsonConverter))]
	public interface IRegisterPercolatorRequest : IIndexNamePath<IndexRequestParameters>, ICustomJson
	{
		IDictionary<string, object> MetaData { get; set; }
		QueryContainer Query { get; set; }
	}

	internal static class RegisterPercolatorPathInfo
	{
		public static void Update(ElasticsearchPathInfo<IndexRequestParameters> pathInfo, IRegisterPercolatorRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.POST;
			pathInfo.Index = pathInfo.Index;
			pathInfo.Id = pathInfo.Name;
			pathInfo.Type = ".percolator";
		}
	}

	public class RegisterPercolatorRequest : IndexNamePathBase<IndexRequestParameters>, IRegisterPercolatorRequest
	{
		public RegisterPercolatorRequest(IndexNameMarker index, string name) : base(index, name)
		{
		}

		public IDictionary<string, object> MetaData { get; set; }
		public QueryContainer Query { get; set; }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<IndexRequestParameters> pathInfo)
		{
			RegisterPercolatorPathInfo.Update(pathInfo, this);
		}

		public object GetCustomJson()
		{
			return new FluentDictionary<string, object>(this.MetaData)
				.Add("query", this.Query);
		}

	}

	public class RegisterPercolatorDescriptor<T> : IndexNamePathDescriptor<RegisterPercolatorDescriptor<T>, IndexRequestParameters, T>, IRegisterPercolatorRequest
		where T : class
	{
		private IRegisterPercolatorRequest Self { get { return this; } }

		QueryContainer IRegisterPercolatorRequest.Query { get; set; }

		IDictionary<string, object> IRegisterPercolatorRequest.MetaData { get; set; }

		/// <summary>
		/// Add metadata associated with this percolator query document
		/// </summary>
		public RegisterPercolatorDescriptor<T> AddMetadata(Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
		{
			if (selector == null)
				return this;

			Self.MetaData = selector(new FluentDictionary<string, object>());
			return this;
		}

		/// <summary>
		/// The query to perform the percolation
		/// </summary>
		public RegisterPercolatorDescriptor<T> Query(Func<QueryDescriptor<T>, QueryContainer> querySelector)
		{
			querySelector.ThrowIfNull("querySelector");
			var d = querySelector(new QueryDescriptor<T>());
			Self.Query = d;
			return this;
		}

		public object GetCustomJson()
		{
			return new FluentDictionary<string, object>(Self.MetaData)
				.Add("query", Self.Query);
		}

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<IndexRequestParameters> pathInfo)
		{
			RegisterPercolatorPathInfo.Update(pathInfo, this);
		}
	}
}
