using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Hit;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IValidateResponse : IResponse
	{
		bool Valid { get; }
		ShardsMetaData Shards { get; }
		IList<ValidationExplanation> Explanations { get; set; }
	}

	[JsonObject]
	public class ValidateResponse : BaseResponse, IValidateResponse
	{
		public ValidateResponse()
		{
			this.IsValid = true;
		}

		[JsonProperty(PropertyName = "valid")]
		public bool Valid { get; internal set; }

		[JsonProperty(PropertyName = "_shards")]
		public ShardsMetaData Shards { get; internal set; }

		/// <summary>
		/// Gets the explanations if Explain() was set.
		/// </summary>
		[JsonProperty(PropertyName = "explanations")]
		public IList<ValidationExplanation> Explanations { get; set;}
	}
}