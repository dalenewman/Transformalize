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
using System.Linq;
using Transformalize.Contracts;

namespace Transformalize.Impl {
   public class DefaultPipeline : IPipeline {

      private readonly IOutputController _controller;

      protected IRead Reader { get; private set; }
      protected IWrite Writer { get; private set; }
      protected IUpdate Updater { get; private set; }
      protected IEntityDeleteHandler DeleteHandler { get; private set; }
      protected List<ITransform> Transforms { get; }
      protected List<IValidate> Validators { get; }
      protected List<IMapReader> MapReaders { get; }
      protected IOutputProvider OutputProvider { get; private set; }
      protected IInputProvider InputProvider { get; private set; }

      public IContext Context { get; }

      public DefaultPipeline(IOutputController controller, IContext context) {
         Context = context;
         _controller = controller;
         Transforms = new List<ITransform>();
         MapReaders = new List<IMapReader>();
         Validators = new List<IValidate>();

         Context.Debug(() => $"Registering {GetType().Name}.");
         Context.Debug(() => $"Registering {_controller.GetType().Name}.");
      }

      public void Initialize() {
         _controller.Initialize();
      }

      public void Register(IMapReader mapReader) {
         Context.Debug(() => $"Registering {mapReader.GetType().Name}.");
         MapReaders.Add(mapReader);
      }

      public void Register(IRead reader) {
         if (reader != null) {
            Context.Debug(() => $"Registering {reader.GetType().Name}.");
         }
         Reader = reader;
      }

      public void Register(IOutputProvider output) {
         if (output != null) {
            Context.Debug(() => $"Registering {output.GetType().Name}.");
         }
         OutputProvider = output;
      }

      public void Register(IInputProvider input) {
         if (input != null) {
            Context.Debug(() => $"Registering {input.GetType().Name}.");
         }
         InputProvider = input;
      }

      public void Register(ITransform transform) {
         Context.Debug(() => $"Registering {transform.GetType().Name}.");
         Transforms.Add(transform);
      }

      public void Register(IEnumerable<ITransform> transforms) {
         foreach (var transform in transforms) {
            Register(transform);
         }
      }

      public void Register(IEnumerable<IValidate> validators) {
         foreach (var v in validators) {
            Register(v);
         }
      }
      public void Register(IWrite writer) {
         if (writer != null) {
            Context.Debug(() => $"Registering {writer.GetType().Name}.");
         }
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
         Context.Debug(() => $"Running {Transforms.Count} transforms.");
         if (Context.Entity.NeedsUpdate()) {
            if (Context.Process.Mode != "init") {
               if (Context.Entity.Version != string.Empty) {
                  var version = Context.Entity.GetVersionField();
                  if (version.Type == "byte[]") {
                     var min = Context.Entity.MinVersion == null ? "null" : "0x" + string.Format("{0:X}", Utility.BytesToHexString((byte[])Context.Entity.MinVersion).TrimStart(new[] { '0' }));
                     var max = Context.Entity.MaxVersion == null ? "null" : "0x" + string.Format("{0:X}", Utility.BytesToHexString((byte[])Context.Entity.MaxVersion).TrimStart(new[] { '0' }));
                     Context.Info("Change Detected: Input: {0} > Output: {1}", max, min);
                  } else {
                     Context.Info("Change Detected: Input: {0} > Output: {1}", Context.Entity.MaxVersion, Context.Entity.MinVersion);
                  }
               }
            }
            var data = Reader == null ? InputProvider.Read() : Reader.Read();
            if (Transforms.Any()) {
               data = Transforms.Aggregate(data, (rows, t) => t.Operate(rows));
            }
            if (Validators.Any()) {
               data = Validators.Aggregate(data, (rows, v) => v.Operate(rows));
            }
            return data;
         }
         Context.Info("Change Detected: No.");
         return Enumerable.Empty<IRow>();
      }

      public void Execute() {
         _controller.Start();
         DeleteHandler?.Delete();
         if (Writer == null) {
            OutputProvider.Write(Read());
         } else {
            Writer.Write(Read());
         }
         Updater.Update();
         _controller.End();
      }

      /// <inheritdoc />
      /// <summary>
      /// CAUTION: If you're using Read without Execute, make sure you consume enumerable before disposing
      /// </summary>
      public void Dispose() {
         foreach (var transform in Transforms) {
            transform.Dispose();
         }
         foreach (var validator in Validators) {
            validator.Dispose();
         }
         Transforms.Clear();
         Validators.Clear();
         Reader = null;
         Writer = null;
         Updater = null;
         DeleteHandler = null;
      }

      public void Register(IValidate validator) {
         Context.Debug(() => $"Registering {validator.GetType().Name}.");
         Validators.Add(validator);
      }
   }
}