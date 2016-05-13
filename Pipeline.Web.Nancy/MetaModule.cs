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
using Pipeline.Configuration;
using Pipeline.Ioc.Autofac;
using Environment = System.Environment;
using Response = Nancy.Response;

namespace Pipeline.Web.Nancy {
    public class MetaModule : NancyModule {

        public MetaModule(
            IContext context,
            Configuration.Process process,
            IRunTimeRun runner,
            Stopwatch stopwatch
        ) {

            Get["/api/meta/{id:int}"] = parameters => {
                context.Info("Executing Meta");
                var format = Request.Query["format"] ?? "json";
                var contentType = format == "xml" ? "application/xml" : "text/json";

                process.Load($"OrchardJob.xml?id={parameters.id}");

                if (process.Errors().Any()) {
                    return new Response {
                        ContentType = contentType,
                        StatusCode = HttpStatusCode.NotFound,
                        ReasonPhrase = string.Join(Environment.NewLine, process.Errors())
                    };
                }

                var rows = runner.Run(process).ToArray();
                if (rows.Length == 0) {
                    return new Response {
                        ContentType = contentType,
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                var cfg = rows.First().GetString(process.Entities.First().GetField("configuration"));

                process.Load(cfg, null);

                if (process.Errors().Any()) {
                    if (process.Errors().Any()) {
                        return new Response {
                            ContentType = contentType,
                            StatusCode = HttpStatusCode.ServiceUnavailable,
                            ReasonPhrase = string.Join(Environment.NewLine, process.Errors())
                        };
                    }
                }

                process.Templates.Clear();
                process.Maps.Clear();
                process.Actions.Clear();
                process.Entities.Clear();
                process.Relationships.Clear();
                process.CalculatedFields.Clear();
                process.SearchTypes.Clear();

                process.Environments.Clear();
                process.Request = "Meta";
                process.Status = 200;
                process.Message = string.Empty;
                process.Time = stopwatch.ElapsedMilliseconds;

                if (!process.Connections.First().Types.Any()) {
                    process.Connections.First().Types.Add(new TflType("int"));
                    process.Connections.First().Types.Add(new TflType("decimal"));
                    process.Connections.First().Types.Add(new TflType("datetime"));
                }

                process.Entities = new RunTimeSchemaReader(process, context).Read().Entities;

                var bytes = Encoding.UTF8.GetBytes(process.Serialize());
                return new Response {
                    ContentType = contentType,
                    Contents = s => s.Write(bytes, 0, bytes.Length)
                };
            };
        }
    }
}

