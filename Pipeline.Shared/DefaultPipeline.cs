#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize {
    public class DefaultPipeline : IPipeline {

        readonly IOutputController _controller;

        protected IRead Reader { get; private set; }
        protected IWrite Writer { get; private set; }
        protected IUpdate Updater { get; private set; }
        protected IEntityDeleteHandler DeleteHandler { get; private set; }
        protected List<ITransform> Transformers { get; }
        protected List<IMapReader> MapReaders { get; }

        public IContext Context { get; }
        public bool Valid { get; private set; }

        public DefaultPipeline(IOutputController controller, IContext context) {
            Valid = true;
            Context = context;
            _controller = controller;
            Transformers = new List<ITransform>();
            MapReaders = new List<IMapReader>();

            Context.Debug(() => $"Registering {GetType().Name}.");
            Context.Debug(() => $"Registering {_controller.GetType().Name}.");
        }

        public void Initialize() {
            if (Valid) {
                _controller.Initialize();
            }
        }

        public void Register(IMapReader mapReader) {
            Context.Debug(() => $"Registering {mapReader.GetType().Name}.");
            MapReaders.Add(mapReader);
        }

        public void Register(IRead reader) {
            Context.Debug(() => $"Registering {reader.GetType().Name}.");
            Reader = reader;
        }

        public void Register(ITransform transform) {
            Context.Debug(() => $"Registering {transform.GetType().Name}.");
            Transformers.Add(transform);
        }

        public void Register(Transforms.Transforms transforms) {
            Valid = transforms.Valid;
            foreach (var transform in transforms) {
                Register(transform);
            }
        }

        public void Register(IWrite writer) {
            Context.Debug(() => $"Registering {writer.GetType().Name}.");
            Writer = writer;
        }

        public void Register(IUpdate updater) {
            Context.Debug(() => $"Registering {updater.GetType().Name}.");
            Updater = updater;
        }

        public void Register(IEntityDeleteHandler deleteHandler) {
            Context.Debug(() => $"Registering {deleteHandler.GetType().Name}.");
            DeleteHandler = deleteHandler;
        }

        public virtual IEnumerable<IRow> Read() {
            if (Valid) {
                Context.Debug(() => $"Running {Transformers.Count} transforms.");
                if (Context.Entity.NeedsUpdate()) {
                    if (Context.Entity.Version != string.Empty) {
                        if (Context.Entity.GetVersionField().Type == "byte[]") {
                            var min = Context.Entity.MinVersion == null ? "null" : Utility.BytesToHexString((byte[])Context.Entity.MinVersion).TrimStart(new[] { '0' });
                            var max = Context.Entity.MaxVersion == null ? "null" : Utility.BytesToHexString((byte[])Context.Entity.MaxVersion).TrimStart(new[] { '0' });
                            Context.Info("Change Detected: Input:0x{0:X} != Output:0x{1:X}", max, min);
                        } else {
                            Context.Info("Change Detected: Input:{0} > Output:{1}", Context.Entity.MaxVersion, Context.Entity.MinVersion);
                        }
                    }
                    return Transformers.Aggregate(Reader.Read(), (current, transformer) => transformer.Transform(current));
                }
                Context.Info("Change Detected: No.");
            }
            return Enumerable.Empty<IRow>();
        }

        public void Execute() {
            _controller.Start();
            if (Valid) {
                DeleteHandler?.Delete();
                Writer.Write(Read());
                Updater.Update();
            }
            _controller.End();
        }

        /// <summary>
        /// CAUTION: If you're using Read without Execute, make sure you consume enumerable before disposing
        /// </summary>
        public void Dispose() {
            foreach (var transform in Transformers) {
                transform.Dispose();
            }
            Transformers.Clear();
            Reader = null;
            Writer = null;
            Updater = null;
            DeleteHandler = null;
        }
    }
}