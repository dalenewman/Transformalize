/*
Transformalize - Replicate, Transform, and Denormalize Your Data...
Copyright (C) 2013 Dale Newman

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Core.Parameters_;
using Transformalize.Core.Process_;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class TestNorthConfiguration : EtlProcessHelper
    {
        private readonly Process _process = new ProcessReader("NorthWind").Read();

        [Test]
        public void TestRelatedKeys() {

            Assert.AreEqual(7, _process.RelatedKeys.Count());

            foreach (var relatedKey in _process.RelatedKeys)
            {
                Console.WriteLine("{0} : {1}", relatedKey.Entity, relatedKey.Name);
            }

            Assert.AreEqual(0, Process.Entities.First().RelationshipToMaster.Count());

            Assert.AreEqual(1, Process.Entities.First(e => e.Alias.Equals("Products")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Customers")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Employees")).RelationshipToMaster.Count());

            Assert.AreEqual(1, Process.Entities.First(e => e.Alias.Equals("Orders")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Categories")).RelationshipToMaster.Count());
            Assert.AreEqual(2, Process.Entities.First(e => e.Alias.Equals("Suppliers")).RelationshipToMaster.Count());

        }

    }
}
