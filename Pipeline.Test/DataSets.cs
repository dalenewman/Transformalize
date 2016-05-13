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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cfg.Net.Reader;
using Cfg.Net.Shorthand;
using NUnit.Framework;
using Pipeline.Configuration;
using Pipeline.Context;
using Pipeline.Contracts;
using Pipeline.Logging;
using Pipeline.Scripting.Jint;

namespace Pipeline.Test {

    [TestFixture]
    public class DataSets {

        static Field FieldAt(short index) {
            return new Field { Index = index, MasterIndex = index };
        }

        [Test(Description = "A DataSet can be stored in an configuration, typed, and enumerated through.")]
        public void GetTypedDataSet() {

            var cfg = new FileReader().Read(@"Files\PersonAndPet.xml", null, new Cfg.Net.Loggers.NullLogger());
            var sh = new ShorthandRoot(@"Files\Shorthand.xml", new FileReader());
            var process = new Process(cfg, new JintValidator("js"), new ShorthandModifier(sh, "sh"));

            var personContext = new PipelineContext(new DebugLogger(), process, process.Entities.Last());
            var entityInput = new InputContext(personContext, new Incrementer(personContext));
            var rowFactory = new RowFactory(entityInput.RowCapacity, entityInput.Entity.IsMaster, false);
            var rows = new DataSetEntityReader(entityInput, rowFactory).Read().ToArray();

            Assert.IsInstanceOf<IEnumerable<IRow>>(rows);
            Assert.AreEqual(3, rows.Length);

            var dale = rows[0];
            var micheal = rows[1];
            Assert.IsInstanceOf<int>(dale[FieldAt(4)]);
            Assert.AreEqual(1, dale[FieldAt(4)]);
            Assert.AreEqual("Dale", dale[FieldAt(5)]);
            Assert.AreEqual("Michael", micheal[FieldAt(5)]);

            foreach (var row in rows) {
                Console.WriteLine(row);
            }
        }
    }
}
