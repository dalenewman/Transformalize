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
using Autofac;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac.Modules;

namespace Transformalize.Ioc.Autofac {
    public static class ConfigurationContainer {
        public static ILifetimeScope Create(string cfg, IPipelineLogger logger, Dictionary<string, string> parameters = null, string placeHolderStyle = "@()") {
            var builder = new ContainerBuilder();
            builder.Register(c => cfg).Named<string>("cfg");
            builder.Register(c => logger).As<IPipelineLogger>();
            builder.Register(c => placeHolderStyle).Named<string>("placeHolderStyle");
            builder.RegisterModule(new TransformModule(new Process { Name = "ConfigurationContainer" }, logger));
            builder.RegisterModule(new ValidateModule(new Process { Name = "ConfigurationContainer" }, logger));
            builder.RegisterModule(new RootModule());
            return builder.Build();
        }
    }

}
