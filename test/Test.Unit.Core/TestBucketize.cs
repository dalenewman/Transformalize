#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class BucketizeTransformTester {

      [TestMethod]
      public void BucketizeBasic() {

         const string xml = @"
<add name='TestProcess'>

    <maps>
        <add name='AgeGroups'>
            <items>
                <add from='*'  to='17' value='Under 18' />
                <add from='18' to='64' value='Working Age' />
                <add from='65' to='*'  value='Senior' />
            </items>
        </add>
    </maps>

    <entities>
        <add name='TestData'>
            <rows>
                <add Age='10' />
                <add Age='18' />
                <add Age='40' />
                <add Age='65' />
                <add Age='80' />
            </rows>
            <fields>
                <add name='Age' type='int' />
            </fields>
            <calculated-fields>
                <add name='AgeGroup' t='copy(Age).bucketize(AgeGroups)' default='Unknown' />
            </calculated-fields>
        </add>
    </entities>

</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var field = process.Entities.First().CalculatedFields.First();
               Assert.AreEqual("Under 18", output[0][field]);
               Assert.AreEqual("Working Age", output[1][field]);
               Assert.AreEqual("Working Age", output[2][field]);
               Assert.AreEqual("Senior", output[3][field]);
               Assert.AreEqual("Senior", output[4][field]);
            }
         }
      }

      [TestMethod]
      public void BucketizeFallThrough() {

         const string xml = @"
<add name='TestProcess'>

    <maps>
        <add name='Scores'>
            <items>
                <add from='90' to='100' value='A' />
                <add from='80' to='89'  value='B' />
                <add from='70' to='79'  value='C' />
            </items>
        </add>
    </maps>

    <entities>
        <add name='TestData'>
            <rows>
                <add Score='95' />
                <add Score='85' />
                <add Score='60' />
            </rows>
            <fields>
                <add name='Score' type='int' />
            </fields>
            <calculated-fields>
                <add name='Grade' t='copy(Score).bucketize(Scores)' />
            </calculated-fields>
        </add>
    </entities>

</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var field = process.Entities.First().CalculatedFields.First();
               Assert.AreEqual("A", output[0][field]);
               Assert.AreEqual("B", output[1][field]);
               Assert.AreEqual("60", output[2][field], "Value outside all buckets should fall through as string");
            }
         }
      }

      [TestMethod]
      public void BucketizeDecimalInput() {

         const string xml = @"
<add name='TestProcess'>

    <maps>
        <add name='Ranges'>
            <items>
                <add from='*'    to='0'    value='Negative' />
                <add from='0.01' to='9.99' value='Low' />
                <add from='10'   to='*'    value='High' />
            </items>
        </add>
    </maps>

    <entities>
        <add name='TestData'>
            <rows>
                <add Amount='-5.5' />
                <add Amount='0' />
                <add Amount='5.5' />
                <add Amount='10' />
                <add Amount='100.25' />
            </rows>
            <fields>
                <add name='Amount' type='double' />
            </fields>
            <calculated-fields>
                <add name='Bucket' t='copy(Amount).bucketize(Ranges)' />
            </calculated-fields>
        </add>
    </entities>

</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var field = process.Entities.First().CalculatedFields.First();
               Assert.AreEqual("Negative", output[0][field]);
               Assert.AreEqual("Negative", output[1][field], "0 is <= 0, so it falls in Negative");
               Assert.AreEqual("Low", output[2][field]);
               Assert.AreEqual("High", output[3][field]);
               Assert.AreEqual("High", output[4][field]);
            }
         }
      }

      [TestMethod]
      public void BucketizeFirstDefinedWinsOnOverlap() {

         const string xml = @"
<add name='TestProcess'>

    <maps>
        <add name='Overlap'>
            <items>
                <add from='1'  to='20' value='Wide' />
                <add from='10' to='15' value='Narrow' />
            </items>
        </add>
    </maps>

    <entities>
        <add name='TestData'>
            <rows>
                <add Val='12' />
            </rows>
            <fields>
                <add name='Val' type='int' />
            </fields>
            <calculated-fields>
                <add name='Bucket' t='copy(Val).bucketize(Overlap)' />
            </calculated-fields>
        </add>
    </entities>

</add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var field = process.Entities.First().CalculatedFields.First();
               Assert.AreEqual("Wide", output[0][field], "First defined bucket wins when ranges overlap");
            }
         }
      }

   }
}
