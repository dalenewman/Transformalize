using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Providers.Ado;
using Transformalize.Providers.Console;

namespace Test.Unit {
   [TestClass]
   public class TestAdoUpdateMasterKeysQueryWriter {
      [TestMethod]
      public void CheckUpdateMasterQuery() {
         const string xml = @"
   <cfg name='Test'>
      <entities>
         <add name='Fact'>
            <fields>
               <add name='f1' type='int' primary-key='true' />
               <add name='f2' />
               <add name='d1' type='int' />
            </fields>
         </add>
         <add name='Dim'>
            <fields>
               <add name='d1' type='int' primary-key='true' />
               <add name='d2' />
               <add name='sd1' type='int' />
            </fields>
         </add>
         <add name='SubDim'>
            <fields>
               <add name='sd1' type='int' primary-key='true' />
               <add name='sd2' />
            </fields>
         </add>
      </entities>
      <relationships>
         <add left-entity='Fact' left-field='d1' right-entity='Dim' right-field='d1' />
         <add left-entity='Dim' left-field='sd1' right-entity='SubDim' right-field='sd1' />
      </relationships>
   </cfg>
";
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            // setup test
            var fact = process.Entities[0];
            var dimension = process.Entities[1];

            fact.BatchId = 1;
            dimension.BatchId = 2;
            
            var context = new PipelineContext(logger, process, dimension);
            var writer = new AdoUpdateMasterKeysQueryWriter(context, new NullConnectionFactory());
            var entityStatus = new Transformalize.EntityStatus(context) {
               Modified = true,
               MasterUpserted = true,
               HasForeignKeys = true
            };

            var expected = @"UPDATE A
SET A.B7 = B.B7,
A.A2 = @TflBatchId
 FROM TestFactTable A INNER JOIN TestDimTable B ON ( A.A7 = B.B5)
WHERE B.B2 = @TflBatchId 
OR A.A2 >= @MasterTflBatchId
";
            var actual = writer.Write(entityStatus);

            Assert.AreEqual(expected, actual);

         }

      }
   }
}
