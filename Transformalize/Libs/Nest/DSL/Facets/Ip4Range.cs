using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Libs.Nest.DSL.Facets
{
	public class Ip4Range 
	{
		[JsonProperty(PropertyName = "mask")]
		internal string _Mask { get; set; }

		public Ip4Range Mask(string value)
		{
			this._Mask = value;
			return this;
		}
	}
}