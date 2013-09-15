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
using Transformalize.Runner;
using Transformalize.Libs.Dapper;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class NorthWindUpdates {
        [Test]
        public void Go() {
            // initialize (destroy and create)
            var options = new Options { Mode = Modes.Initialize};
            var process = new ProcessReader(new ProcessXmlConfigurationReader("NorthWind.xml").Read(), options).Read();
            new ProcessRunner(process).Run();

            // first run
            options = new Options { Mode = Modes.Normal, RenderTemplates = false };
            process = new ProcessReader(new ProcessXmlConfigurationReader("NorthWind.xml").Read(), options).Read();
            new ProcessRunner(process).Run();

            Assert.AreEqual(2155, process["Order Details"].Inserts);
            Assert.AreEqual(830, process["Orders"].Inserts);
            Assert.AreEqual(91, process["Customers"].Inserts);
            Assert.AreEqual(9, process["Employees"].Inserts);
            Assert.AreEqual(77, process["Products"].Inserts);
            Assert.AreEqual(29, process["Suppliers"].Inserts);
            Assert.AreEqual(8, process["Categories"].Inserts);
            Assert.AreEqual(3, process["Shippers"].Inserts);

            // second run
            options = new Options { Mode = Modes.Normal, RenderTemplates = false };
            process = new ProcessReader(new ProcessXmlConfigurationReader("NorthWind.xml").Read(), options).Read();
            new ProcessRunner(process).Run();

            Assert.AreEqual(0, process["Order Details"].Inserts);
            Assert.AreEqual(0, process["Orders"].Inserts);
            Assert.AreEqual(0, process["Customers"].Inserts);
            Assert.AreEqual(0, process["Employees"].Inserts);
            Assert.AreEqual(0, process["Products"].Inserts);
            Assert.AreEqual(0, process["Suppliers"].Inserts);
            Assert.AreEqual(0, process["Categories"].Inserts);
            Assert.AreEqual(0, process["Shippers"].Inserts);

            // add 1 fact
            using (var cn = process["Order Details"].InputConnection.GetConnection())
            {
                cn.Open();
                cn.Execute("insert into [Order Details](OrderID, ProductID, UnitPrice, Quantity, Discount) values(10261,41,7.70,2,0);");
            }

            // third run
            options = new Options { Mode = Modes.Normal, RenderTemplates = false };
            process = new ProcessReader(new ProcessXmlConfigurationReader("NorthWind.xml").Read(), options).Read();
            new ProcessRunner(process).Run();

            Assert.AreEqual(1, process["Order Details"].Inserts);
            Assert.AreEqual(0, process["Orders"].Inserts);
            Assert.AreEqual(0, process["Customers"].Inserts);
            Assert.AreEqual(0, process["Employees"].Inserts);
            Assert.AreEqual(0, process["Products"].Inserts);
            Assert.AreEqual(0, process["Suppliers"].Inserts);
            Assert.AreEqual(0, process["Categories"].Inserts);
            Assert.AreEqual(0, process["Shippers"].Inserts);

            //put things back
            using (var cn = process["Order Details"].InputConnection.GetConnection()) {
                cn.Open();
                cn.Execute("delete from [Order Details] where OrderID = 10261 and ProductID = 41;");
            }

        }
    }
}