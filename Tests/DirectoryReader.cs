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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Contracts;

namespace Tests {

    [TestClass]
    public class DirectoryReader {

        [TestMethod]
        [Ignore]
        public void Read1() {

            const string xml = @"
<add name='Directory' mode='default'>
    <connections>
        <add name='input' provider='directory' folder='c:\temp' />
        <add name='output' provider='console' />
    </connections>
    <entities>
        <add name='Files'>
            <fields>
                <add name='DirectoryName' length='256' />
                <add name='FullName' length='256' />
                <add name='LastWriteTimeUtc' type='datetime' />
            </fields>
        </add>
    </entities>
</add>";


            var composer = new CompositionRoot();
            var controller = composer.Compose(xml, LogLevel.Debug);
            var process = composer.Process;
            controller.Execute();
            controller.Dispose();

            var entity = process.Entities.First();
            Assert.AreEqual(115, entity.Rows.Count, "Most people keep about 115 files in there.");

        }

    }
}
