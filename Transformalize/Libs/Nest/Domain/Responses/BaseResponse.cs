using Transformalize.Libs.Elasticsearch.Net.Domain.Response;
using Transformalize.Libs.Elasticsearch.Net.Exceptions;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.ExposedInternals;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IResponse : IResponseWithRequestInformation
	{
		bool IsValid { get; }
		IElasticsearchResponse ConnectionStatus { get; }
		ElasticInferrer Infer { get; }

		ElasticsearchServerError ServerError { get; }
	}
		
	public class BaseResponse : IResponse
	{
		private ElasticInferrer _infer;

		public virtual bool IsValid { get; internal set; }

		public BaseResponse()
		{
			this.IsValid = true;
		}
		
		IElasticsearchResponse IResponseWithRequestInformation.RequestInformation { get; set; }

		public IElasticsearchResponse ConnectionStatus { get { return ((IResponseWithRequestInformation)this).RequestInformation;  } }
		
		public ElasticsearchServerError ServerError
		{
			get
			{
				if (this.IsValid || this.ConnectionStatus == null || this.ConnectionStatus.OriginalException == null)
					return null;
				var e = this.ConnectionStatus.OriginalException as ElasticsearchServerException;
				if (e == null)
					return null;
				return new ElasticsearchServerError
				{
					Status = e.Status,
					Error = e.Message,
					ExceptionType = e.ExceptionType
				};
			}
		}
		
		protected IConnectionSettingsValues Settings
		{
			get
			{
				if (this.ConnectionStatus == null)
					return null;

				var settings = this.ConnectionStatus.Settings as IConnectionSettingsValues;
				return settings;
			}
		}

		public ElasticInferrer Infer
		{
			get
			{
				if (this._infer != null)
					return this._infer;

				var settings = this.Settings;
				if (settings == null)
					return null;
				this._infer = this.Settings.Inferrer;
				return this._infer;
			}
		}

	}
}
