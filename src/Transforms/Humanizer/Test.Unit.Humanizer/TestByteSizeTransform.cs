using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;
using Transformalize.Transforms.Humanizer.Autofac;

namespace UnitTests {
   [TestClass]
   public class TestByteSizeTransform {

      [TestMethod]
      public void ByteSize() {

         var logger = new ConsoleLogger(LogLevel.Debug);

         var xml = $@"
<add name='TestProcess' read-only='false'>
    <entities>
        <add name='TestData'>
            <rows>
                <add Number='128' />
            </rows>
            <fields>
                <add name='Number' type='int' />
            </fields>
            <calculated-fields>
                <add name='Bits' t='copy(Number).bits().humanize()' />
                <add name='Bytes' t='copy(Number).bytes().humanize()' />
                <add name='Kilobytes' t='copy(Number).kilobytes().humanize()' />
                <add name='Megabytes' t='copy(Number).megabytes().humanize()' />
                <add name='Gigabytes' t='copy(Number).gigabytes().humanize()' />
                <add name='Terabytes' t='copy(Number).terabytes().humanize()' />

                <add name='BitsFormat' t='copy(Number).bits().humanize(b)' />
                <add name='BytesFormat' t='copy(Number).bytes().humanize(b)' />
                <add name='KilobytesFormat' t='copy(Number).kilobytes().humanize(B)' />
                <add name='MegabytesFormat' t='copy(Number).megabytes().humanize(KB)' />
                <add name='GigabytesFormat' t='copy(Number).gigabytes().humanize(MB)' />
                <add name='TerabytesFormat' t='copy(Number).terabytes().humanize(GB)' />

                <add name='bs1' type='double' t='copy(Number).append( MB).bytesize(KB)' />
                <add name='bs2' type='double' t='copy(Number).megabytes().bytesize(KB)' />
            </calculated-fields>
        </add>
    </entities>

</add>";
         using (var outer = new ConfigurationContainer(new HumanizeModule()).CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new HumanizeModule()).CreateScope(process, logger)) {

               var cf = process.Entities.First().CalculatedFields;

               var bits = cf[0];
               var bytes = cf[1];
               var kilobytes = cf[2];
               var megabytes = cf[3];
               var gigabytes = cf[4];
               var terabytes = cf[5];
               var bitsF = cf[6];
               var bytesF = cf[7];
               var kilobytesF = cf[8];
               var megabytesF = cf[9];
               var gigabytesF = cf[10];
               var terabytesF = cf[11];
               var bs1 = cf[12];
               var bs2 = cf[13];

               var controller = inner.Resolve<IProcessController>();
               var rows = controller.Read().ToArray();

               Assert.AreEqual("16 B", rows[0][bits]);
               Assert.AreEqual("128 B", rows[0][bytes]);
               Assert.AreEqual("128 KB", rows[0][kilobytes]);
               Assert.AreEqual("128 MB", rows[0][megabytes]);
               Assert.AreEqual("128 GB", rows[0][gigabytes]);
               Assert.AreEqual("128 TB", rows[0][terabytes]);
               Assert.AreEqual("128 b", rows[0][bitsF]);
               Assert.AreEqual("1024 b", rows[0][bytesF]);
               Assert.AreEqual("131072 B", rows[0][kilobytesF]);
               Assert.AreEqual("131072 KB", rows[0][megabytesF]);
               Assert.AreEqual("131072 MB", rows[0][gigabytesF]);
               Assert.AreEqual("131072 GB", rows[0][terabytesF]);

               Assert.AreEqual(131072d, rows[0][bs1], "append MB to 128 getting 128 MB parsing that into a bytesize and asking for translation to KB as a number");
               Assert.AreEqual(131072d, rows[0][bs2], "treating 128 as megabytes and asking for translation to KB as a number");

            }
         }

      }
   }
}
