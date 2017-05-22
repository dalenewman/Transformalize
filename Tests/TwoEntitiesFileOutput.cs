#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Cfg.Net.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Ioc.Autofac;
using Transformalize.Logging;
using Environment = System.Environment;

namespace Tests {

    [TestClass]
    public class TwoEntitiesFileOutput {

        [TestMethod]
        public void Execute() {
            #region cfg

            var xml = @"
    <add name='CombineInput' mode='default'>

      <entities>

        <add name='Feeding'>
          <fields>
            <add name='Id' type='byte' primary-key='true' />
            <add name='When' type='datetime' />
            <add name='PetId' type='byte' />
          </fields>
          <rows>
            <add Id='1' When='2017-02-03 9:00 AM' PetId='1' />
            <add Id='2' When='2017-02-03 9:05 AM' PetId='2' />
            <add Id='3' When='2017-02-03 1:30 PM' PetId='1' />
            <add Id='4' When='2017-02-03 5:00 PM' PetId='1' />
            <add Id='5' When='2017-02-03 5:03 PM' PetId='2' />
          </rows>
        </add>

        <add name='Pet'>
            <fields>
                <add name='Id' type='byte' primary-key='true' />
                <add name='Name' />
            </fields>
            <rows>
                <add Id='1' Name='Lucy' />
                <add Id='2' Name='Hammond' />
            </rows>
        </add>

      </entities>

      <relationships>
        <add left-entity='Feeding' left-field='PetId' right-entity='Pet' right-field='Id' />
      </relationships>
    </add>";

            #endregion

            var provider = "sqlite";
            var ext = "sqlite3";

            var process = ProcessFactory.Create(xml);

            if (!process.Errors().Any()) {

                var originalOutput = process.Output().Clone();

                if (process.Entities.Count > 1 && !process.OutputIsRelational()) {

                    process.Output().Provider = provider;
                    var file = new FileInfo(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Transformalize"), process.Name + "." + ext));
                    process.Output().File = file.FullName;
                    Console.WriteLine(file.FullName);
                    process.Flatten = provider == "sqlce";
                    process.Mode = "init";

                    if (!file.Exists) {
                        if (!Directory.Exists(file.DirectoryName)) {
                            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Transformalize"));
                        }
                    }

                    using (var scope = DefaultContainer.Create(process, new DebugLogger())) {
                        scope.Resolve<IProcessController>().Execute();
                    }

                    var threshold = process.Entities.Min(e => e.BatchId) - 1;

                    var reversed = new Process {
                        Name = process.Name,
                        ReadOnly = true,
                        Connections = new List<Connection>(2){
                            new Connection { Name = "input", Provider = provider, File = file.FullName},
                            originalOutput
                        },
                        Entities = new List<Entity>(1) {
                            new Entity {
                                Name = provider == "sqlce" ? process.Flat : process.Star,
                                CalculateHashCode = false,
                                Connection = "input",
                                Fields = process.GetStarFields().SelectMany(f => f).Select(field => new Field {
                                    Name = field.Alias,
                                    Alias = field.Alias,
                                    Type = field.Type,
                                    Input = true,
                                    PrimaryKey = field.Name == Transformalize.Constants.TflKey
                                }).ToList(),
                                Filter = new List<Filter> {
                                    new Filter {
                                        Field = Transformalize.Constants.TflBatchId,
                                        Operator = "greaterthan",
                                        Value = threshold.ToString()
                                    }
                                }
                            }
                        }
                    };

                    reversed.Check();
                    if (reversed.Errors().Any()) {
                        foreach (var error in reversed.Errors()) {
                            Console.WriteLine(error);
                        }
                    }

                    using (var scope = DefaultContainer.Create(reversed, new DebugLogger())) {
                        scope.Resolve<IProcessController>().Execute();
                        if (originalOutput.Provider == "internal") {
                            process.Rows = reversed.Entities.First().Rows;
                        }
                        Assert.AreEqual(5, process.Rows.Count);
                    }

                }
            }

            
        }
    }
}