#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Contracts;

namespace Transformalize.Impl {

   [Obsolete("No longer necessary.  AsParallel() is now applied in DefaultPipeline.")]
   public class ParallelPipeline : IPipeline {
        private readonly IPipeline _pipeline;

        public IContext Context => _pipeline.Context;

        public ParallelPipeline(IPipeline pipeline) {
            _pipeline = pipeline;
        }

        public void Execute() {
            _pipeline.Execute();
        }

        public ActionResponse Initialize() {
            return _pipeline.Initialize();
        }

        public void Register(IMapReader mapReader) {
            _pipeline.Register(mapReader);
        }

        public void Register(IEnumerable<ITransform> transforms) {
            _pipeline.Register(transforms);
        }

        public void Register(IUpdate updater) {
            _pipeline.Register(updater);
        }

        public void Register(IEntityDeleteHandler deleteHandler) {
            _pipeline.Register(deleteHandler);
        }

        public void Register(IEnumerable<IValidate> validators) {
            _pipeline.Register(validators);
        }

        public void Register(IWrite writer) {
            _pipeline.Register(writer);
        }

        public void Register(ITransform transformer) {
            _pipeline.Register(transformer);
        }

        public void Register(IRead reader) {
            _pipeline.Register(reader);
        }

        public IEnumerable<IRow> Read() {
            return _pipeline.Read();
        }

        /// <inheritdoc />
        /// <summary>
        /// CAUTION: If you're using Read without Execute, make sure you consume enumerable before disposing
        /// </summary>
        public void Dispose() {
            _pipeline.Dispose();
        }

        public void Register(IOutputProvider output) {
            _pipeline.Register(output);
        }

        public void Register(IInputProvider input) {
            _pipeline.Register(input);
        }

        public void Register(IValidate validator) {
            _pipeline.Register(validator);
        }

        public void Register(IOutputProviderAsync output) {
            _pipeline.Register(output);
        }

        public void Register(IInputProviderAsync input) {
            _pipeline.Register(input);
        }

        public Task<ActionResponse> InitializeAsync(CancellationToken cancellationToken = default) => _pipeline.InitializeAsync(cancellationToken);

        public Task ExecuteAsync(CancellationToken cancellationToken = default) => _pipeline.ExecuteAsync(cancellationToken);

        public async IAsyncEnumerable<IRow> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken = default) {
            await foreach (var row in _pipeline.ReadAsync(cancellationToken)) {
                yield return row;
            }
        }
    }
}