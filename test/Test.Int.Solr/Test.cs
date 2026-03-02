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
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Bogus.Autofac;
using Transformalize.Providers.Console;
using Transformalize.Providers.Solr.Autofac;

namespace IntegrationTests {

   [TestClass]
   public class Test {

      [TestMethod]
      public void UsefulTest(){
         Assert.AreEqual(1,1, "Try this for Github Actions");
      }

      [TestMethod]
      [Ignore]
      public void Write773() {
         const string xml = @"<add name='TestProcess' mode='init'>
  <parameters>
    <add name='Size' type='int' value='1000' />
    <add name='MDOP' type='int' value='2' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='solr' core='bogus' server='localhost' folder='c:\temp\mysolrhome' version='7.7.3' path='solr' port='8983' max-degree-of-parallelism='@[MDOP]' request-timeout='100' />
  </connections>
  <entities>
    <add name='Contact' size='@[Size]' insert-size='255'>
      <fields>
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Info);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger, new Dictionary<string, string>() { { "MDOP", "2" } })) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new SolrModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               Assert.AreEqual(process.Entities.First().Inserts, (uint)1000);
            }
         }
      }

      [TestMethod]
      [Ignore]
      public void Read773() {
         const string xml = @"<add name='TestProcess'>
  <connections>
    <add name='input' provider='solr' core='bogus' server='localhost' folder='c:\temp\mysolrhome' path='solr' port='8983' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Contact'>
      <fields>
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new SolrModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(1000, rows.Count);

            }
         }
      }

      [TestMethod]
      [Ignore]
      public void Write621() {
         const string xml = @"<add name='TestProcess' mode='init'>
  <parameters>
    <add name='Size' type='int' value='100000' />
  </parameters>
  <connections>
    <add name='input' provider='bogus' seed='1' />
    <add name='output' provider='solr' core='bogus' folder='d:\java\solr-6.2.1\cores' path='solr' port='8983' version='6.2.1' />
  </connections>
  <entities>
    <add name='Contact' size='@[Size]' insert-size='1000'>
      <fields>
        <add name='Identity' type='int' primary-key='true' />
        <add name='FirstName' />
        <add name='LastName' />
        <add name='Stars' type='byte' min='1' max='5' />
        <add name='Reviewers' type='int' min='0' max='500' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new SolrModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               Assert.AreEqual(process.Entities.First().Inserts, (uint)100000);
            }
         }
      }

      [TestMethod]
      [Ignore]
      public void WriteDates621() {
         const string xml = @"<add name='TestProcess' mode='init'>
  <connections>
    <add name='input' provider='internal' seed='1' />
    <add name='output' provider='solr' core='dates' folder='d:\java\solr-6.2.1\cores' path='solr' port='8983' />
  </connections>
  <entities>
    <add name='dates'>
      <rows>
         <add string='2019-05-02 13:00:00' date='2019-05-02 13:00:00' dateoffset='2019-05-02 13:00:00-04:00' datez='2019-05-02 13:00:00Z' />
         <add string='2019-05-03 13:00:00' date='2019-05-03 13:00:00' dateoffset='2019-05-03 13:00:00-04:00' datez='2019-05-03 13:00:00Z' />
      </rows>
      <fields>
        <add name='string' type='string' primary-key='true' />
        <add name='date' type='datetime' />
        <add name='dateoffset' type='datetime' />
        <add name='datez' type='datetime' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new SolrModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();

               Assert.AreEqual(process.Entities.First().Inserts, (uint)2);
            }
         }
      }


      [TestMethod]
      [Ignore]
      public void Read621FastPaging() {
         const string xml = @"<add name='TestProcess' read-only='true'>
  <connections>
    <add name='input' provider='solr' core='bogus' folder='d:\java\solr-6.2.1\cores' path='solr' port='8983' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Contact'>
      <fields>
        <add name='identity' type='int' primary-key='true' output='false' />
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new SolrModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(100000, rows.Count);


            }
         }
      }

      [TestMethod]
      [Ignore]
      public void Read621SlowPaging() {
         const string xml = @"<add name='TestProcess' read-only='true'>
  <connections>
    <add name='input' provider='solr' core='bogus' folder='d:\java\solr-6.2.1\cores' path='solr' port='8983' version='4.6' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Contact'>
      <fields>
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new SolrModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(100000, rows.Count);
            }
         }
      }

      [TestMethod]
      [Ignore]
      public void ReadWithExpression621() {
         const string xml = @"<add name='TestProcess'>
  <connections>
    <add name='input' provider='solr' core='bogus' server='localhost' path='solr' port='8983' version='6.2.1' />
    <add name='output' provider='internal' />
  </connections>
  <entities>
    <add name='Contact'>
      <filter>
         <add expression='firstname:Justin AND lastname:K*' type='search' />
      </filter>
      <fields>
        <add name='firstname' />
        <add name='lastname' />
        <add name='stars' type='byte' />
        <add name='reviewers' type='int' />
      </fields>
    </add>
  </entities>
</add>";
         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = outer.Resolve<Process>();
            using (var inner = new Container(new BogusModule(), new SolrModule()).CreateScope(process, logger)) {

               var controller = inner.Resolve<IProcessController>();
               controller.Execute();
               var rows = process.Entities.First().Rows;

               Assert.AreEqual(8, rows.Count);

            }
         }
      }
   }
}
