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

using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Contracts;

namespace Tests {

    [TestClass]
    public class GeoJsonProvider {

        [TestMethod]
        public void SeeItWork() {

            const string xml = @"
<add name='TestProcess'>
    <parameters>
        <add name='File' value='' />
    </parameters>
    <connections>
        <add name='input' provider='internal' />
        <add name='output' provider='geojson' file='@(File)' />
    </connections>
    <entities>
        <add name='TestData'>
            <rows>
                <add Lat='39.0997' Lon='94.5786' Name='Kansas City' Bird='Eastern Bluebird' />
            </rows>
            <fields>
                <add name='Lat' type='double' />
                <add name='Lon' type='double' />
                <add name='Name' />
                <add name='Bird' />
            </fields>
        </add>
    </entities>
</add>";

            var composer = new CompositionRoot();
            var filename = Path.GetTempFileName();
            var controller = composer.Compose(xml, LogLevel.Info, new Dictionary<string, string> { { "File", filename } });
            controller.Execute();

            var results = File.ReadAllText(filename);

            Assert.AreNotEqual("", results);
        }

    }
}
