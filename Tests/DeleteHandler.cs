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
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Ext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize;
using Transformalize.Configuration;
using Transformalize.Context;
using Transformalize.Contracts;
using Transformalize.Desktop.Loggers;

namespace Tests {

    [TestClass]
    public class DeleteHandler {

        [TestMethod]
        public void NoDeletes() {
            var entity = GetTestEntity();
            entity.IsFirstRun = false;
            var input = GetTestReader(entity);
            var output = GetTestReader(entity);
            var deleter = new TestDeleter(entity, output.Data);
            var context = new PipelineContext(new TraceLogger(), new Process().WithValidation(), entity);
            var deleteHandler = new DefaultDeleteHandler(context, input, output, deleter);

            deleteHandler.Delete();
            Assert.AreEqual(2, deleter.Data.Count);
        }

        [TestMethod]
        public void OneDelete() {
            var entity = GetTestEntity();
            entity.MinVersion = 1;  // to make entity.IsFirstRun() == false for testing purposes
            var input = GetTestReader(entity);
            var output = GetTestReader(entity);

            // remove one from input
            var last = input.Data.Last();
            input.Data.Remove(last);

            var deleter = new TestDeleter(entity, output.Data);
            var context = new PipelineContext(new TraceLogger(), new Process().WithValidation(), entity);
            var deleteHandler = new DefaultDeleteHandler(context, input, output, deleter);

            deleteHandler.Delete();
            Assert.AreEqual(1, deleter.Data.Count, "output should loose one too");
        }

        [TestMethod]
        public void DeleteAll() {
            var entity = GetTestEntity();
            entity.ModifyIndexes();

            Assert.AreEqual(0, entity.Errors().Length);

            entity.IsFirstRun = false;
            var input = GetTestReader(entity);
            var output = GetTestReader(entity);

            // remove one from input
            input.Data.Clear();

            var deleter = new TestDeleter(entity, output.Data);
            var context = new PipelineContext(new TraceLogger(), new Process().WithValidation(), entity);
            var deleteHandler = new DefaultDeleteHandler(context, input, output, deleter);

            deleteHandler.Delete();
            Assert.AreEqual(0, deleter.Data.Count, "output should everthing too");
        }

        /// <summary>
        /// TestReader has to account for 4 system fields that are present
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static TestReader GetTestReader(Entity entity) {
            var capacity = entity.GetAllFields().Count();
            var rowFactory = new RowFactory(capacity, false, false);
            var row1 = rowFactory.Create();
            row1[entity.Fields[0]] = 1;
            row1[entity.Fields[1]] = "One";
            var row2 = rowFactory.Create();
            row2[entity.Fields[0]] = 2;
            row2[entity.Fields[1]] = "Two";

            var data = new List<IRow> { row1, row2 };
            return new TestReader(data);
        }

        private static Entity GetTestEntity() {
            var entity = new Entity {
                Name = "e1",
                Alias = "e1",
                Delete = true,
                Fields = new List<Field> {
                    new Field { Index = 0, Name="f1", Type="int", PrimaryKey = true}.WithValidation(),
                    new Field { Index = 1, Name="f2", Type="string", Length = "64"}.WithValidation()
                }
            }.WithValidation();
            entity.ModifyIndexes();
            return entity;
        }
    }

    public class TestDeleter : IDelete {
        private readonly Entity _entity;
        public List<IRow> Data { get; private set; }

        public TestDeleter(Entity entity, List<IRow> data) {
            _entity = entity;
            Data = data;
        }

        public void Delete(IEnumerable<IRow> rows) {
            Data = Data.Except(rows, new KeyComparer(_entity.GetPrimaryKey())).ToList();
        }
    }
}
