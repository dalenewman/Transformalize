using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Contracts {
   /// <summary>Optional pull-based row source. Consume and dispose within the owning scope.</summary>
   public interface IReadStream {
      IAsyncEnumerable<IRow> ReadStreamAsync(CancellationToken token = default);
   }
}
