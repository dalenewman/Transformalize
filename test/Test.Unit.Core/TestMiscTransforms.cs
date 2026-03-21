#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2026 Dale Newman
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
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class TestMiscTransforms {

      [TestMethod]
      public void PrependWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='world' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).prepend(hello )' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("hello world", output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void LeftAndRightWork() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='abcdefgh' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).left(3)' />
            <add name='t2' t='copy(Field1).right(3)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("abc", output[0][cf[0]]);
               Assert.AreEqual("fgh", output[0][cf[1]]);
            }
         }
      }

      [TestMethod]
      public void PadLeftAndPadRightWork() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='42' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).padleft(5,0)' />
            <add name='t2' t='copy(Field1).padright(5,.)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("00042", output[0][cf[0]]);
               Assert.AreEqual("42...", output[0][cf[1]]);
            }
         }
      }

      [TestMethod]
      public void InsertAndRemoveWork() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='abcdef' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).insert(3,XY)' />
            <add name='t2' t='copy(Field1).remove(2,2)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("abcXYdef", output[0][cf[0]]);
               Assert.AreEqual("abef", output[0][cf[1]]);
            }
         }
      }

      [TestMethod]
      public void UpperAndLowerWork() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='Hello World' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).toupper()' />
            <add name='t2' t='copy(Field1).tolower()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("HELLO WORLD", output[0][cf[0]]);
               Assert.AreEqual("hello world", output[0][cf[1]]);
            }
         }
      }

      [TestMethod]
      public void ReverseWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='abc' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).split().reverse().join()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("cba", output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void HtmlEncodeAndDecodeWork() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='&lt;b&gt;bold&lt;/b&gt;' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).htmlencode()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("&lt;b&gt;bold&lt;/b&gt;", output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void UrlEncodeAndDecodeWork() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='hello world' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).urlencode()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("hello+world", output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void IsNumericWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='123' />
            <add Field1='abc' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' type='bool' t='copy(Field1).isnumeric()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(true, output[0][cf[0]]);
               Assert.AreEqual(false, output[1][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void IsEmptyWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='hello' />
            <add Field1='' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' type='bool' t='copy(Field1).isempty()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(false, output[0][cf[0]]);
               Assert.AreEqual(true, output[1][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void EndsWithWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='hello.com' />
            <add Field1='hello.org' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' type='bool' t='copy(Field1).endswith(.com)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(true, output[0][cf[0]]);
               Assert.AreEqual(false, output[1][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void RegexReplaceWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='abc 123 def 456' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).regexreplace([0-9]+,#)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("abc # def #", output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void FromSplitWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='a,b,c' />
          </rows>
          <fields>
            <add name='Field1'>
                <transforms>
                    <add method='fromsplit' separator=','>
                        <fields>
                            <add name='f1' />
                            <add name='f2' />
                            <add name='f3' />
                        </fields>
                    </add>
                </transforms>
            </add>
          </fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var fields = process.Entities.First().GetAllFields().Where(f => f.Name == "f1" || f.Name == "f2" || f.Name == "f3").ToArray();
               Assert.AreEqual("a", output[0][fields[0]]);
               Assert.AreEqual("b", output[0][fields[1]]);
               Assert.AreEqual("c", output[0][fields[2]]);
            }
         }
      }

      [TestMethod]
      public void MultiplyWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='5' Field2='3' />
          </rows>
          <fields>
            <add name='Field1' type='int' />
            <add name='Field2' type='int' />
          </fields>
          <calculated-fields>
            <add name='t1' type='int' t='copy(Field1,Field2).multiply()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(15, output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void CoalesceWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='' Field2='' Field3='found' />
          </rows>
          <fields>
            <add name='Field1' default='' />
            <add name='Field2' default='' />
            <add name='Field3' default='' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1,Field2,Field3).coalesce()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("found", output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void ToBoolWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='true' />
            <add Field1='false' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' type='bool' t='copy(Field1).tobool()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(true, output[0][cf[0]]);
               Assert.AreEqual(false, output[1][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void ToYesNoWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='true' />
            <add Field1='false' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' type='bool' t='copy(Field1).tobool()' />
            <add name='t2' t='copy(t1).toyesno()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("Yes", output[0][cf[1]]);
               Assert.AreEqual("No", output[1][cf[1]]);
            }
         }
      }

      [TestMethod]
      public void SplitLengthWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='a,b,c,d' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' type='int' t='copy(Field1).splitlength(\,)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(4, output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void HashCodeWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='hello' />
            <add Field1='hello' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' type='int' t='copy(Field1).hashcode()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(output[0][cf[0]], output[1][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void FirstAndLastWork() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='a,b,c' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).split(\,).first()' />
            <add name='t2' t='copy(Field1).split(\,).last()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("a", output[0][cf[0]]);
               Assert.AreEqual("c", output[0][cf[1]]);
            }
         }
      }

      [TestMethod]
      public void GetWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='a,b,c' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1).split(\,).get(1)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("b", output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void OppositeWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='5' />
          </rows>
          <fields>
            <add name='Field1' type='int' />
          </fields>
          <calculated-fields>
            <add name='t1' type='int' t='copy(Field1).opposite()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(-5, output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void InvertWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='true' />
          </rows>
          <fields>
            <add name='Field1' type='bool' />
          </fields>
          <calculated-fields>
            <add name='t1' type='bool' t='copy(Field1).invert()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(false, output[0][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void IsMatchWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='12345' />
            <add Field1='abc' />
          </rows>
          <fields>
            <add name='Field1' />
          </fields>
          <calculated-fields>
            <add name='t1' type='bool' t='copy(Field1).ismatch(^[0-9]+$)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual(true, output[0][cf[0]]);
               Assert.AreEqual(false, output[1][cf[0]]);
            }
         }
      }

      [TestMethod]
      public void ConcatWorks() {

         const string xml = @"
    <add name='TestProcess'>
      <entities>
        <add name='TestData' pipeline='linq'>
          <rows>
            <add Field1='a' Field2='b' Field3='c' />
          </rows>
          <fields>
            <add name='Field1' />
            <add name='Field2' />
            <add name='Field3' />
          </fields>
          <calculated-fields>
            <add name='t1' t='copy(Field1,Field2,Field3).concat()' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Info);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {
            var process = cfgScope.Resolve<Process>();
            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();
               var cf = process.Entities.First().CalculatedFields.ToArray();
               Assert.AreEqual("abc", output[0][cf[0]]);
            }
         }
      }
   }
}
