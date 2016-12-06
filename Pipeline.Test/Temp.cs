#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2016 Dale Newman
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

using NUnit.Framework;
using Transformalize.Contracts;

namespace Transformalize.Test {

    [TestFixture]
    public class Temp {
        public const string Cfg = @"a url pointing to an xml or json configuration";

        [Test]
        [Ignore("Integration testing")]
        public void RunInit() {

            var composer = new CompositionRoot();
            composer.Compose(Cfg + @"?Mode=init", LogLevel.Info).Execute();

        }

        [Test]
        [Ignore("Integration testing")]
        public void RunDelta() {
            var composer = new CompositionRoot();
            var controller = composer.Compose(Cfg, LogLevel.Info);
            controller.Execute();
        }

    }


}
