#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2024 Dale Newman
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

namespace Transformalize.Contracts {
    public interface IPipeline : IRead, IDisposable {
        ActionResponse Initialize();
        void Register(IMapReader mapReader);
        void Register(IRead reader);
        void Register(ITransform transformer);
        void Register(IEnumerable<ITransform> transforms);
        void Register(IValidate validator);
        void Register(IEnumerable<IValidate> validators);
        void Register(IWrite writer);
        void Register(IUpdate updater);
        void Register(IEntityDeleteHandler deleteHandler);
        void Register(IOutputProvider output);
        void Register(IInputProvider input);
        void Execute();

        IContext Context { get; }
    }

}