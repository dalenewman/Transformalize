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
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline.Context {
    public class InputContext : IConnectionContext {
        readonly IContext _context;
        readonly IIncrement _incrementer;

        public Connection Connection { get; set; }
        public LogLevel LogLevel => _context.Logger.LogLevel;
        public int RowCapacity { get; set; }
        public Field[] InputFields { get; set; }

        public Entity Entity => _context.Entity;

        public Field Field => _context.Field;

        public Process Process => _context.Process;

        public Transform Transform => _context.Transform;

        public string Key { get; }
        public IPipelineLogger Logger => _context.Logger;

        public InputContext(IContext context, IIncrement incrementer) {
            _incrementer = incrementer;
            _context = context;
            RowCapacity = context.GetAllEntityFields().Count();
            InputFields = context.Entity.Fields.Where(f => f.Input).ToArray();
            Connection = context.Process.Connections.First(c => c.Name == context.Entity.Connection);
            Key = context.Key;
        }

        public void Increment(int by = 1) {
            _incrementer.Increment(by);
        }

        public void Debug(Func<string> lamda) {
            _context.Debug(lamda);
        }

        public void Error(string message, params object[] args) {
            _context.Error(message, args);
        }

        public void Error(Exception exception, string message, params object[] args) {
            _context.Error(exception, message, args);
        }

        public IEnumerable<Field> GetAllEntityFields() {
            return _context.GetAllEntityFields();
        }

        public IEnumerable<Field> GetAllEntityOutputFields() {
            return _context.GetAllEntityOutputFields();
        }

        public void Info(string message, params object[] args) {
            _context.Info(message, args);
        }

        public void Warn(string message, params object[] args) {
            _context.Warn(message, args);
        }
    }
}