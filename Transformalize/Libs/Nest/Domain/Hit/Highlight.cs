using System.Collections.Generic;

namespace Transformalize.Libs.Nest.Domain.Hit
{
	public class Highlight
	{
		public string DocumentId { get; internal set; }
		public string Field { get; internal set; }
		public IEnumerable<string> Highlights { get; set; }
	}
}
