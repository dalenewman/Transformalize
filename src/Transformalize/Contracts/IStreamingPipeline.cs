using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Contracts {
   public interface IStreamingPipeline : IPipeline, IReadStream, IExecuteStream { }
}
