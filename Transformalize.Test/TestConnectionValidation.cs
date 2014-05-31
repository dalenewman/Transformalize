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

using System.Linq;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation;

namespace Transformalize.Test {
    [TestFixture]
    public class TestConnectionValidation {

        [Test]
        public void TestValidConnectionString() {
            var c = new ConnectionConfigurationElement {
                ConnectionString = "server=localhost;database=NorthWind;trusted_connection=True;"
            };

            var validator = ValidationFactory.CreateValidator<ConnectionConfigurationElement>();
            var results = validator.Validate(c);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void TestValidDatabase() {
            var c = new ConnectionConfigurationElement {
                Database = "NorthWind"
            };

            var validator = ValidationFactory.CreateValidator<ConnectionConfigurationElement>();
            var results = validator.Validate(c);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void TestValidFile() {
            var c = new ConnectionConfigurationElement { Provider = "File", File = @"c:\temp\log.txt" };

            var validator = ValidationFactory.CreateValidator<ConnectionConfigurationElement>();

            var results = validator.Validate(c);

            Assert.IsTrue(results.IsValid);
        }

        [Test]
        public void TestNoFile() {
            var c = new ConnectionConfigurationElement { Provider = "File" };

            var validator = ValidationFactory.CreateValidator<ConnectionConfigurationElement>();

            var results = validator.Validate(c);

            Assert.IsFalse(results.IsValid);
            Assert.AreEqual("The File provider requires the File property setting.", results.First().Message);
        }

    }
}