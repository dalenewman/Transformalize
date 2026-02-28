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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests {
   [TestClass]
   public class PaginationTest {

      [TestMethod]
      public void TestPage1of100() {
         var actual = new Transformalize.Configuration.Pagination(1000, 1, 100);
         Assert.AreEqual(1, actual.StartRow);
         Assert.AreEqual(100, actual.EndRow);
         Assert.AreEqual(10, actual.Pages);
         Assert.AreEqual(100, actual.Size);
         Assert.AreEqual(1, actual.Previous);
         Assert.AreEqual(2, actual.Next);
         Assert.AreEqual(1, actual.First);
         Assert.AreEqual(10, actual.Last);
         Assert.IsFalse(actual.HasPrevious);
         Assert.IsTrue(actual.HasNext);
      }

      [TestMethod]
      public void TestPage2of100() {
         var actual = new Transformalize.Configuration.Pagination(1000, 2, 100);
         Assert.AreEqual(101, actual.StartRow);
         Assert.AreEqual(200, actual.EndRow);
         Assert.AreEqual(10, actual.Pages);
         Assert.AreEqual(100, actual.Size);
         Assert.AreEqual(1, actual.Previous);
         Assert.AreEqual(3, actual.Next);
         Assert.AreEqual(1, actual.First);
         Assert.AreEqual(10, actual.Last);
         Assert.IsTrue(actual.HasPrevious);
         Assert.IsTrue(actual.HasNext);
      }

      [TestMethod]
      public void TestPage10of100() {
         var actual = new Transformalize.Configuration.Pagination(1000, 10, 100);
         Assert.AreEqual(901, actual.StartRow);
         Assert.AreEqual(1000, actual.EndRow);
         Assert.AreEqual(10, actual.Pages);
         Assert.AreEqual(100, actual.Size);
         Assert.AreEqual(9, actual.Previous);
         Assert.AreEqual(10, actual.Next);
         Assert.AreEqual(1, actual.First);
         Assert.AreEqual(10, actual.Last);
         Assert.IsTrue(actual.HasPrevious);
         Assert.IsFalse(actual.HasNext);
      }

      [TestMethod]
      public void TestPage1of11() {
         var actual = new Transformalize.Configuration.Pagination(1030, 1, 100);
         Assert.AreEqual(1, actual.StartRow);
         Assert.AreEqual(100, actual.EndRow);
         Assert.AreEqual(11, actual.Pages);
         Assert.AreEqual(100, actual.Size);
         Assert.AreEqual(1, actual.Previous);
         Assert.AreEqual(2, actual.Next);
         Assert.AreEqual(1, actual.First);
         Assert.AreEqual(11, actual.Last);
         Assert.IsFalse(actual.HasPrevious);
         Assert.IsTrue(actual.HasNext);
      }

      [TestMethod]
      public void TestPage10of11() {
         var actual = new Transformalize.Configuration.Pagination(1030, 10, 100);
         Assert.AreEqual(901, actual.StartRow);
         Assert.AreEqual(1000, actual.EndRow);
         Assert.AreEqual(11, actual.Pages);
         Assert.AreEqual(100, actual.Size);
         Assert.AreEqual(9, actual.Previous);
         Assert.AreEqual(11, actual.Next);
         Assert.AreEqual(1, actual.First);
         Assert.AreEqual(11, actual.Last);
         Assert.IsTrue(actual.HasPrevious);
         Assert.IsTrue(actual.HasNext);
      }

      [TestMethod]
      public void TestPage11of11() {
         var actual = new Transformalize.Configuration.Pagination(1030, 11, 100);
         Assert.AreEqual(1001, actual.StartRow);
         Assert.AreEqual(1030, actual.EndRow);
         Assert.AreEqual(11, actual.Pages);
         Assert.AreEqual(30, actual.Size);
         Assert.AreEqual(10, actual.Previous);
         Assert.AreEqual(11, actual.Next);
         Assert.AreEqual(1, actual.First);
         Assert.AreEqual(11, actual.Last);
         Assert.IsTrue(actual.HasPrevious);
         Assert.IsFalse(actual.HasNext);
      }

      [TestMethod]
      public void TestPage11of11Odd() {
         var actual = new Transformalize.Configuration.Pagination(1033, 11, 100);
         Assert.AreEqual(1001, actual.StartRow);
         Assert.AreEqual(1033, actual.EndRow);
         Assert.AreEqual(11, actual.Pages);
         Assert.AreEqual(33, actual.Size);
         Assert.AreEqual(10, actual.Previous);
         Assert.AreEqual(11, actual.Next);
         Assert.AreEqual(1, actual.First);
         Assert.AreEqual(11, actual.Last);
         Assert.IsTrue(actual.HasPrevious);
         Assert.IsFalse(actual.HasNext);
      }

   }
}
