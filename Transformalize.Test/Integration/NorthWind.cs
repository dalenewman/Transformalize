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

using System;
using System.Linq;
using NUnit.Framework;
using Transformalize.Libs.Dapper;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class NorthWind {
        private const string FILE = @"NorthWind.xml";

        [SetUp]
        public void SetUp() {
            LogManager.Configuration.LoggingRules[0].EnableLoggingForLevel(LogLevel.Info);
            LogManager.ReconfigExistingLoggers();
        }

        [Test]
        public void Init() {
            var options = new Options { Mode = "init"};
            var process = ProcessFactory.Create(FILE, options);
            process.Run();
            LogManager.Flush();
        }

        [Test]
        public void First() {
            var options = new Options() { Mode = "first" };
            var process = ProcessFactory.Create(FILE, options);
            process.Run();
            LogManager.Flush();
        }

        [Test]
        public void Normal() {
            var process = ProcessFactory.Create(FILE);
            process.Run();
            LogManager.Flush();
        }

        [Test]
        public void ManipulateData()
        {
            var process = ProcessFactory.Create(FILE);
            using (var cn = process["Order Details"].Input.First().Connection.GetConnection()) {
                cn.Open();
                var count = cn.Execute("insert into [Order Details](OrderID, ProductID, UnitPrice, Quantity, Discount) values(10261,41,7.70,2,0);");
                Console.WriteLine("row count: {0}", count);
            }
        }

        [Test]
        public void UnManipulateData() {
            var process = ProcessFactory.Create(FILE);
            using (var cn = process["Order Details"].Input.First().Connection.GetConnection()) {
                cn.Open();
                var count = cn.Execute("delete from [Order Details] where OrderID = 10261 and ProductID = 41;");
                Console.WriteLine("row count: {0}", count);
            }
        }
    }
}