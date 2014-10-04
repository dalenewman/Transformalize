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
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Sqloogle.Processes {

    /// <summary>
    /// A DefinitionProcess queries object definitions from each database
    /// on a server.  It also queries statistics (use and last used).
    /// </summary>
    public class DefinitionProcess : PartialProcessOperation {

        public DefinitionProcess(Process process, AbstractConnection connection)
            : base(process) {
            Register(new DatabaseExtract(connection));
            Register(new DefinitionExtract());
            Register(new CachedObjectStatsJoin(process).Right(new CachedObjectStatsExtract(connection)));
            Register(new TableStatsJoin(process).Right(new TableStatsExtract(connection)));
            Register(new IndexStatsJoin(process).Right(new IndexStatsExtract(connection)));
        }
    }
}
