#region license
// Transformalize
// A Configurable ETL Solution Specializing in Incremental Denormalization.
// Copyright 2013 Dale Newman
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
using System.Linq;
using Cfg.Net.Ext;
using Pipeline.Configuration;
using Pipeline.Contracts;
using Pipeline.Extensions;

namespace Pipeline.Context {
    public class PipelineContext : IContext {

        public Process Process { get; set; }
        public Entity Entity { get; set; }
        public Field Field { get; set; }
        public Transform Transform { get; set; }
        public object[] ForLog { get; }
        public IPipelineLogger Logger { get; set; }

        public LogLevel LogLevel => Logger.LogLevel;

        public string Key { get; }

        public PipelineContext(
            IPipelineLogger logger,
            Process process,
            Entity entity = null,
            Field field = null,
            Transform transform = null
        ) {
            ForLog = new object[4];
            Logger = logger;
            Process = process;
            Entity = entity ?? new Entity { Name = string.Empty, Alias = string.Empty }.WithDefaults();
            Field = field ?? new Field { Name = string.Empty, Alias = string.Empty }.WithDefaults();
            Transform = transform ?? new Transform { Method = string.Empty }.WithDefaults();
            Key = Process.Name + Entity.Key + Field.Alias + Transform.Method + Transform.Index;
            ForLog[0] = process.Name.PadRight(process.LogLimit, ' ').Left(process.LogLimit);
            ForLog[1] = Entity.Alias.PadRight(process.EntityLogLimit, ' ').Left(process.EntityLogLimit);
            ForLog[2] = Field.Alias.PadRight(process.FieldLogLimit, ' ').Left(process.FieldLogLimit);
            ForLog[3] = Transform.Method.PadRight(process.TransformLogLimit, ' ').Left(process.TransformLogLimit);
        }

        public void Info(string message, params object[] args) {
            Logger.Info(this, message, args);
        }

        public void Warn(string message, params object[] args) {
            Logger.Warn(this, message, args);
        }

        public void Debug(Func<string> lamda) {
            Logger.Debug(this, lamda);
        }

        public void Error(string message, params object[] args) {
            Logger.Error(this, message, args);
        }

        public void Error(Exception exception, string message, params object[] args) {
            Logger.Error(this, exception, message, args);
        }

        public IEnumerable<Field> GetAllEntityOutputFields() {
            return GetAllEntityFields().Where(f => f.Output);
        }

        /// <summary>
        /// Gets all fields for an entity.  Takes into account 
        /// the master entity's responsibility for carrying
        /// the process' calculated fields and 
        /// relationship fields.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Field> GetAllEntityFields() {

            foreach (var field in Entity.GetAllFields())
                yield return field;

            if (!Entity.IsMaster)
                yield break;

            foreach (var field in Process.CalculatedFields)
                yield return field;

            foreach (var field in GetRelationshipFields())
                yield return field;

        }

        public IEnumerable<Field> GetRelationshipFields() {

            var fields = new List<Field>();
            foreach (var r in Process.Relationships) {
                var leftCount = r.Summary.LeftEntity.RelationshipToMaster.Count();
                var rightCount = r.Summary.RightEntity.RelationshipToMaster.Count();
                if (r.Summary.LeftEntity.Alias != Entity.Alias && r.Summary.RightEntity.Alias != Entity.Alias) {
                    fields.AddRange(leftCount <= rightCount ? r.Summary.LeftFields : r.Summary.RightFields);
                }
            }

            return fields.Distinct();
        }


    }
}