using System;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac;
using Transformalize.Providers.Trace;

namespace Tests {
    [TestClass]
    public class TestCalculatedFieldProcess {
        [TestMethod]
        public void TestOne() {
            const string main = @"
<cfg name='main'>
    <connections>
        <add name='input' provider='sqlserver' database='test' />
        <add name='output' provider='sqlserver' database='tflTest' />
    </connections>
    <entities>
        <add name='e1'>
            <fields>
                <add name='pk1' primary-key='true' />
                <add name='f1' />
                <add name='fk1' />
            </fields>
        </add>
        <add name='e2'>
            <fields>
                <add name='pk2' primary-key='true' />
                <add name='f2' />
            </fields>
        </add>
    </entities>
    <relationships>
        <add left-entity='e1' left-field='fk1' right-entity='e2' right-field='pk2' />
    </relationships>
    <calculated-fields>
        <add name='cf1' t='format({f1} {f2})' />
    </calculated-fields>
</cfg>
";

            var logger = new TraceLogger(LogLevel.Debug);

            using (var scope = ConfigurationContainer.Create(main, logger, new Dictionary<string, string>(), "@[]")) {
                var process = scope.Resolve<Process>();
                Assert.AreEqual(0, process.Errors().Length);
                Assert.AreEqual(true, process.CalculatedFields.First().Transforms.First().Method == "format");
                var cfp = process.ToCalculatedFieldsProcess();
                cfp.Check();
                Assert.AreEqual(0, cfp.Errors().Length);
                Assert.IsTrue(cfp.Entities.First().TryGetField("f1",out var f1),"f1 is not in the calculated entity");
                Assert.IsTrue(cfp.Entities.First().TryGetField("f2", out var f2),"f2 is not in the calculated entity");
                Console.WriteLine(cfp.Serialize());
            }

        }

    }
}