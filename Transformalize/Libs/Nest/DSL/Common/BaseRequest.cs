using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Connection.Configuration;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;

namespace Transformalize.Libs.Nest.DSL.Common
{
	public abstract class BaseRequest<TParameters> : IRequest<TParameters>
		where TParameters : IRequestParameters, new()
	{
		[JsonIgnore]
		protected IRequest<TParameters> Request { get { return this; } }

		[JsonIgnore]
		IRequestConfiguration IRequest<TParameters>.RequestConfiguration
		{
			get { return _requestConfiguration; }
			set { _requestConfiguration = value; }
		}

		private TParameters _requestParameters = new TParameters();
		private IRequestConfiguration _requestConfiguration;

		[JsonIgnore]
		TParameters IRequest<TParameters>.RequestParameters  
		{ 
			get { return _requestParameters; }
			set { _requestParameters = value; }
		}

		internal virtual ElasticsearchPathInfo<TParameters> ToPathInfo(
			IConnectionSettingsValues settings, 
			TParameters queryString
			)
		{
			var pathInfo = new ElasticsearchPathInfo<TParameters>();
			pathInfo.RequestParameters = queryString;
			var config = this._requestConfiguration;
			if (config != null)
			{
				IRequestParameters p = pathInfo.RequestParameters;
				p.RequestConfiguration = config;
			}

			SetRouteParameters(settings, pathInfo);
			UpdatePathInfo(settings, pathInfo);
			ValidatePathInfo(pathInfo);

			return pathInfo;
		}

		protected virtual void SetRouteParameters(
			IConnectionSettingsValues settings, 
			ElasticsearchPathInfo<TParameters> pathInfo)
		{
			
		}

		protected virtual void ValidatePathInfo(ElasticsearchPathInfo<TParameters> pathInfo)
		{
		}

		protected abstract void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<TParameters> pathInfo);
		
		ElasticsearchPathInfo<TParameters> IPathInfo<TParameters>.ToPathInfo(IConnectionSettingsValues settings)
		{
			return this.ToPathInfo(settings, this.Request.RequestParameters);
		}

	}
}