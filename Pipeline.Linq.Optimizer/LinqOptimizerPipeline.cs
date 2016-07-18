#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using Nessos.LinqOptimizer.CSharp;
using Pipeline.Contracts;

namespace Pipeline.Linq.Optimizer {
    public class LinqOptimizerPipeline : IPipeline {
        private readonly IPipeline _pipeline;

        public LinqOptimizerPipeline(IPipeline pipeline) {
            _pipeline = pipeline;
        }

        public IEnumerable<IRow> Read() {
            return _pipeline.Read().AsParallelQueryExpr().Run();
        }

        public void Dispose() {
            _pipeline.Dispose();
        }

        public void Initialize() {
            _pipeline.Initialize();
        }

        public void Register(IMapReader mapReader) {
            _pipeline.Register(mapReader);
        }

        public void Register(IRead reader) {
            _pipeline.Register(reader);
        }

        public void Register(ITransform transformer) {
            _pipeline.Register(transformer);
        }

        public void Register(IEnumerable<ITransform> transforms) {
            _pipeline.Register(transforms);
        }

        public void Register(IWrite writer) {
            _pipeline.Register(writer);
        }

        public void Register(IUpdate updater) {
            _pipeline.Register(updater);
        }

        public void Register(IEntityDeleteHandler deleteHandler) {
            _pipeline.Register(deleteHandler);
        }

        public void Execute() {
            _pipeline.Execute();
        }

        public IContext Context => _pipeline.Context;
    }
}
