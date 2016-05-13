using System;
using System.Linq;
using Autofac;
using NUnit.Framework;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Ioc.Autofac;
using Pipeline.Ioc.Autofac.Modules;
using Pipeline.Logging;

namespace Pipeline.Test {
    public class TestQuery {

        [Test]
        [Ignore("Requires SQL Server")]
        public void Test1() {

            const string xml = @"
<add name='p1' mode='meta'>
    <connections>
        <add name='input' provider='sqlserver' server='localhost' database='NorthWindStar' />
    </connections>
    <entities>
        <add name='e1' query='select OrderDetailsDiscount, OrderDetailsOrderID from NorthWindStar;' />
    </entities>
</add>";

            var builder = new ContainerBuilder();

            builder.Register<ISchemaReader>((ctx, p) => {
                var process = p.TypedAs<Process>();
                var context = ctx.IsRegisteredWithName<IContext>(process.Key) ? ctx.ResolveNamed<IContext>(process.Key) : new PipelineContext(new NullLogger(), process);
                return new RunTimeSchemaReader(process, context);
            }).As<ISchemaReader>();

            builder.RegisterModule(new RootModule("Shorthand.xml"));

            using (var scope = builder.Build().BeginLifetimeScope()) {
                var process = scope.Resolve<Process>(new NamedParameter("cfg", xml));

                if (process.Errors().Any()) {
                    foreach (var error in process.Errors()) {
                        System.Diagnostics.Trace.WriteLine(error);
                    }
                    throw new Exception(string.Join(System.Environment.NewLine, process.Errors()));
                }

                var reader = new RunTimeDataReader(new DebugLogger(LogLevel.Debug));
                var rows = reader.Run(process).ToArray();

                Assert.AreEqual(2155, rows.Length);
            }
        }

        [Test]
        public void Test2()
        {

            const string cfg = @"
<cfg name='query'>
    <connections>
        <add name='input' provider='sqlserver' server='SBSQL1.mwf.local' database='ClevestDukeCarolina' />
        <add name='output' provider='console' />
    </connections>
    <entities>
        <add name='query' query=""
select
	oh.Id, 
	oh.OrderKey, 
	oh.DataXml
from OrderHistory oh WITH (NOLOCK)
inner join WorkOrder wo WITH (NOLOCK) on (oh.OrderKey = wo.WorkOrderKey)
inner join OrderCategory oc WITH (NOLOCK) on (wo.CategoryKey = oc.OrderCategoryKey)
inner join EventType et WITH (NOLOCK) ON (oh.EventTypeKey = et.EventTypeKey)
where oc.Name = 'UTC'
AND et.Name IN ('UpdateEntity','UpdateEntityByApi')
"">
            <fields>
                <add name='Id' type='long' primary-key='true' />
                <add name='OrderKey' type='guid' />
                <add name='DataXml' length='max' output='false'>
                    <transforms>    
                        <add method='fromxml'>
                            <fields>
                                <add name='GZipCompression' length='max' output='false' />
                            </fields>
                        </add>
                    </transforms>
                </add>
            </fields>
            <calculated-fields>
                <add name='decompressed' t='copy(GZipCompression).decompress()' length='max' output='false' />
                <add name='CategoryKey' t='copy(decompressed).xpath(/WorkOrder/c\:CategoryKey,c,http\://www.mobilefieldforce.com/)' type='guid' />
            </calculated-fields>
        </add>
    </entities>
</cfg>
";
            var composer = new CompositionRoot();
            var controller = composer.Compose(cfg);
            controller.Execute();


        }

    }
}
