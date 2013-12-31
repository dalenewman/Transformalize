#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using NUnit.Framework;
using Transformalize.Main;

namespace Transformalize.Test.Unit
{
    [TestFixture]
    public class TestConfiguration
    {
        [Test]
        public void TestBase()
        {
            var northWind = ProcessFactory.Create("NorthWind.xml");
            Assert.AreEqual("System.Int16", northWind.Entities[0].Fields["OrderDetailsQuantity"].Type);
            Assert.AreEqual(8, northWind.Entities.Count);
            Assert.AreEqual(3, northWind.Entities[1].CalculatedFields.Count);
        }

        [Test]
        public void TestSimpleExpansion() {
            var northWind = ProcessFactory.Create("NorthWindExpanded.xml");
            Assert.AreEqual("System.Int32", northWind.Entities[0].Fields["OrderDetailsQuantity"].Type);
            Assert.AreEqual(8, northWind.Entities.Count);
            Assert.AreEqual(4, northWind.Entities[1].CalculatedFields.Count);
        }
    }
}