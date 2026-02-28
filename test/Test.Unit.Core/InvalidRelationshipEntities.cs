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
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Trace;

namespace Tests {

   [TestClass]
   public class InvalidRelationshipEntities {

      [TestMethod]
      public void IsValid() {
         const string xml = @"
    <add name='bsh'>
        <entities>
            <add name='t1'>
                <rows>
                    <add t1_id='1' t1_name='dalenewman' />
                    <add t1_id='2' t1_name='pywakit' />
                </rows>
                <fields>
                    <add name='t1_id' type='int' />
                    <add name='t1_name' />
                </fields>
            </add>
            <add name='t2'>
                <rows>
                    <add t2_id='1' t2_name='daryl' />
                    <add t2_id='2' t2_name='titanic' />
                </rows>
                <fields>
                    <add name='t2_id' type='int' />
                    <add name='t2_name' />
                </fields>
            </add>
        </entities>
        <relationships>
            <add left-entity='t1' left-field='t1_id' right-entity='t2' right-field='t2_id' />
        </relationships>
    </add>";

         var tracer = new TraceLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, tracer, null, "@[]")) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container().CreateScope(process, tracer)) {
               var context = inner.Resolve<IContext>();
               foreach (var error in process.Errors()) {
                  context.Error(error);
               }

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               Assert.AreEqual(0, process.Errors().Length);

            }
         }


      }

      [TestMethod]
      public void IsNotValid() {
         const string xml = @"
    <add name='bsh'>
        <entities>
            <add name='t1'>
                <rows>
                    <add t1_id='1' t1_name='dalenewman' />
                    <add t1_id='2' t1_name='pywakit' />
                </rows>
                <fields>
                    <add name='t1_id' type='int' />
                    <add name='t1_name' />
                </fields>
            </add>
            <add name='t2'>
                <rows>
                    <add t2_id='1' t2_name='daryl' />
                    <add t2_id='2' t2_name='titanic' />
                </rows>
                <fields>
                    <add name='t2_id' type='int' />
                    <add name='t2_name' />
                </fields>
            </add>
        </entities>
        <relationships>
            <add left-entity='eye' left-field='t1_id' right-entity='t2' right-field='t2_id' />
        </relationships>
    </add>";

         var tracer = new TraceLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, tracer, null, "@[]")) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container().CreateScope(process, tracer)) {
               var context = inner.Resolve<IContext>();
               foreach (var error in process.Errors()) {
                  context.Error(error);
               }

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               Assert.AreEqual(1, process.Errors().Length);
               Assert.AreEqual("The left entity eye does not exist in entities.", process.Errors()[0]);

            }
         }


      }


   }
}
