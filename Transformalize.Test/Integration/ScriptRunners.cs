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
using Transformalize.Configuration.Builders;
using Transformalize.Main;
using Transformalize.Main.Providers;
using Transformalize.Main.Providers.AnalysisServices;

namespace Transformalize.Test.Integration {
    [TestFixture]
    public class ScriptRunners {

        [Test]
        public void TestBadXmlaScript()
        {

            const string script = @"
<Batch xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
  <Process xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:ddl2=""http://schemas.microsoft.com/analysisservices/2003/engine/2"" xmlns:ddl2_2=""http://schemas.microsoft.com/analysisservices/2003/engine/2/2"" xmlns:ddl100_100=""http://schemas.microsoft.com/analysisservices/2008/engine/100/100"" xmlns:ddl200=""http://schemas.microsoft.com/analysisservices/2010/engine/200"" xmlns:ddl200_200=""http://schemas.microsoft.com/analysisservices/2010/engine/200/200""> 
    <Object>
      <DatabaseID>Es</DatabaseID>
    </Object>
    <Type>ProcessFull</Type>
    <WriteBackTableCreation>UseExisting</WriteBackTableCreation>
  </Process>
</Batch>
";
            var cfg = new ProcessBuilder("test")
                .Connection("test")
                    .Server("localhost")
                    .Database("Es")
                    .Provider(ProviderType.AnalysisServices)
                .Connection("output")
                    .Provider(ProviderType.Internal)
                .Process();

            var process = ProcessFactory.Create(cfg)[0];

            var connection = process.Connections["test"];

            var runner = new AnalysisServicesScriptRunner();

            var response = runner.Execute(connection, script);

            Assert.IsFalse(response.Success);
            Assert.Less(0, response.Messages.Count);

        }

        [Test]
        public void TestGoodXmlaScript()
        {
            const string script = @"
<Batch xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine"">
  <Process xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:ddl2=""http://schemas.microsoft.com/analysisservices/2003/engine/2"" xmlns:ddl2_2=""http://schemas.microsoft.com/analysisservices/2003/engine/2/2"" xmlns:ddl100_100=""http://schemas.microsoft.com/analysisservices/2008/engine/100/100"" xmlns:ddl200=""http://schemas.microsoft.com/analysisservices/2010/engine/200"" xmlns:ddl200_200=""http://schemas.microsoft.com/analysisservices/2010/engine/200/200"">
    <Object>
      <DatabaseID>NorthWind2</DatabaseID>
    </Object>
    <Type>ProcessFull</Type>
    <WriteBackTableCreation>UseExisting</WriteBackTableCreation>
  </Process>
</Batch>
";
            var cfg = new ProcessBuilder("test")
                .Connection("test")
                    .Server("localhost")
                    .Database("NorthWind")
                    .Provider(ProviderType.AnalysisServices)
                .Connection("output")
                    .Provider(ProviderType.Internal)
                .Process();

            var process = ProcessFactory.Create(cfg)[0];

            var connection = process.Connections["test"];

            var runner = new AnalysisServicesScriptRunner();

            var response = runner.Execute(connection, script);

            Assert.IsTrue(response.Success);
            Assert.IsTrue(response.Messages.Count == 0);

        }

    }

}