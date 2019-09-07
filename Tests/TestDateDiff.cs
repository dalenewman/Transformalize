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
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tests.TestContainer;
using Transformalize.Configuration;

using Transformalize.Contracts;
using Transformalize.Logging;
using Transformalize.Transforms.Dates;

namespace Tests {

   [TestClass]
   public class TestDateDiff {

      [TestMethod]
      public void DateDiff1() {

         var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' >
          <rows>
            <add StartDate='2016-06-01' EndDate='2016-08-01' />
          </rows>
          <fields>
            <add name='StartDate' type='datetime' />
            <add name='EndDate' type='datetime' />
          </fields>
          <calculated-fields>
            <add name='Years' type='int' t='copy(StartDate,EndDate).datediff(year)' />
            <add name='Days' type='int' t='copy(StartDate,EndDate).datediff(day)' />
            <add name='Minutes' type='int' t='copy(StartDate,EndDate).datediff(minute)' />
            <add name='Hours' type='double' t='copy(StartDate,EndDate).datediff(hour)' />
          </calculated-fields>
        </add>
      </entities>
    </add>
            ".Replace('\'', '"');

         var transform = new TransformHolder((c)=>new DateDiffTransform(c), new DateDiffTransform().GetSignatures());

         using(var outer = new ConfigurationContainer(transform).CreateScope(xml,new DebugLogger())) {
            var process = outer.Resolve<Process>();
            using(var inner = new Container(transform).CreateScope(process, new DebugLogger())) {
               var output = inner.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(0, output[0][cf[0]]);
               Assert.AreEqual(61, output[0][cf[1]]);
               Assert.AreEqual(87840, output[0][cf[2]]);
               Assert.AreEqual(1464d, output[0][cf[3]]);
            }
         }
      }
   }
}
