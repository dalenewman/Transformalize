#region license
// Transformalize
// Copyright 2013 Dale Newman
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Diagnostics;
using System.Linq;
using Nancy;
using Pipeline.Contracts;
using System.Text;
using Cfg.Net.Ext;
using Environment = System.Environment;
using Response = Nancy.Response;

namespace Pipeline.Web.Nancy {
    public class JobsModule : NancyModule {

        //IAction action
        public JobsModule(IContext context, IRunTimeRun runner, Configuration.Process process, Stopwatch stopwatch) {

            Get["/api/jobs"] = _ => {
                context.Info("Retrieving Jobs");

                process.Load("OrchardJobs.xml");

                if (process.Errors().Any()) {
                    return new Response {
                        ContentType = "text/json",
                        StatusCode = HttpStatusCode.NotFound,
                        ReasonPhrase = string.Join(Environment.NewLine, process.Errors())
                    };
                }

                var rows = runner.Run(process);

                var entity = process.Entities.First();
                var fields = entity.GetAllOutputFields().ToArray();
                foreach (var row in rows) {
                    entity.Rows.Add(row.ToStringDictionary(fields));
                }

                process.Status = 200;
                process.Message = "Ok";
                process.Time = stopwatch.ElapsedMilliseconds;

                var bytes = Encoding.UTF8.GetBytes(process.Serialize());
                return new Response {
                    ContentType = "text/json",
                    Contents = s => s.Write(bytes, 0, bytes.Length)
                };

            };
        }

    }
}

