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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {

    [TestClass]
    public class ConnectionTransform {

        [TestMethod]
        public void FileStuff() {

            const string xml = @"
    <add name='TestProcess'>
      <connections>
        <add name='input' provider='internal' file='c:\temp.txt' port='6' />
        <add name='other' provider='internal' file='\\server\projects\ETL\2016-04-24.txt' />
      </connections>
      <entities>
        <add name='TestData'>
          <rows>
            <add Field1='1' Field2='2' Field3='3' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='File' type='string' t='connection(input,File)' />
            <add name='Port' type='int' t='connection(input,Port)' />            
            <add name='FileName' length='128' t='connection(other,File).filename()' />
            <add name='FileNameNoExt' length='128' t='connection(other,File).filename(false)' />
            <add name='FileExt' length='128' t='connection(other,File).fileext()' />
            <add name='FilePath' length='128' t='connection(other,File).filepath()' />
          </calculated-fields>
        </add>
      </entities>
    </add>
            ";

            var composer = new CompositionRoot();
            var controller = composer.Compose(xml);
            var output = controller.Read().ToArray();

            var f = composer.Process.Entities.First().CalculatedFields;
            var file = f.First(cf => cf.Name == "File");
            var port = f.First(cf => cf.Name == "Port");
            var fileName = f.First(cf => cf.Name == "FileName");
            var fileNameNoExt = f.First(cf => cf.Name == "FileNameNoExt");
            var fileExt = f.First(cf => cf.Name == "FileExt");
            var filePath = f.First(cf => cf.Name == "FilePath");

            Assert.AreEqual(@"c:\temp.txt", output[0][file]);
            Assert.AreEqual(6, output[0][port]);
            Assert.AreEqual(@"2016-04-24.txt", output[0][fileName]);
            Assert.AreEqual(@"2016-04-24", output[0][fileNameNoExt]);
            Assert.AreEqual(@".txt", output[0][fileExt]);
            Assert.AreEqual(@"\\server\projects\ETL\2016-04-24.txt", output[0][filePath]);

        }
    }
}
