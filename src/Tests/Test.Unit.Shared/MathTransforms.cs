#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
   public class MathTransforms {

      [TestMethod]
      public void DoMath() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='10.6954' Field2='129.992' Field3='7' Field4='3' />
          </rows>
          <fields>
            <add name='Field1' type='double' />
            <add name='Field2' type='decimal' />
            <add name='Field3' type='int' />
            <add name='Field4' type='int' />
          </fields>
          <calculated-fields>
            <add name='Ceiling' type='double' t='copy(Field1).ceiling()' />
            <add name='Floor' type='double' t='copy(Field1).floor()' />
            <add name='Round' type='decimal' t='copy(Field2).round(1)' />
            <add name='Abs' type='decimal' t='copy(Field2).abs()' />
            <add name='Add' type='decimal' t='copy(Field1,Field2,Field3).add()' />
            <add name='AddInts' type='int' t='copy(Field3,Field4).add()' />
            <add name='RoundTo5' type='double' t='copy(Field1).roundTo(5)' />
            <add name='RoundTo3' type='double' t='copy(Field1).roundTo(3)' />
            <add name='RoundTo7' type='decimal' t='copy(Field2).roundTo(7)' />
            <add name='RoundUpTo5' type='double' t='copy(Field1).roundUpTo(5)' />
            <add name='RoundDownTo4' type='int' t='copy(Field3).roundDownTo(4)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();

               var cf = process.Entities.First().CalculatedFields.ToArray();
               var row = output.First();
               Assert.AreEqual(11d, row[cf[0]]);
               Assert.AreEqual(10d, row[cf[1]]);
               Assert.AreEqual((decimal)130.00, row[cf[2]]);
               Assert.AreEqual((decimal)129.992, row[cf[3]]);
               Assert.AreEqual(147.6874m, row[cf[4]]);

               Assert.AreEqual(10, row[cf[5]]);
               Assert.AreEqual(10d, row[cf[6]], "nearest 5 is 10");
               Assert.AreEqual(12d, row[cf[7]], "nearest 3 is 12");
               Assert.AreEqual(133m, row[cf[8]], "nearest 7 is 133");
               Assert.AreEqual(15d, row[cf[9]], "next 5 is 15");
               Assert.AreEqual(4, row[cf[10]], "previous 4 is 4");
            }
         }
      }
   }
}
