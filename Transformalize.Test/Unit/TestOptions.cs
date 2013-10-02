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

using System.Configuration;
using NUnit.Framework;
using Transformalize.Configuration;
using Transformalize.Main;

namespace Transformalize.Test.Unit
{
    [TestFixture]
    public class TestOptions
    {
        [Test]
        public void TestOptionsWithBoolean()
        {
            const string args = "{ \"Mode\":\"Init\", \"RenderTemplates\":false }";

            var options = new Options(args);

            Assert.IsFalse(options.RenderTemplates);
            Assert.AreEqual("init", options.Mode);
        }

        [Test]
        public void TestOptionsWithStrings()
        {
            const string args = "{ \"Mode\":\"Init\", \"RenderTemplates\":\"False\" }";

            var options = new Options(args);

            Assert.IsFalse(options.RenderTemplates);
            Assert.AreEqual("init", options.Mode);
        }

        [Test]
        public void TestSerializeConfig()
        {
            var configIn = (TransformalizeConfiguration) ConfigurationManager.GetSection("transformalize");
            var configOut = new TransformalizeConfiguration();
            Assert.IsNotNull(configIn);
            var xml = configIn.Serialize();

            xml = xml.Replace("\"output\"", "\"OUT\"");

            configOut.Deserialize(xml);

            Assert.AreEqual("OUT", configOut.Processes[0].Connections[1].Name);
        }
    }
}