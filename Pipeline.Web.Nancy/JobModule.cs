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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nancy;
using Pipeline.Contracts;
using System.Text;
using Environment = System.Environment;
using Response = Nancy.Response;

namespace Pipeline.Web.Nancy {
    public class JobModule : NancyModule {

        public JobModule(IContext context, IRunTimeRun runner, Configuration.Process process, Stopwatch stopwatch) {

            Get["/api/job/{id:int}"] = parameters => {
                context.Info("Retrieving Job");

                process.Load($"OrchardJob.xml?id={parameters.id}");

                if (process.Errors().Any()) {
                    return new Response {
                        ContentType = "text/json",
                        StatusCode = HttpStatusCode.NotFound,
                        ReasonPhrase = string.Join(Environment.NewLine, process.Errors())
                    };
                }

                var rows = runner.Run(process).ToArray();
                if (rows.Length == 0) {
                    return new Response {
                        ContentType = "text/json",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var cfg = rows.First().GetString(process.Entities.First().GetField("configuration"));

                process.Load(cfg, null);
                process.Status = 200;
                process.Message = string.Empty;
                process.Request = "Job";
                process.Time = stopwatch.ElapsedMilliseconds;
                process.RemoveSystemFields();

                var bytes = Encoding.UTF8.GetBytes(process.Serialize());
                return new Response {
                    ContentType = "text/json",
                    Contents = s => s.Write(bytes, 0, bytes.Length)
                };
            };

        }
    }
}

