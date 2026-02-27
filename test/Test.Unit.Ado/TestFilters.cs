using Autofac;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Context;
using Transformalize.Providers.Ado;
using Transformalize.Providers.Ado.Autofac;
using Transformalize.Providers.Ado.Ext;
using Transformalize.Providers.Console;

namespace Test.Unit {

   [TestClass]
   public class TestFilters {

      [TestMethod]
      public void IgnoreAsterisk() {
         const string xml = @"
   <cfg name='Test'>
      <connections>
         <add name='input' provider='sqlserver' server='localhost' database='junk' />
      </connections>
      <parameters>
         <add name='f2' value='*' prompt='true' />
      </parameters>
      <entities>
         <add name='Fact'>
            <filter>
               <add field='f2' value='@[f2]' type='facet' />
               <add expression='1=2' />
            </filter>
            <fields>
               <add name='f1' type='int' primary-key='true' />
               <add name='f2' />
               <add name='d1' type='int' />
            </fields>
         </add>
      </entities>
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

            using(var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var actual = context.SqlSelectInput(context.Entity.GetAllFields().Where(f => f.Input).ToArray(), new NullConnectionFactory());
               Assert.AreEqual("SELECT f1,f2,d1 FROM Fact WHERE (1=2)", actual);
            }

         }

      }

      [TestMethod]
      public void IgnoreAsterisk2() {
         const string xml = @"<cfg name='name' mode='report'>
  <parameters>
    <add name='Stars' value='2' prompt='true' />
    <add name='FirstName' value='*' prompt='true' multiple='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Junk' />
  </connections>
  <entities>
    <add name='BogusStar'>
      <filter>
          <add field='Stars' value='@[Stars]' type='facet' />
          <add field='FirstName' operator='in' value='@[FirstName]' type='facet' />
      </filter>
      <fields>
        <add name='Identity' primary-key='true' type='int' />
        <add name='FirstName' alias='FirstName' label='FirstName' sortfield='FirstName' sortable='true' />
        <add name='LastName' alias='LastName' label='LastName' sortfield='LastName' sortable='true' />
        <add name='Stars' type='byte' alias='Stars' label='Stars' sortfield='Stars' sortable='true' />
        <add name='Reviewers' type='int' alias='Reviewers' label='Reviewers' sortfield='Reviewers' sortable='true'/>
      </fields>
    </add>
  </entities>
</cfg>";
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("nameBogusStar");
               var actual = context.SqlSelectInput(context.Entity.GetAllFields().Where(f => f.Input).ToArray(), new NullConnectionFactory());
               Assert.AreEqual("SELECT Identity,FirstName,LastName,Stars,Reviewers FROM BogusStar WHERE (Stars = 2)", actual);
            }

         }

      }

      [TestMethod]
      public void IgnoreSearchAsterisk() {
         const string xml = @"<cfg name='name'>
  <parameters>
    <add name='FirstName' value='*' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Junk' />
  </connections>
  <entities>
    <add name='BogusStar'>
      <filter>
          <add field='FirstName' value='@[FirstName]' type='search' />
      </filter>
      <fields>
        <add name='Identity' primary-key='true' type='int' />
        <add name='FirstName' alias='FirstName' label='FirstName' sortfield='FirstName' sortable='true' />
        <add name='LastName' alias='LastName' label='LastName' sortfield='LastName' sortable='true' />
        <add name='Stars' type='byte' alias='Stars' label='Stars' sortfield='Stars' sortable='true' />
        <add name='Reviewers' type='int' alias='Reviewers' label='Reviewers' sortfield='Reviewers' sortable='true'/>
      </fields>
    </add>
  </entities>
</cfg>";
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("nameBogusStar");
               var actual = context.SqlSelectInput(context.Entity.GetAllFields().Where(f => f.Input).ToArray(), new NullConnectionFactory());
               Assert.AreEqual("SELECT Identity,FirstName,LastName,Stars,Reviewers FROM BogusStar", actual);
            }

         }

      }

      [TestMethod]
      public void HandleSearchWithoutAsterisk() {
         const string xml = @"<cfg name='name'>
  <parameters>
    <add name='FirstName' value='Dale' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Junk' />
  </connections>
  <entities>
    <add name='BogusStar'>
      <filter>
          <add field='FirstName' value='@[FirstName]' type='search' />
      </filter>
      <fields>
        <add name='Identity' primary-key='true' type='int' />
        <add name='FirstName' alias='FirstName' label='FirstName' sortfield='FirstName' sortable='true' />
        <add name='LastName' alias='LastName' label='LastName' sortfield='LastName' sortable='true' />
        <add name='Stars' type='byte' alias='Stars' label='Stars' sortfield='Stars' sortable='true' />
        <add name='Reviewers' type='int' alias='Reviewers' label='Reviewers' sortfield='Reviewers' sortable='true'/>
      </fields>
    </add>
  </entities>
</cfg>";
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("nameBogusStar");
               var actual = context.SqlSelectInput(context.Entity.GetAllFields().Where(f => f.Input).ToArray(), new NullConnectionFactory());
               Assert.AreEqual("SELECT Identity,FirstName,LastName,Stars,Reviewers FROM BogusStar WHERE (FirstName LIKE '%Dale%')", actual);
            }

         }

      }

      [TestMethod]
      public void HandleSearchWithoutTrailingAsterisk() {
         const string xml = @"<cfg name='name'>
  <parameters>
    <add name='FirstName' value='Dale*' prompt='true' />
  </parameters>
  <connections>
    <add name='input' provider='sqlserver' server='localhost' database='Junk' />
  </connections>
  <entities>
    <add name='BogusStar'>
      <filter>
          <add field='FirstName' value='@[FirstName]' type='search' />
          <add expression='1=1' />
      </filter>
      <fields>
        <add name='Identity' primary-key='true' type='int' />
        <add name='FirstName' alias='FirstName' label='FirstName' sortfield='FirstName' sortable='true' />
        <add name='LastName' alias='LastName' label='LastName' sortfield='LastName' sortable='true' />
        <add name='Stars' type='byte' alias='Stars' label='Stars' sortfield='Stars' sortable='true' />
        <add name='Reviewers' type='int' alias='Reviewers' label='Reviewers' sortfield='Reviewers' sortable='true'/>
      </fields>
    </add>
  </entities>
</cfg>";
         var logger = new ConsoleLogger();
         using (var outer = new ConfigurationContainer().CreateScope(xml, logger)) {

            // get and test process
            var process = outer.Resolve<Process>();
            foreach (var error in process.Errors()) {
               Console.WriteLine(error);
            }
            Assert.AreEqual(0, process.Errors().Length);

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("nameBogusStar");
               var actual = context.SqlSelectInput(context.Entity.GetAllFields().Where(f => f.Input).ToArray(), new NullConnectionFactory());
               Assert.AreEqual("SELECT Identity,FirstName,LastName,Stars,Reviewers FROM BogusStar WHERE (FirstName LIKE 'Dale%' AND 1=1)", actual);
            }

         }

      }


      [TestMethod]
      public void NoFilters() {
         const string xml = @"
   <cfg name='Test'>
      <connections>
         <add name='input' provider='sqlserver' server='localhost' database='junk' />
      </connections>
      <parameters>
         <add name='f2' value='*' prompt='true' />
      </parameters>
      <entities>
         <add name='Fact'>
            <filter>
               <add field='f2' value='@[f2]' type='facet' />
            </filter>
            <fields>
               <add name='f1' type='int' primary-key='true' />
               <add name='f2' />
               <add name='d1' type='int' />
            </fields>
         </add>
      </entities>
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

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var actual = context.SqlSelectInput(context.Entity.GetAllFields().Where(f => f.Input).ToArray(), new NullConnectionFactory());
               Assert.AreEqual("SELECT f1,f2,d1 FROM Fact", actual);
            }

         }

      }

      [TestMethod]
      public void HandleMultipleStrings() {
         const string xml = @"
   <cfg name='Test'>
      <connections>
         <add name='input' provider='sqlserver' server='localhost' database='junk' />
      </connections>
      <parameters>
         <add name='f2' value='v1,v2' prompt='true' />
      </parameters>
      <entities>
         <add name='Fact'>
            <filter>
               <add field='f2' operator='in' value='@[f2]' />
            </filter>
            <fields>
               <add name='f1' type='int' primary-key='true' />
               <add name='f2' />
               <add name='d1' type='int' />
            </fields>
         </add>
      </entities>
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

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var actual = context.SqlSelectInput(context.Entity.GetAllFields().Where(f => f.Input).ToArray(), new NullConnectionFactory());
               Assert.AreEqual("SELECT f1,f2,d1 FROM Fact WHERE (f2 IN ('v1','v2'))", actual);
            }

         }

      }

      [TestMethod]
      public void HandleMultipleNumbers() {
         const string xml = @"
   <cfg name='Test'>
      <connections>
         <add name='input' provider='sqlserver' server='localhost' database='junk' />
      </connections>
      <parameters>
         <add name='f2' value='1,2' prompt='true' />
      </parameters>
      <entities>
         <add name='Fact'>
            <filter>
               <add field='f2' operator='in' value='@[f2]' />
            </filter>
            <fields>
               <add name='f1' type='int' primary-key='true' />
               <add name='f2' type='double' />
               <add name='d1' type='int' />
            </fields>
         </add>
      </entities>
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

            using (var inner = new Container(new AdoProviderModule()).CreateScope(process, logger)) {
               var context = inner.ResolveNamed<InputContext>("TestFact");
               var actual = context.SqlSelectInput(context.Entity.GetAllFields().Where(f => f.Input).ToArray(), new NullConnectionFactory());
               Assert.AreEqual("SELECT f1,f2,d1 FROM Fact WHERE (f2 IN (1,2))", actual);
            }

         }

      }

   }
}
