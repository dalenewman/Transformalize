/*
   Copyright 2013 Dale Newman

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Sqloogle.Operations;
using Transformalize.Libs.Sqloogle.Operations.Support;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Sqloogle.Processes {

    /// <summary>
    /// A ServerProcess queries cached sql, sql agent jobs,
    /// reporting services commands, and object definitions from
    /// a single SQL Server. 
    /// </summary>
    public class ServerCrawlProcess : PartialProcessOperation {

        public ServerCrawlProcess(Process process, AbstractConnection connection) : base(process)
        {

            var union = new ParallelUnionAllOperation(
                new DefinitionProcess(process, connection),
                new CachedSqlProcess(process, connection),
                new SqlAgentJobExtract(connection),
                new ReportingServicesProcess(process, connection)
            );

            Register(union);
            RegisterLast(new AppendToRowOperation("server", connection.Server));

        }
    }
}
