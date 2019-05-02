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
using System;
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class TryDateConvert {

      [TestMethod]
      public void TryConvertSpecificFormat() {

         var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' >
          <rows>
            <add OwensDate='02152018040000' />
          </rows>
          <fields>
            <add name='OwensDate' type='datetime' t='convert(date,MMddyyyyHHmmss)' />
          </fields>
        </add>
      </entities>
    </add>
            ".Replace('\'', '"');


         var logger = new ConsoleLogger(LogLevel.Debug);
         using(var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using(var inner = new Container().CreateScope(process, logger)) {
               inner.Resolve<IProcessController>().Execute();
               var row = process.Entities.First().Rows.First();
               Assert.AreEqual(new DateTime(2018, 2, 15, 4, 0, 0), row["OwensDate"]);
            }
         }


      }

      [TestMethod]
      public void MakeSureTimeIsntChanging() {

         var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' >
          <rows>
            <add Date='2019-05-05 1:05 PM' />
          </rows>
          <fields>
            <add name='Date' type='datetime' />
          </fields>
          <calculated-fields>
            <add name='DateDefault' type='datetime' default='2019-05-05 1:07 PM' />
            <add name='DateWithZ' type='datetime' default='2019-05-05 1:08 PM Z' />
            <add name='DateWithOffSet' type='datetime' default='2019-05-05 1:09 PM +00:00' />
            <add name='DateWithOffSet4' type='datetime' default='2019-05-05 1:09 PM -04:00' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container().CreateScope(process, logger)) {
               inner.Resolve<IProcessController>().Execute();
               var row = process.Entities.First().Rows.First();
               Assert.AreEqual(new DateTime(2019, 5, 5, 13, 5, 0), row["Date"]);
               Assert.AreEqual(new DateTime(2019, 5, 5, 13, 7, 0), row["DateDefault"]);
               Assert.AreEqual(new DateTime(2019, 5, 5, 13, 8, 0), row["DateWithZ"]);
               Assert.AreEqual(new DateTime(2019, 5, 5, 13, 9, 0), row["DateWithOffSet"]);
               Assert.AreEqual(new DateTime(2019, 5, 5, 17, 9, 0), row["DateWithOffSet4"]);
            }
         }


      }
   }
}
