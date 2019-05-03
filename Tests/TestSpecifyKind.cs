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
using Transformalize.Transforms.Dates;

namespace Tests {

   [TestClass]
   public class TestSpecifyKind {

      [TestMethod]
      public void Try() {

         var xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' >
          <rows>
            <add Date1='2019-05-05 1:05 PM' Date2='2019-05-05 1:05 PM' Date3='2019-05-05 1:05 PM' Date4='2019-05-05 1:05 PM' Date5='2019-05-05T13:05:00Z' />
          </rows>
          <fields>
            <add name='Date1' type='datetime' t='specifyKind(unspecified)' />
            <add name='Date2' type='datetime' t='specifyKind(local)' />
            <add name='Date3' type='datetime' t='specifyKind(utc)' />
            <add name='Date4' type='datetime' />
            <add name='Date5' type='datetime' />
          </fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         var transform = new TransformHolder((c) => new SpecifyKindTransform(c), new SpecifyKindTransform().GetSignatures());

         using (var outer = new ConfigurationContainer(transform).CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(transform).CreateScope(process, logger)) {
               inner.Resolve<IProcessController>().Execute();
               var row = process.Entities.First().Rows.First();
               Assert.AreEqual(DateTimeKind.Unspecified, ((DateTime)row["Date1"]).Kind);
               Assert.AreEqual(DateTimeKind.Local, ((DateTime)row["Date2"]).Kind);
               Assert.AreEqual(DateTimeKind.Utc, ((DateTime)row["Date3"]).Kind);
               Assert.AreEqual(DateTimeKind.Unspecified, ((DateTime)row["Date4"]).Kind);
               Assert.AreEqual(DateTimeKind.Utc, ((DateTime)row["Date5"]).Kind);
            }
         }
      }
   }
}
