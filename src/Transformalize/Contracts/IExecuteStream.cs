using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Contracts {
   /// <summary>Opts into streaming execution, including documented compatibility fallbacks.</summary>
   public interface IExecuteStream {
      Task ExecuteStreamAsync(CancellationToken token = default);
   }
}
