#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
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
using System.Linq;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.Elasticsearch.Autofac;

namespace Test.Integration.Core {

   [TestClass]
   [Ignore]
   public class Test {

      // note: these credentials are specific to the container running on dale's computer
      private static string Version = "8.3.2";
      private static string User = "elastic";
      private static string Password = "1JN_8oISzY1d-T=P9iHF";  
      private static string Fingerprint = "DA:35:81:AA:E4:02:75:31:CE:22:A3:25:E7:54:26:49:20:B2:8C:73:1C:AD:14:B0:58:9B:F8:3E:3F:1B:B9:31";

      [TestMethod]
      public void Write() {
         string xml = $@"<add name='TestProcess' mode='init'>
  <parameters>
    <add name='Size' type='int' value='1000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' 
         provider='elasticsearch' 
         server='localhost' 
         index='bogus' 
         shards='3' 
         replicas='0' 
         port='9200'
         useSsl='true'
         user='{User}'
         password='{Password}'
         certificate-fingerprint='{Fingerprint}'
         version='{Version}' />
  </connections>
  <entities>
    <add name='Contact' size='@[Size]'>
      <fields>
        <add name='Identity' type='int' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
      <calculated-fields>
         <add name='Names' t='copy(FirstName,LastName).toArray()' />
      </calculated-fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);

         using (var x = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = x.Resolve<Process>();
            using (var y = new Container(new BogusModule(), new ElasticsearchModule()).CreateScope(process, logger)) {
               y.Resolve<IProcessController>().Execute();
               Assert.AreEqual(process.Entities.First().Inserts, (uint)1000);
            }
         }
      }

      [TestMethod]
      public void Read() {
         string xml = $@"<add name='TestProcess'>
  <connections>
    <add name='input' provider='elasticsearch' server='localhost' index='bogus' port='9200' useSsl='true' version='{Version}' user='{User}' password='{Password}' certificate-fingerprint='{Fingerprint}' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='contact'>
      <fields>
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
        <add name='names' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new ElasticsearchModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(1000, rows.Count);
            }
         }
      }

      [TestMethod]
      public void ReadPage1() {
         string xml = $@"<add name='TestProcess'>
  <connections>
    <add name='input' provider='elasticsearch' server='localhost' index='bogus' port='9200' version='{Version}' useSsl='true' user='{User}' password='{Password}' certificate-fingerprint='{Fingerprint}' scroll='30s' />
  </connections>
  <entities>
    <add name='contact' page='1' size='10'>
      <order>
         <add field='identity' />
      </order>
      <fields>
        <add name='identity' type='int' />
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
        <add name='names' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new ElasticsearchModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(10, rows.Count);
               Assert.AreEqual("Justin", rows[0]["firstname"]);
            }
         }
      }

      [TestMethod]
      public void ReadPage2() {
         string xml = $@"<add name='TestProcess'>
  <connections>
    <add name='input' provider='elasticsearch' server='localhost' index='bogus' port='9200' version='{Version}' useSsl='true' user='{User}' password='{Password}' certificate-fingerprint='{Fingerprint}' scroll='30s' />
  </connections>
  <entities>
    <add name='contact' page='2' size='5'>
      <order>
         <add field='identity' />
      </order>
      <fields>
        <add name='identity' type='int' />
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
        <add name='names' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new ElasticsearchModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(5, rows.Count);
               Assert.AreEqual("Mitchell", rows[0]["firstname"]);
            }
         }
      }

   }
}
