#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using System.Collections.Generic;
using Transformalize.Contracts;
using System.Linq;

namespace Transformalize.Impl {
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
#if NETS10
            return _pipeline.Read();
#else
            return _pipeline.Read().AsParallel();
#endif
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
    }
}