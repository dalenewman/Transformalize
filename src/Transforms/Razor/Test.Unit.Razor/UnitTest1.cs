using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Impl;
using Transformalize.Providers.Console;
using Transformalize.Providers.Internal;
using Transformalize.Transforms.Razor;

namespace Test.Unit {
   [TestClass]
   public class UnitTest1 {
      [TestMethod]
      public void TestMethod1() {

         var cfg = @"<cfg name='process' read-only='true'>
   <entities>
      <add name='entity'>
         <rows>
            <add FirstName='Dale' LastName='Newman' />
         </rows>
         <fields>
            <add name='FirstName' />
            <add name='LastName' />
         </fields>
         <calculated-fields>
            <add name='FullName'>
               <transforms>
                  <add method='razor' template='@{var fullName = Model.FirstName + "" "" + Model.LastName;}@fullName' >
                     <parameters>
                        <add field='FirstName' />
                        <add field='LastName' />
                     </parameters>
                  </add>
               </transforms>
            </add>
         </calculated-fields>
      </add>
   </entities>
</cfg>";
         var process = new Process(cfg);

         Assert.AreEqual(0, process.Errors().Length);

         // manually build out test without autofac to make it more of a unit test
         var logger = new ConsoleLogger();
         var entity = process.Entities.First();
         var field = entity.CalculatedFields.First();
         var operation = field.Transforms.First();
         var context = new PipelineContext(logger, process, entity, field, operation);
         var transform = new RazorTransform(context);
         var input = new InputContext(context);
         var reader = new InternalReader(input, new RowFactory(input.RowCapacity, entity.IsMaster, false));

         var rows = transform.Operate(reader.Read()).ToArray();

         Assert.AreEqual("Dale Newman", rows[0][field]);

      }

   }
}
