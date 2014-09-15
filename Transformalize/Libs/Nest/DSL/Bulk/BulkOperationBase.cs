using System;
using Transformalize.Libs.Elasticsearch.Net.Domain;
using Transformalize.Libs.Nest.Domain.Bulk;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.ExposedInternals;
using Transformalize.Libs.Nest.Extensions;

namespace Transformalize.Libs.Nest.DSL.Bulk
{
	public abstract class BulkOperationBase : IBulkOperation
	{
		public abstract string Operation { get; }
		public abstract Type ClrType { get; }
		public IndexNameMarker Index { get; set; }
		public TypeNameMarker Type { get; set; }
		public string Id { get; set; }
		public string Version { get; set; }
		public VersionType? VersionType { get; set; }
		public string Routing { get; set; }
		public string Parent { get; set; }
		public long? Timestamp { get; set; }
		public string Ttl { get; set; }
		public int? RetriesOnConflict { get; set; }
		public abstract object GetBody();

		public virtual string GetIdForOperation(ElasticInferrer inferrer)
		{
			return !this.Id.IsNullOrEmpty() ? this.Id : inferrer.Id(this.GetBody());
		}
	}
}