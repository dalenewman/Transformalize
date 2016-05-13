#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Collections.Generic;
using System.Linq;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Nulls;

namespace Pipeline {
    public class DefaultPipeline : IPipeline {

        readonly IOutputController _controller;

        protected IRead Reader { get; private set; }
        protected IWrite Writer { get; private set; }
        protected IUpdate Updater { get; private set; }
        protected IEntityDeleteHandler DeleteHandler { get; private set; }
        protected List<ITransform> Transformers { get; }

        readonly PipelineContext _context;

        public DefaultPipeline(IOutputController controller, IContext context) {
            _context = (PipelineContext)context;
            _controller = controller;
            Transformers = new List<ITransform>();

            _context.Debug(() => $"Registering {GetType().Name}.");
            _context.Debug(() => $"Registering {_controller.GetType().Name}.");
        }

        public void Initialize() {
            _controller.Initialize();
        }

        public void Register(IRead reader) {
            _context.Debug(() => $"Registering {reader.GetType().Name}.");
            Reader = reader;
        }

        public void Register(ITransform transform) {
            _context.Debug(() => $"Registering {transform.GetType().Name}.");
            Transformers.Add(transform);
        }

        public void Register(IEnumerable<ITransform> transforms) {
            foreach (var transform in transforms) {
                Register(transform);
            }
        }

        public void Register(IWrite writer) {
            _context.Debug(() => $"Registering {writer.GetType().Name}.");
            Writer = writer;
        }

        public void Register(IUpdate updater) {
            _context.Debug(() => $"Registering {updater.GetType().Name}.");
            Updater = updater;
        }

        public void Register(IEntityDeleteHandler deleteHandler) {
            _context.Debug(() => $"Registering {deleteHandler.GetType().Name}.");
            DeleteHandler = deleteHandler;
        }

        public virtual IEnumerable<IRow> Read() {
            _context.Debug(() => $"Running {Transformers.Count} transforms.");
            if (_context.Entity.NeedsUpdate()) {
                if (_context.Entity.Version != string.Empty) {
                    if (_context.Entity.GetVersionField().Type == "byte[]") {
                        var min = _context.Entity.MinVersion == null ? "null" : Utility.BytesToHexString((byte[])_context.Entity.MinVersion).TrimStart(new[] { '0' });
                        var max = _context.Entity.MaxVersion == null ? "null" : Utility.BytesToHexString((byte[])_context.Entity.MaxVersion).TrimStart(new[] { '0' });
                        _context.Info("Change Detected: Input:0x{0:X} != Output:0x{1:X}", max, min);
                    } else {
                        _context.Info("Change Detected: Input:{0} > Output:{1}", _context.Entity.MaxVersion, _context.Entity.MinVersion);
                    }
                }
                return Transformers.Aggregate(Reader.Read(), (current, transform) => current.Select(transform.Transform));
            }
            _context.Info("Change Detected: No.");
            return Enumerable.Empty<IRow>();
        }

        public void Execute() {
            _controller.Start();
            DeleteHandler?.Delete();
            Writer.Write(Read());
            Updater.Update();
            _controller.End();
        }

        /// <summary>
        /// CAUTION: If you're using Read without Execute, make sure you consume enumerable before disposing
        /// </summary>
        public void Dispose() {
            Transformers.Clear();
            Reader = null;
            Writer = null;
            Updater = null;
            DeleteHandler = null;
        }
    }
}